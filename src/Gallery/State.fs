module Gallery.State

open Elmish
open Types
open Fable.Core

let update msg model : Model * Cmd<Msg> =
    match msg with
    | SetSearch search ->
        { model with idSearch = search }, Cmd.none
    | SelectTrait (name, Some trait) ->
        { model with filter = model.filter |> Map.add name trait }, Cmd.none
    | SelectTrait (name, None) ->
        { model with filter = model.filter |> Map.remove name }, Cmd.none
