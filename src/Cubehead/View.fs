module Cubehead.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Types
open Common
open Global

let root (model :Model option) dispatch =
    match model with
    | None -> div [] [str "Cubehead not found"]
    | Some pageModel ->
        match pageModel.cubehead with
        | None ->
            div [ClassName "notification has-text-centered mt8"]
                [div [ClassName "lds-dual-ring"] []]
        | Some cubehead ->
            let image = """data:image/svg+xml, """ + JS.encodeURIComponent cubehead.svg
            let image = cubehead.svg
            let breadCrumb =
                match pageModel.previousPage with
                | None -> ofOption None
                | Some prevPage ->
                   let text =
                       match prevPage with
                       | Page.Cubeball -> "Back to Cubeheads"
                       | Page.Gallery -> "Back to Gallery"
                   section [ClassName "back-breadcrumb"]
                        [a [Href <| toHash prevPage]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-chevron-left"] []]
                             span [] [b [] [str text]]]]
            div []
                [
                 yield
                     section [ClassName "section"] [
                        div [ClassName "box cubehead"]
                            [breadCrumb
                             div [ClassName "has-text-centered"] [img [ClassName "mb4"; Src image]]
                             ViewComponents.cubeheadDetail cubehead]]]
