module App.State

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Browser
open Global
open Types
open Imports
open Common
open System

let cubeheadsAbi : obj = import "cubeheads" "./abis.js"
let cubeletsAbi : obj = import "cubelets" "./abis.js"
let cubetrophiesAbi : obj = import "cubetrophies" "./abis.js"
let cubeballAbi : obj = import "cubeball" "./abis.js"
let cubemintingAbi : obj = import "cubeminting" "./abis.js"

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map Cubehead (s "cubehead" </> i32)
        map Home (s "")
        map About (s "about")
        map Counter (s "counter")
        map Home (s "home")
        map Cubeball (s "cubeball")
        map Gallery (s "gallery")
        map UserGuide (s "guide")
    ]

let urlUpdate gbl (result : Page option) model =
    match result with
    | None ->
        console.error("Error parsing url")
        model, Navigation.modifyUrl (toHash model.CurrentPage)
    | Some (Cubehead index) ->
        let prevPage =
            match model.CurrentPage with
            | Home -> None
            | page -> Some page
        { model with
            CurrentPage = Cubehead index
            cubehead = Some { previousPage = prevPage; cubehead = None } }, Cmd.none
    | Some page ->
        { model with CurrentPage = page }, []

let createContracts (web3 :Web3) =
    // create contracts
    promise {
        let cubeheadsContract =  web3.NewContract(cubeheadsAbi, Config.network.cubeheadsContractAddress)
        let! cubeletsContractAddr = cubeheadsContract?methods?cubeletsContractAddress()?call()
        let cubeletsContract = web3.NewContract(cubeletsAbi, cubeletsContractAddr)
        let! cubeballContractAddr = cubeheadsContract?methods?cubeballContractAddress()?call()
        let cubeballContract = web3.NewContract(cubeballAbi, cubeballContractAddr)
        let! cubetrophiesContractAddr = cubeheadsContract?methods?cubetrophiesContractAddress()?call()
        let cubetrophiesContract = web3.NewContract(cubetrophiesAbi, cubetrophiesContractAddr)
        let! cubemintingContractAddr = cubeheadsContract?methods?cubemintingContractAddress()?call()
        let cubemintingContract = web3.NewContract(cubemintingAbi, cubemintingContractAddr)
        console.log ("cubeheads contract: " + Config.network.cubeheadsContractAddress)
        console.log ("cubelets contract: " + cubeletsContractAddr)
        console.log ("cubeball contract: " + cubeballContractAddr)
        console.log ("cubetrophies contract: " + cubetrophiesContractAddr)
        console.log ("cubeheads minting contract: " + cubemintingContractAddr)
        return {
            cubeheadsContract = cubeheadsContract
            cubemintingContract = cubemintingContract
            cubeletsContract = cubeletsContract
            cubeballContract = cubeballContract
            cubetrophiesContract = cubetrophiesContract
        } }

let getAuctionBatchFromEvent contracts (web3 :obj) (currentAuction :obj) =
    promise {
        // load auction
        let origindate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    
        let startTimeUx = currentAuction?returnValues?startTime
        let repeatTime = currentAuction?returnValues?repeatTime
        let t0 = currentAuction?returnValues?token0
        let t1 = currentAuction?returnValues?token1
        let t2 = currentAuction?returnValues?token2
        let t3 = currentAuction?returnValues?token3
        let t4 = currentAuction?returnValues?token4
        let t5 = currentAuction?returnValues?token5

        let tokenIds = [|t0; t1; t2; t3; t4; t5 |]

        let startTime = origindate.AddSeconds( startTimeUx )
        let now = DateTime.UtcNow
        let totalDur = (now - startTime).TotalSeconds |> int
        let num = totalDur / repeatTime
        let startTime = startTime.AddSeconds(float (repeatTime * num))
        let endTime = startTime.AddSeconds(float repeatTime)

        let cubeheads = tokenIds |> Array.map (fun tid -> cubeheadsDataByIndex.[tid] |> Cubehead.fromCubeheadData)

        let getMintEvent tokenId =
            contracts.cubemintingContract?getPastEvents "Mint"
                (keyValueList CaseRules.LowerFirst
                    ["fromBlock", !!0
                     "toBlock", !!"latest"
                     "filter", keyValueList CaseRules.LowerFirst ["tokenId", tokenId]])

        let! mintEvents = tokenIds |> Array.map getMintEvent |> Promise.Parallel

        let auctions =
            Array.zip cubeheads mintEvents
                |> Array.map (fun (cubehead, event) ->
                    let priceSold = if event?length > 0 then Some 1M else None
                    { Home.Types.Auction.cubehead = cubehead; Home.Types.Auction.priceSold = priceSold; Home.Types.Auction.minting = None })
                |> Array.toList

        let! price = contracts.cubemintingContract?methods?getAuctionPrice()?call()
        
        return 
            { Home.Types.endTime = endTime
              Home.Types.auctions = auctions
              Home.Types.timeRemaining = endTime - now
              Home.Types.price = web3?utils?fromWei(price, "ether")
              Home.Types.priceRaw = price
              Home.Types.repeatTimeSecs = repeatTime } 
        }



let init gbl result =
    let initWeb3 = 
        Cmd.OfPromise.perform 
            (fun _ -> promise {
                let web3 = gbl.web3

                let! contracts = createContracts web3
                gbl.contracts <- Some contracts

                let! auctionEvents = contracts.cubemintingContract?getPastEvents "Auction" (keyValueList CaseRules.LowerFirst [("fromBlock", !!0); ("toBlock", !!"latest")])
                let currentAuction = auctionEvents?at(auctionEvents?length - 1)

                let! auctionBatch = getAuctionBatchFromEvent contracts web3 currentAuction
                return {
                    auctionBlock = !!currentAuction?blockNumber
                    auction = auctionBatch
                }
            }) () SetWeb3InitData
    let (counter, counterCmd) = Counter.State.init()
    let (home, homeCmd) = Home.State.init()
    let (model, cmd) =
        let (gchs1::gchs) = Cubehead.getAllCubeheadsPaged [] None
        urlUpdate gbl result
          { CurrentPage = Home
            Counter = counter
            Home = home
            accountData = None
            cubehead = None
            navbarMenuActive = false
            gallery = { filter = Map.empty; idSearch = ""; cubeheads = [gchs1]; filteredCubeheads = gchs }
            cubeball =
              { activeTab = Cubeball.Types.Tab.AllCubeheads
                cubeheads = None
                team = None
                draftTeam = [|None; None; None; None|]
                selectingCubehead = None
                results = None
                saving = false
                messageToSign = None } }
    model, Cmd.batch [ cmd
                       Cmd.map CounterMsg counterCmd
                       Cmd.map HomeMsg homeCmd 
                       initWeb3 ]

let update gbl msg model =
    let updateAccountCmd web3 =
        Cmd.OfPromise.perform
            (fun _ -> promise {
                let! chainId = web3?eth?getChainId()
                let! accounts = web3?eth?getAccounts()
                let selectedAccount = accounts?at $ 0
                let signedMessage = WebStorage.localStorage.["signed-message: " + selectedAccount]
                return
                    { AccountData.selectedAccount = selectedAccount
                      chainId = chainId
                      signedMessage = if System.String.IsNullOrEmpty signedMessage then None else Some signedMessage }
            }) () SetAccountData
    let getPriceCmd =
        Cmd.OfPromise.perform
            (fun _ -> promise {
                match gbl.contracts with
                | Some contracts ->
                    let! price = contracts.cubemintingContract?methods?getAuctionPrice()?call()
                    return (price, gbl.web3?utils?fromWei(price, "ether"))
            }) () SetAuctionPrice
    match msg with
    | CounterMsg msg ->
        let (counter, counterCmd) = Counter.State.update msg model.Counter
        { model with Counter = counter }, Cmd.map CounterMsg counterCmd
    | HomeMsg (Home.Types.MintCubehead index) ->
        match model.accountData, model.Home.currentAuction with
        | Some { selectedAccount = account }, Some auction ->
            let tokenData = Common.cubeheadsDataByIndex.[index].data
            let merklePath = Common.getMerklePath gbl.cubeheadsMerkleTree index
            { model with
                Home =
                    { model.Home with
                        currentAuction =
                            Some { auction with
                                    auctions =
                                        auction.auctions
                                            |> List.map (fun a ->
                                                if a.cubehead.index = index then { a with minting = Some "" } else a) } } },
                Cmd.OfPromise.either
                    (fun _ -> promise {
                        let! result = gbl.contracts.Value.cubemintingContract?methods?mint(index, tokenData, merklePath)?send(createObj ["from", !!account; "value", !!auction.priceRaw]) 
                        return result
                    }) () (fun receipt -> HomeMsg (Home.Types.MintSuccess(index, receipt))) (fun exn -> HomeMsg (Home.Types.MintFail(index, exn)))
        | _ -> model, Cmd.none
    | HomeMsg (Home.Types.MintSuccess (index, receipt)) ->
        match model.Home.currentAuction with
        | Some auction ->
            { model with
                Home =
                    { model.Home with
                        currentAuction =
                            Some { auction with
                                    auctions =
                                        auction.auctions
                                            |> List.map (fun a ->
                                                if a.cubehead.index = index then { a with minting = None; priceSold = Some 0M } else a) } } }, Cmd.none
        | _ -> model, Cmd.none
    | HomeMsg (Home.Types.MintFail (index, exn)) ->
        match model.Home.currentAuction with
        | Some auction ->
            { model with
                Home =
                    { model.Home with
                        currentAuction =
                            Some { auction with
                                    auctions =
                                        auction.auctions
                                            |> List.map (fun a ->
                                                if a.cubehead.index = index then { a with minting = None } else a) } } }, Cmd.none
        | _ -> model, Cmd.none
    | HomeMsg msg ->
        let (home, homeCmd) = Home.State.update msg model.Home
        { model with Home = home }, Cmd.map HomeMsg homeCmd
    | NavMsg Navbar.Types.Msg.ConnectWallet ->
        model, Cmd.OfPromise.perform gbl.web3Modal.connect () SetProvider
    | NavMsg Navbar.Types.Msg.ToggleNavbarMenu ->
        { model with navbarMenuActive = not model.navbarMenuActive }, Cmd.none
    | CubeballMsg (Cubeball.Types.SignMessage) ->
        match model with
        | { cubeball = { messageToSign = Some message }; accountData = Some data } ->
            model,
                Cmd.OfPromise.perform (fun _ -> promise {
                    let! signedMessage = gbl.web3?eth?personal?sign(message, data.selectedAccount)
                    return signedMessage
                }) () SetSignedMessage
        | _ -> model, Cmd.none
    | CubeballMsg (Cubeball.Types.SaveTeamChanges) ->
        match model with
        | { accountData = Some { signedMessage = Some _; selectedAccount = account }; cubeball = { team = Some team } } ->
            let salt = gbl.web3?utils?randomHex(32)
            let team =
                match model.cubeball.team with
                | None | Some [||] -> model.cubeball.draftTeam |> Array.choose id
                | Some team -> Array.zip team model.cubeball.draftTeam |> Array.map (function | (_, Some (i, c)) -> (i, c) | c, _ -> c)
            let teamArray = Array.append (team |> Array.map fst) (team |> Array.filter snd |> Array.map fst)
            // fix indices for breeders
            teamArray.[4] <-
                match teamArray.[4] with
                | x when teamArray.[0] = x -> 0
                | x when teamArray.[1] = x -> 1
                | x when teamArray.[2] = x -> 2
                | x when teamArray.[3] = x -> 3
            teamArray.[5] <-
                match teamArray.[5] with
                | x when teamArray.[0] = x -> 0
                | x when teamArray.[1] = x -> 1
                | x when teamArray.[2] = x -> 2
                | x when teamArray.[3] = x -> 3
            let hash = gbl.web3?utils?soliditySha3(teamArray.[0], teamArray.[1], teamArray.[2], teamArray.[3], teamArray.[4], teamArray.[5], salt)
            console.log("hash: " + hash)
            { model with cubeball = { model.cubeball with saving = true } },
                Cmd.OfPromise.either (fun _ ->
                    promise {
                        let value = gbl.web3?utils?toWei("0.02", "ether")
                        let cubeheadId = teamArray |> Array.find (fun i -> i < 1995)
                        let! result = gbl.contracts.Value.cubeballContract?methods?commitTeams(cubeheadId, hash)?send(createObj ["from", !!account; "value", !!value]) 
                        return salt, teamArray, result, hash
                    }) () (CubeballMsg << Cubeball.Types.CommitTeamSucceeded) (CubeballMsg << Cubeball.Types.CommitTeamFailed)
        | _ -> model, Cmd.none
    | CubeballMsg (Cubeball.Types.CommitTeamSucceeded (salt, teamArray, receipt, hash)) ->
        match model with
        | { accountData = Some { signedMessage = Some signedMessage; selectedAccount = account }; cubeball = { team = Some team } } ->
            { model with cubeball = { model.cubeball with saving = true } },
                Cmd.OfAsync.perform (fun _ -> Server.saveTeam signedMessage account { date = System.DateTime.UtcNow; account = account; team = teamArray; teamHash = hash; salt = salt }
                    ) () (fun _ -> CubeballMsg (Cubeball.Types.SaveSucceeded))
        | _ -> model, Cmd.none
    | CubeballMsg msg ->
        let (cubeball, cubeballCmd) = Cubeball.State.update msg model.cubeball
        { model with cubeball = cubeball }, Cmd.map CubeballMsg cubeballCmd
    | GalleryMsg msg ->
        let (gallery, galleryCmd) = Gallery.State.update msg model.gallery
        { model with gallery = gallery }, Cmd.map GalleryMsg galleryCmd
    | SetSignedMessage signedMessage ->
        match model with
        | { accountData = Some data } ->
            WebStorage.localStorage.["signed-message: " + data.selectedAccount] <- signedMessage
            { model with accountData = Some { data with signedMessage = Some signedMessage } },
                Cmd.OfAsync.perform (Server.getTeam) (signedMessage, data.selectedAccount) SetTeam
        | _ -> model, Cmd.none
    | SetTeam team ->
        match model with
        | { cubeball = cubeball } ->
            { model with cubeball = { cubeball with team = Some team } }, Cmd.none
    | SetProvider provider ->
        gbl.window?web3provider <- provider
        let web3 = new Web3(provider)
        gbl.window?web3_ <- web3
        gbl.web3 <- web3
        model, updateAccountCmd web3
    | SetAccountData data ->
        match model.accountData with
        | Some accountData when accountData.selectedAccount = data.selectedAccount && accountData.chainId = data.chainId ->
            model, Cmd.none
        | _ ->
            // recreate contracts
            { model with accountData = Some data },
                Cmd.OfAsync.perform (fun _ ->
                    async {
                        let! contracts = createContracts gbl.web3 |> Async.AwaitPromise
                        gbl.contracts <- Some contracts
                        let! cubeheads = Cubehead.getCubeheadsForAccount gbl.contracts.Value data.selectedAccount
                        let! results = Cubehead.getResultsForAccount gbl.contracts.Value data.selectedAccount
                        let! msgToSign = Server.getAuthenticationMessage data.selectedAccount
                        let wcubeheads = cubeheads |> Array.map (fun c -> { cubehead = c; teamStatus = TeamStatus.NotInTeam })
                        match data.signedMessage with
                        | None -> 
                            return { cubeheads = wcubeheads; team = [||]; results = results |> Array.toList; teamFetched = false; messageToSign = msgToSign }
                        | Some msg ->
                            let! team = Server.getTeam (msg, data.selectedAccount)
                            return { cubeheads = wcubeheads; team = team; results = results |> Array.toList; teamFetched = true; messageToSign = msgToSign } }) () SetWalletData
    | SetWalletData walletData ->
        { model with
            cubeball =
                { model.cubeball with
                    cubeheads = Some walletData.cubeheads
                    team = match walletData.teamFetched with | true -> Some walletData.team | false -> None
                    results = Some walletData.results
                    messageToSign = Some walletData.messageToSign } }, Cmd.none
    | SetWeb3InitData web3Data ->
        // load cubehead if cubehead page
        let cmd =
            match model.CurrentPage with
            | Cubehead index -> Cmd.OfAsync.perform (fun _ -> Cubehead.getCubeheadGenericByIndex gbl.contracts.Value index) () SetCubehead
            | _ -> Cmd.none
        // event subscriptions
        let eventSubCmd =
            Cmd.ofSub (fun dispatch ->
                gbl.contracts.Value.cubemintingContract?events?Mint(createObj ["fromBlock", !!(web3Data.auctionBlock)])?on("data", fun event ->
                    dispatch <| MintEvent event)
                gbl.contracts.Value.cubemintingContract?events?Auction(createObj ["fromBlock", !!(web3Data.auctionBlock)])?on("data", fun event ->
                    dispatch <| AuctionEvent event)
                ())
        { model with Home = { model.Home with currentAuction = Some web3Data.auction } }, Cmd.batch [cmd; eventSubCmd]
    | SetAuctionPrice (priceRaw, price) ->
        match model.Home.currentAuction with
        | None -> model, Cmd.none
        | Some auction ->
            { model with Home = { model.Home with currentAuction = Some { auction with price = price; priceRaw = priceRaw } } }, Cmd.none
    | SetCubehead cubehead ->
        match model.cubehead with
        | None -> model, Cmd.none
        | Some c -> { model with cubehead = Some { c with cubehead = Some cubehead} }, Cmd.none
    | SetAuction auction ->
        { model with Home = { model.Home with currentAuction = Some auction } }, Cmd.none
    | MintEvent event ->
        let index = event?returnValues?tokenId
        match model.Home.currentAuction with
        | None -> model, Cmd.none
        | Some auction ->
            let auctions = auction.auctions |> List.map (fun a -> if a.cubehead.index = index then { a with priceSold = Some 1M } else a)
            { model with Home = { model.Home with currentAuction = Some { auction with auctions = auctions } } }, Cmd.none
    | AuctionEvent event ->
        model, Cmd.OfPromise.perform (getAuctionBatchFromEvent gbl.contracts.Value gbl.web3) event SetAuction
    | TimerTick ->
        match model.Home.currentAuction with
        | None -> model, Cmd.none
        | Some auction ->
            let cmd =
                match model.accountData with
                | None -> getPriceCmd
                | Some _ -> Cmd.batch [updateAccountCmd gbl.window?web3_; getPriceCmd]
            let endTime = if (auction.endTime - date.Now).TotalSeconds < 0.0 then auction.endTime.AddSeconds(float auction.repeatTimeSecs) else auction.endTime
            { model with Home = { model.Home with currentAuction = Some { auction with timeRemaining = auction.endTime - date.Now; endTime = endTime } } },
                cmd
    | _ -> model, Cmd.none
