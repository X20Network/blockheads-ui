module Navbar.View

open Fable.React
open Fable.React.Props
open Types
open Global

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

let root model dispatch =
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
                     | Some selectedAccount -> connectedAccount selectedAccount]
                  a
                    [ ClassName "navbar-burger"; AriaLabel "Menu"; Role "button"; AriaExpanded false; HTMLAttr.Custom("data-target", "navbarMenu") ]
                    [ span [AriaHidden true] []
                      span [AriaHidden true] []
                      span [AriaHidden true] [] ]]
              div
                [ Id "navbarMenu"; ClassName "navbar-menu" ]
                [ div [ClassName "navbar-start"]
                    []
                  div [ClassName "navbar-end"]
                    [a [ClassName "navbar-item"; Href (toHash About)]
                        [span [ClassName "icon"]
                            [i [ClassName "mdi mdi-cube"] []]
                         span [] [str "About"]]
                     a [ClassName "navbar-item"; Href <| toHash Gallery]
                        [span [ClassName "icon"]
                            [i [ClassName "mdi mdi-view-grid-outline"] []]
                         span [] [str "Gallery"]]
                     a [ClassName "navbar-item"]
                        [span [ClassName "icon"]
                            [i [ClassName "mdi mdi-text-box"] []]
                         span [] [str "Whitepaper"]]
                     a [ClassName "navbar-item"; Href (toHash Cubeball)]
                        [span [ClassName "icon"]
                            [i [ClassName "mdi mdi-soccer"] []]
                         span [] [b [] [str "Cubeball"]]]
                     match model with
                     | None -> connectButton dispatch
                     | Some selectedAccount -> connectedAccount selectedAccount] ] ] ]
