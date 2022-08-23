module App.View

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Types
open App.State
open Global
open Imports

importAll "../sass/main.sass"

open Fable.React
open Fable.React.Props

let [<Global("window")>] window: obj = jsNative

let web3ModalConfig =
    keyValueList CaseRules.LowerFirst
        [!!("network", "mainnet")
         !!("cacheProvider", true)
         !!("providerOptions", keyValueList CaseRules.LowerFirst [])]

let web3Modal = new Web3Modal(web3ModalConfig)

let menuItem label page currentPage =
    li
      [ ]
      [ a
          [ classList [ "is-active", page = currentPage ]
            Href (toHash page) ]
          [ str label ] ]

let menu currentPage =
  aside
    [ ClassName "menu" ]
    [ p
        [ ClassName "menu-label" ]
        [ str "General" ]
      ul
        [ ClassName "menu-list" ]
        [ menuItem "Home" Home currentPage
          menuItem "Counter sample" Counter currentPage
          menuItem "About" Page.About currentPage ] ]

let root model dispatch =

  let pageHtml page =
    match page with
    | Page.About -> About.View.root
    | Counter -> Counter.View.root model.Counter (CounterMsg >> dispatch)
    | Home -> Home.View.root model.timeToLaunch model.carousel (HomeMsg >> dispatch)
    | Blockball -> Blockball.View.root model.blockball model.accountData (BlockballMsg >> dispatch)
    | Blockhead _ -> Blockhead.View.root model.blockhead (BlockheadMsg >> dispatch)
    | Gallery -> Gallery.View.root model.gallery model.accountData (GalleryMsg >> dispatch)
    | UserGuide -> UserGuide.View.root

  let containerCls page =
    match page with
    | Page.About -> ""
    | Page.Home -> ""
    | _ -> "container"

  div
    []
    [ Navbar.View.root model.accountData model.navbarMenuActive (NavMsg >> dispatch)
      div [Id "page-container"]
         [div [ClassName <| containerCls model.CurrentPage] [pageHtml model.CurrentPage ]
          footer [ClassName "footer"]
            [div [ClassName "has-text-centered"]
                [//a [Href ""; ClassName "opensea"; Target "_blank"] []
                 a [Href "https://discord.gg/cpShffArsz"; ClassName "discord"; Target "_blank"] []
                 a [Href "https://twitter.com/StillBlockheads"; ClassName "twitter"; Target "_blank"] []
                 a [Href "https://www.instagram.com/theblockheadsnetwork/"; ClassName "instagram"; Target "_blank"] []
                 a [Href "https://medium.com/@blockheadsnft"; ClassName "medium"; Target "_blank"] []]]]]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

let gbl =
    let web3 = new Web3(new Web3WsProvider(Common.Config.network.infuraWsUrl))
    { web3Modal = web3Modal
      window = window
      web3 = web3
      contracts = None
      blockheadsMerkleTree = Common.buildBlockheadsMerkleTree web3 }

window?path <- Common.getMerklePath gbl.blockheadsMerkleTree 888

let timer _ =
    Cmd.ofSub (fun dispatch ->
        JS.setInterval (fun () -> dispatch TimerTick) 1000 |> ignore)

// App
Program.mkProgram (init gbl) (update gbl) root
|> Program.withSubscription timer
|> Program.toNavigable (parsePath pageParser) (urlUpdate gbl)
#if DEBUG
|> Program.withDebugger
#endif
|> Program.withReactBatched "elmish-app"
|> Program.run
