module Gallery.State

open Elmish
open Types
open Fable.Core
open Common

let update msg model : Model * Cmd<Msg> =
    let apply model =
        let cs, fcs =
            let sindex =
                match System.Int32.TryParse model.idSearch with
                | true, i -> Some i
                | _ -> None
            match Blockhead.getAllBlockheadsPaged (model.filter |> Map.toList |> List.map snd) sindex with
            | [] -> [], []
            | (cs1::cs) ->
                [cs1], cs
        { model with blockheads = cs; filteredBlockheads = fcs }
    match msg with
    | SetSearch search ->
        apply { model with idSearch = search }, Cmd.none
    | SelectTrait (name, Some trait) ->
        apply { model with filter = model.filter |> Map.add name trait }, Cmd.none
    | SelectTrait (name, None) ->
        apply { model with filter = model.filter |> Map.remove name }, Cmd.none
    | LoadPage ->
        match model.filteredBlockheads with
        | [] -> model, Cmd.none
        | c::cs -> { model with blockheads = model.blockheads @ [c]; filteredBlockheads = cs }, Cmd.none
