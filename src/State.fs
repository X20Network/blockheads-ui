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

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map About (s "about")
        map Counter (s "counter")
        map Home (s "home")
        map Cubeball (s "cubeball")
        map Gallery (s "gallery")
        map UserGuide (s "guide")
        map Cubehead (str)
    ]

let urlUpdate (result : Page option) model =
    match result with
    | None ->
        console.error("Error parsing url")
        model, Navigation.modifyUrl (toHash model.CurrentPage)
    | Some (Cubehead name) ->
        let cubehead = Cubehead.getCubeheadByName name
        let prevPage =
            match model.CurrentPage with
            | Home -> None
            | page -> Some page
        { model with
            CurrentPage = Cubehead name
            cubehead = Some { previousPage = prevPage; cubehead = cubehead } }, []
    | Some page ->
        { model with CurrentPage = page }, []

let init result =
    let (counter, counterCmd) = Counter.State.init()
    let (home, homeCmd) = Home.State.init()
    let (model, cmd) =
        urlUpdate result
          { CurrentPage = Home
            Counter = counter
            Home = home
            accountData = None
            cubehead = None
            gallery = { filter = Map.empty; idSearch = "" }
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
                       Cmd.map HomeMsg homeCmd ]

let update gbl msg model =
    match msg with
    | CounterMsg msg ->
        let (counter, counterCmd) = Counter.State.update msg model.Counter
        { model with Counter = counter }, Cmd.map CounterMsg counterCmd
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
        model, Cmd.OfPromise.perform
            (fun _ -> promise {
                let! chainId = web3?eth?getChainId()
                let! accounts = web3?eth?getAccounts()
                let selectedAccount = accounts?at $ 0
                gbl.window?console?log $ (chainId)
                gbl.window?console?log $ (selectedAccount)
                return { selectedAccount = selectedAccount }
            }) () SetAccountData
    | SetAccountData data ->
        { model with accountData = Some data },
            Cmd.ofSub (fun dispatch -> JS.setTimeout(fun _ ->
                let cubeheads, team = Cubehead.getCubeheadsForAccount data.selectedAccount
                let results = Cubehead.getResultsForAccount data.selectedAccount
                dispatch <| SetWalletData { cubeheads = cubeheads; team = team; results = results }) 4000 |> ignore)
    | SetWalletData walletData ->
        { model with
            cubeball =
                { model.cubeball with
                    cubeheads = Some walletData.cubeheads
                    team = Some walletData.team
                    results = Some walletData.results} }, Cmd.none
    | TimerTick ->
        match model.Home.currentAuction with
        | None -> model, Cmd.none
        | Some auction ->
            { model with Home = { model.Home with currentAuction = Some { auction with timeRemaining = auction.endTime - date.Now } } }, Cmd.none
    | _ -> model, Cmd.none
