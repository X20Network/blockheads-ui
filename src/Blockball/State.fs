module Blockball.State

open Elmish
open Types
open Fable.Core
open Common

let update msg model : Model * Cmd<Msg> =
    match msg with
    | SelectTab tab ->
        { model with activeTab = tab }, Cmd.none
    | SelectingBlockhead n ->
        { model with selectingBlockhead = Some n }, Cmd.none
    | CancelSelectBlockhead ->
        { model with selectingBlockhead = None }, Cmd.none
    | SelectBlockhead (pos, blockheadIndex) ->
        let draftTeam = model.draftTeam |> Array.mapi (fun i t -> if i = pos then Some (blockheadIndex, false) else t)
        { model with draftTeam = draftTeam; selectingBlockhead = None }, Cmd.none
    | CancelTeamChanges ->
        { model with draftTeam = [| None; None; None; None |] }, Cmd.none
    | SaveSucceeded ->
        { model with
            saving = false
            team =
                match model.team with
                | None | Some [||] -> model.draftTeam |> Array.choose id |> Some
                | Some team -> Array.zip team model.draftTeam |> Array.map (function | (_, Some (i, c)) -> (i, c) | c, _ -> c) |> Some
            draftTeam = [| None; None; None; None |]}, Cmd.none
    | CommitTeamFailed e ->
        Browser.Dom.console.log("error committing team: " + e.Message)
        { model with saving = false }, Cmd.none
    | SetCaptainState (pos, captain) ->
        match model.team, model.draftTeam.[pos] with
        | _, Some _ ->
            let draftTeam = model.draftTeam |> Array.mapi (fun i t -> if i = pos then Some (fst t.Value, captain) else t)
            { model with draftTeam = draftTeam }, Cmd.none
        | Some team, None ->
            { model with draftTeam = model.draftTeam |> Array.mapi (fun i t -> if i = pos then Some (fst team.[pos], captain) else t) }, Cmd.none
