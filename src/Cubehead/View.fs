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
    | Some cubehead ->
        let image = """data:image/svg+xml, """ + JS.encodeURIComponent cubehead.cubehead.svg
        let imageIndex = cubehead.cubehead.index % 305
        let image = "/img/cubeheads/cubehead (" + imageIndex.ToString() + ").png"
        let breadCrumb =
            match cubehead.previousPage with
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
                         ViewComponents.cubeheadDetail cubehead.cubehead]]]
