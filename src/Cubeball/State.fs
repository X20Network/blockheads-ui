module Cubeball.State

open Elmish
open Types
open Fable.Core

let update msg model : Model * Cmd<Msg> =
    match msg with
    | SelectTab tab ->
        { model with activeTab = tab }, Cmd.none
    | SelectingCubehead n ->
        { model with selectingCubehead = Some n }, Cmd.none
    | CancelSelectCubehead ->
        { model with selectingCubehead = None }, Cmd.none
    | SelectCubehead (pos, cubeheadIndex) ->
        let draftTeam = model.draftTeam |> Array.mapi (fun i t -> if i = pos then Some (cubeheadIndex, false) else t)
        { model with draftTeam = draftTeam; selectingCubehead = None }, Cmd.none
    | CancelTeamChanges ->
        { model with draftTeam = [| None; None; None; None |] }, Cmd.none
    | SaveTeamChanges ->
        { model with saving = true },
            Cmd.ofSub (fun dispatch -> JS.setTimeout(fun _ -> dispatch SaveSucceeded) 4000 |> ignore)
    | SaveSucceeded ->
        { model with
            saving = false
            team =
                match model.team with
                | None | Some [||] -> model.draftTeam |> Array.choose id |> Some
                | Some team -> Array.zip team model.draftTeam |> Array.map (function | (_, Some (i, c)) -> (i, c) | c, _ -> c) |> Some
            draftTeam = [| None; None; None; None |]}, Cmd.none
    | SetCaptainState (pos, captain) ->
        match model.team, model.draftTeam.[pos] with
        | _, Some _ ->
            let draftTeam = model.draftTeam |> Array.mapi (fun i t -> if i = pos then Some (fst t.Value, captain) else t)
            { model with draftTeam = draftTeam }, Cmd.none
        | Some team, None ->
            { model with draftTeam = model.draftTeam |> Array.mapi (fun i t -> if i = pos then Some (fst team.[pos], captain) else t) }, Cmd.none
