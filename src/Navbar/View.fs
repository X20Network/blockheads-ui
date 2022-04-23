module Navbar.View

open Fable.React
open Fable.React.Props
open Types
open Global
open Common

let navButton name =
    a [ClassName "navbar-item"] [str name]

let navButtons =
    []

let connectButton dispatch =
    div [ClassName "navbar-item"]
        [div [ClassName "buttons"]
            [a [ClassName "button is-primary"; OnClick <| fun _ -> dispatch ConnectWallet]
                [str "Connect Wallet"]]]

let connectedAccount (selectedAccount :string) =
    div [ClassName "navbar-item"]
        [span [ClassName "tag is-info is-medium"]
            [str (selectedAccount.Substring(0, 4).ToUpper() + "..." + selectedAccount.Substring(12, 4).ToUpper())]]

let wrongChain =
    div [ClassName "navbar-item"]
        [span [ClassName "tag is-danger is-medium"]
            [span [ClassName "icon"] [i [ClassName "mdi mdi-alert-outline"] []]; span [] [str <| "Connect to " + Common.Config.network.chainName]]]

let root model navbarMenuActive dispatch =
    nav
        [ ClassName "navbar is-dark is-fixed-top" ]
        [ div
            [ ClassName "container" ]
            [ div
                [ ClassName "navbar-brand" ]
                [ a
                    [ ClassName "navbar-item is-1 brand"; Href (toHash Home) ]
                    [ str "CUBEHEADS" ]
                  div [ClassName "is-hidden-desktop"]
                    [match model with
                     | None -> connectButton dispatch
                     | Some { chainId = chainId } when chainId <> Config.network.chainId -> wrongChain
                     | Some accountData -> connectedAccount accountData.selectedAccount]
                  a
                    [ classBaseList "navbar-burger" ["is-active", navbarMenuActive]; AriaLabel "Menu"; Role "button"; AriaExpanded false; HTMLAttr.Custom("data-target", "navbarMenu"); OnClick (fun _ -> dispatch ToggleNavbarMenu) ]
                    [ span [AriaHidden true] []
                      span [AriaHidden true] []
                      span [AriaHidden true] [] ]]
              div
                [ Id "navbarMenu"; classBaseList "navbar-menu" ["is-active", navbarMenuActive] ]
                [ div [ClassName "navbar-start"]
                    []
                  div [ClassName "navbar-end"]
                    [a [ClassName "navbar-item"; Href (toHash About)]
                        [span [ClassName "icon has-text-primary"]
                            [i [ClassName "mdi mdi-cube"] []]
                         span [] [str "About"]]
                     a [ClassName "navbar-item"; Href <| toHash Gallery]
                        [span [ClassName "icon has-text-success"]
                            [i [ClassName "mdi mdi-view-grid-outline"] []]
                         span [] [str "Gallery"]]
                     a [ClassName "navbar-item"; Href <| toHash UserGuide]
                        [span [ClassName "icon has-text-warning"]
                            [i [ClassName "mdi mdi-newspaper-variant"] []]
                         span [] [str "User Guide"]]
                     a [ClassName "navbar-item"]
                        [span [ClassName "icon has-text-pink"]
                            [i [ClassName "mdi mdi-note-text-outline"] []]
                         span [] [str "Whitepaper"]]
                     a [ClassName "navbar-item"; Href (toHash Cubeball)]
                        [span [ClassName "icon has-text-info"]
                            [i [ClassName "mdi mdi-soccer"] []]
                         span [] [b [] [str "Cubeball"]]]
                     div [ClassName "is-hidden-mobile"] [
                         match model with
                         | None -> connectButton dispatch
                         | Some { chainId = chainId } when chainId <> Common.Config.network.chainId -> wrongChain
                         | Some accountData -> connectedAccount accountData.selectedAccount ] ] ] ] ]
