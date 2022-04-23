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
    | Page.About -> Info.View.root
    | Counter -> Counter.View.root model.Counter (CounterMsg >> dispatch)
    | Home -> Home.View.root model.Home model.accountData (HomeMsg >> dispatch)
    | Cubeball -> Cubeball.View.root model.cubeball model.accountData (CubeballMsg >> dispatch)
    | Cubehead _ -> Cubehead.View.root model.cubehead (CubeheadMsg >> dispatch)
    | Gallery -> Gallery.View.root model.gallery model.accountData (GalleryMsg >> dispatch)
    | UserGuide -> UserGuide.View.root

  let containerCls page =
    match page with
    | Page.About -> ""
    | _ -> "container"

  div
    []
    [ Navbar.View.root model.accountData (NavMsg >> dispatch)
      div [Id "page-container"]
         [div [ClassName <| containerCls model.CurrentPage] [pageHtml model.CurrentPage ]
          footer [ClassName "footer"]
            [div [ClassName "container content"]
                [div [ClassName "level"] 
                    [div [ClassName "level-left"] [div [ClassName "level-item"] [img [ClassName "logo";Src "/img/logo.png"]]]
                     div [ClassName "level-right"] [div [ClassName "level-item social-icons"]
                        [img [ClassName "opensea"; Src "/img/opensea.svg"]
                         img [ClassName "discord"; Src "/img/discord.svg"]
                         img [ClassName "twitter"; Src "/img/twitter.svg"]
                         img [ClassName "instagram"; Src "/img/instagram.svg"]
                         img [ClassName "medium"; Src "/img/medium.svg"]]]]]]]]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

let gbl =
    let web3 = new Web3(new Web3WsProvider(Common.Config.network.infuraWsUrl))
    { web3Modal = web3Modal
      window = window
      web3 = web3
      contracts = None
      cubeheadsMerkleTree = Common.buildCubeheadsMerkleTree web3 }

window?path <- Common.getMerklePath gbl.cubeheadsMerkleTree 888

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
