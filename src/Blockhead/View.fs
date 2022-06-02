module Blockhead.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Types
open Common
open Global

let root (model :Model option) dispatch =
    match model with
    | None -> div [] [str "Blockhead not found"]
    | Some pageModel ->
        match pageModel.blockhead with
        | None ->
            div [ClassName "notification has-text-centered mt8"]
                [div [ClassName "lds-dual-ring"] []]
        | Some blockhead ->
            let image = """data:image/svg+xml, """ + JS.encodeURIComponent blockhead.svg
            let image = blockhead.svg
            let breadCrumb =
                match pageModel.previousPage with
                | None -> ofOption None
                | Some prevPage ->
                   let text =
                       match prevPage with
                       | Page.Blockball -> "Back to Blockheads"
                       | Page.Gallery -> "Back to Gallery"
                   section [ClassName "back-breadcrumb"]
                        [a [Href <| toHash prevPage]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-chevron-left"] []]
                             span [] [b [] [str text]]]]
            div []
                [
                 yield
                     section [ClassName "section"] [
                        div [ClassName "box blockhead"]
                            [breadCrumb
                             div [ClassName "has-text-centered"] [img [ClassName "mb4"; Src image]]
                             ViewComponents.blockheadDetail blockhead]]]
