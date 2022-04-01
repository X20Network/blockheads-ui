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

let urlUpdate (result : Page option) model =
    match result with
    | None ->
        console.error("Error parsing url")
        model, Navigation.modifyUrl (toHash model.CurrentPage)
    | Some (Cubehead index) ->
        let cubehead = Cubehead.getCubeheadByIndex index
        let prevPage =
            match model.CurrentPage with
            | Home -> None
            | page -> Some page
        { model with
            CurrentPage = Cubehead index
            cubehead = Some { previousPage = prevPage; cubehead = cubehead } }, []
    | Some page ->
        { model with CurrentPage = page }, []

let init gbl result =
    let initWeb3 = 
        Cmd.OfPromise.perform 
            (fun _ -> promise {
                let web3 = gbl.web3
                let cubeheadsContract =  web3.NewContract(cubeheadsAbi, Config.network.cubeheadsContractAddress)
                gbl.window?cubeheadsContract <- cubeheadsContract
                let! cubeletsContractAddr = cubeheadsContract?methods?cubeletsContractAddress()?call()
                let cubeletsContract = web3.NewContract(cubeletsAbi, cubeletsContractAddr)
                gbl.window?cubeletsContract <- cubeletsContract
                let! cubeballContractAddr = cubeheadsContract?methods?cubeballContractAddress()?call()
                let cubeballContract = web3.NewContract(cubeballAbi, cubeballContractAddr)
                gbl.window?cubeballContract <- cubeballContract
                let! cubetrophiesContractAddr = cubeheadsContract?methods?cubetrophiesContractAddress()?call()
                let cubetrophiesContract = web3.NewContract(cubetrophiesAbi, cubetrophiesContractAddr)
                gbl.window?cubetrophiesContract <- cubetrophiesContract
                let! cubemintingContractAddr = cubeheadsContract?methods?cubemintingContractAddress()?call()
                let cubemintingContract = web3.NewContract(cubemintingAbi, cubemintingContractAddr)
                gbl.window?cubemintingContract <- cubemintingContract

                let! auctionEvents = cubemintingContract?getPastEvents "Auction" (keyValueList CaseRules.LowerFirst [("fromBlock", !!0); ("toBlock", !!"latest")])
                let currentAuction = auctionEvents?at(auctionEvents?length - 1)

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
                    cubemintingContract?getPastEvents "Mint"
                        (keyValueList CaseRules.LowerFirst
                            ["fromBlock", !!0
                             "toBlock", !!"latest"
                             "filter", keyValueList CaseRules.LowerFirst ["tokenId", tokenId]])

                let! mintEvents = tokenIds |> Array.map getMintEvent |> Promise.Parallel

                let auctions =
                    Array.zip cubeheads mintEvents
                        |> Array.map (fun (cubehead, event) ->
                            let priceSold = if event?length > 0 then Some 1M else None
                            { Home.Types.Auction.cubehead = cubehead; Home.Types.Auction.priceSold = priceSold })
                        |> Array.toList

                let! price = cubemintingContract?methods?getAuctionPrice()?call()

                return 
                    { auction =
                        { endTime = endTime
                          auctions = auctions
                          timeRemaining = endTime - now
                          price = web3?utils?fromWei(price, "ether")
                          priceRaw = price
                            } }
            }) () SetWeb3InitData
    let (counter, counterCmd) = Counter.State.init()
    let (home, homeCmd) = Home.State.init()
    let (model, cmd) =
        let (gchs1::gchs) = Cubehead.getAllCubeheadsPaged [] None
        urlUpdate result
          { CurrentPage = Home
            Counter = counter
            Home = home
            accountData = None
            cubehead = None
            gallery = { filter = Map.empty; idSearch = ""; cubeheads = [gchs1]; filteredCubeheads = gchs }
            cubeball =
              { activeTab = Cubeball.Types.Tab.AllCubeheads
                cubeheads = None
                team = None
                draftTeam = [|None; None; None; None|]
                selectingCubehead = None
                results = None
                saving = false } }
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
                return { AccountData.selectedAccount = selectedAccount; chainId = chainId }
            }) () SetAccountData
    let getPriceCmd =
        Cmd.OfPromise.perform
            (fun _ -> promise {
                let! price = gbl.window?cubemintingContract?methods?getAuctionPrice()?call()
                return (price, gbl.web3?utils?fromWei(price, "ether"))
            }) () SetAuctionPrice
    match msg with
    | CounterMsg msg ->
        let (counter, counterCmd) = Counter.State.update msg model.Counter
        { model with Counter = counter }, Cmd.map CounterMsg counterCmd
    | HomeMsg (Home.Types.MintCubehead index) ->
        match model.accountData, model.Home.currentAuction with
        | Some { selectedAccount = account }, Some auction ->
            window.alert "click"
            let tokenData = Common.cubeheadsDataByIndex.[index].data
            let merklePath = Common.getMerklePath gbl.cubeheadsMerkleTree index
            window.alert (index.ToString() + "\n" + tokenData)
            window?merkle <- merklePath
            model, Cmd.OfPromise.either
                (fun _ -> promise {
                    let! result = gbl.window?cubemintingContract?methods?mint(index, tokenData, merklePath)?send(createObj ["from", !!account; "value", !!auction.priceRaw]) 
                    return result
                }) () (HomeMsg << Home.Types.MintSuccess) (HomeMsg << Home.Types.MintFail)
        | _ -> model, Cmd.none
    | HomeMsg (Home.Types.MintSuccess receipt) ->
        window?receipt <- receipt
        model, Cmd.none
    | HomeMsg (Home.Types.MintFail exn) ->
        window?exn <- exn
        model, Cmd.none
    | HomeMsg msg ->
        let (home, homeCmd) = Home.State.update msg model.Home
        { model with Home = home }, Cmd.map HomeMsg homeCmd
    | NavMsg Navbar.Types.Msg.ConnectWallet ->
        model, Cmd.OfPromise.perform gbl.web3Modal.connect () SetProvider
    | CubeballMsg msg ->
        let (cubeball, cubeballCmd) = Cubeball.State.update msg model.cubeball
        { model with cubeball = cubeball }, Cmd.map CubeballMsg cubeballCmd
    | GalleryMsg msg ->
        let (gallery, galleryCmd) = Gallery.State.update msg model.gallery
        { model with gallery = gallery }, Cmd.map GalleryMsg galleryCmd
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
            gbl.window?cubeheadsContract <- gbl.web3.NewContract(cubeheadsAbi, Config.network.cubeheadsContractAddress)
            gbl.window?cubemintingContract <- gbl.web3.NewContract(cubemintingAbi, gbl.window?cubemintingContract?_address)
            gbl.window?cubeletsContract <- gbl.web3.NewContract(cubeletsAbi, gbl.window?cubeletsContract?_address)
            { model with accountData = Some data },
                Cmd.OfAsync.perform (fun _ ->
                    async {
                        let! cubeheads = Cubehead.getCubeheadsForAccount gbl.window?cubeletsContract?_address data.selectedAccount
                        let results = Cubehead.getResultsForAccount data.selectedAccount
                        return { cubeheads = [||]; team = [||]; results = results } }) () SetWalletData
    | SetWalletData walletData ->
        { model with
            cubeball =
                { model.cubeball with
                    cubeheads = Some walletData.cubeheads
                    team = Some walletData.team
                    results = Some walletData.results} }, Cmd.none
    | SetWeb3InitData web3Data ->
        { model with Home = { model.Home with currentAuction = Some web3Data.auction } }, Cmd.none
    | SetAuctionPrice (priceRaw, price) ->
        match model.Home.currentAuction with
        | None -> model, Cmd.none
        | Some auction ->
            { model with Home = { model.Home with currentAuction = Some { auction with price = price; priceRaw = priceRaw } } }, Cmd.none
    | TimerTick ->
        match model.Home.currentAuction with
        | None -> model, Cmd.none
        | Some auction ->
            let cmd =
                match model.accountData with
                | None -> getPriceCmd
                | Some _ -> Cmd.batch [updateAccountCmd gbl.window?web3_; getPriceCmd]
            { model with Home = { model.Home with currentAuction = Some { auction with timeRemaining = auction.endTime - date.Now } } },
                cmd
    | _ -> model, Cmd.none
