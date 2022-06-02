module Blockball.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Elmish
open Types
open Common

let connectNotification =
    div [ClassName "notification"]
        [span [ClassName "icon"] [i [ClassName "mdi mdi-alert-circle"] []]
         span [] [str "Connect your wallet"]]

let loadingNotification =
    div [ClassName "notification has-text-centered"]
        [div [ClassName "lds-dual-ring"] []]

let noBlockheadsNotification =
    div [ClassName "notification has-text-centered"]
        [h2 [] [b [] [str "Your wallet contains no Blockheads"]]]

let noResultsNotification =
    div [ClassName "notification has-text-centered"]
        [h2 [] [b [] [str "No games have been played yet. Check back soon"]]]

let notEnoughBlockheadsNotification =
    div [ClassName "notification has-text-centered"]
        [h2 [] [b [] [str "You need at least four Blockheads or Blocklets to form a team, including at least one Blockhead"]]]

let signMsgNotification dispatch =
    div [ClassName "notification"]
        [h2 [] [b [] [str "You need to prove your account ownership before we can fetch your team. Click the button below to sign a message that will prove ownership."]]
         div [ClassName "has-text-centered pt4"]
            [button [ClassName "button"; OnClick (fun _ -> dispatch SignMessage)]
                [span [ClassName "icon"] [i [ClassName "mdi mdi-file-sign"] []]; b [] [str "Sign Message"]]]]

let teamPositions position =
    let getCls pos = if pos = position then "team-position active" else "team-position"
    table [ClassName "team-diagram"]
        [tr [] [td [ColSpan 3; ClassName "is-size-7"] [i [] [b [] [str "Attacking"]]]]
         tr []
            [td [] []
             td [] [div [ClassName <| getCls 1] []]
             td [] []]
         tr []
             [td [] [div [ClassName <| getCls 2] []]
              td [] []
              td [] [div [ClassName <| getCls 3] []]]
         tr []
            [td [] []
             td [] [div [ClassName <| getCls 4] []]
             td [] []]
         tr [] [td [ColSpan 3; ClassName "is-size-7"] [i [] [b [] [str "Defending"]]]]]

let team model dispatch =
    let getBlockheadByIndex i =
        (model.blockheads.Value |> Array.find (fun c -> c.blockhead.index = i)).blockhead
    let nteam =
        match model.team with
        | None | Some [||] ->
            let dt = model.draftTeam |> Array.choose id
            if dt.Length = 4 && dt |> Array.filter snd |> Array.length = 2 then Some dt else None
        | Some team ->
            let dt = Array.zip team model.draftTeam |> Array.map (function | (_, Some (i, c)) -> i, c | (i, c), _ -> i, c)
            if dt |> Array.filter snd |> Array.length = 2 then Some dt else None
    let saveRevertBtn =
        let btns =
            div [ClassName "buttons"]
                [button [OnClick (fun _ -> dispatch SaveTeamChanges);ClassName "button is-primary"]
                    [span [ClassName "icon"] [i [ClassName "mdi mdi-check-circle-outline mdi-24px"] []]
                     span [] [b [] [str "Save"]]]
                 button [OnClick (fun _ -> dispatch CancelTeamChanges); ClassName "button is-light"]
                    [span [ClassName "icon"] [i [ClassName "mdi mdi-cancel mdi-24px"] []]
                     span [] [b [] [str "Cancel Changes"]]]]
        match model.team, nteam with
        | _, None -> ofOption None
        | Some team, Some nteam when team = nteam -> ofOption None
        | _, Some _ -> btns
    let rowContent n =
        let btn =
            let txt =
                match model.team, model.draftTeam.[n] with
                | Some [||], None -> "Select Blockhead"
                | _, Some _ -> "Change Blockhead"
                | Some _, None -> "Change Blockhead"
            button [Disabled model.saving; OnClick (fun _ -> dispatch <| SelectingBlockhead n); ClassName "button is-primary is-rounded is-normal is-responsive"]
                [span [] [b [] [str txt]]
                 span [ClassName "icon"] [i [ClassName "mdi mdi-arrow-right-bold-hexagon-outline mdi-24px"] []]]
        let title =
            match model.team, model.draftTeam.[n] with
            | Some [||], None ->
                h1 [ClassName "has-text-grey-light"] [i [] [str "No Blockhead Selected"]]
            | _, Some (ci, _) ->
                let blockhead = getBlockheadByIndex ci
                h1 [] [str blockhead.name]
            | Some t, None ->
                let blockhead = getBlockheadByIndex (t.[n] |> fst)
                h1 [] [str blockhead.name]
        let parentSelect =
            let ps checked disableUnchecked =
                div [ClassName "parent-select"]
                    [label [ClassName <| if ((not checked) && disableUnchecked) then "disabled" else ""]
                        [input [Disabled ((not checked) && disableUnchecked);Type "checkbox"; Checked checked; OnChange (fun _ -> dispatch <| SetCaptainState (n, not checked))]
                         span [ClassName "icon"] [i [ClassName "mdi mdi-check-bold"] []]]
                     span [] [b [] [str "Parent"]]]
            match model.team, model.draftTeam.[n] with
            | Some [||], None -> ofOption None
            | _, Some (_, checked) ->
                let disableUnchecked = match nteam with | Some _ -> true | _ -> false
                ps checked disableUnchecked
            | Some t, None ->
                let disableUnchecked = match nteam with | Some _ -> true | _ -> false
                ps (t.[n] |> snd) disableUnchecked
        let img =
            match model.team, model.draftTeam.[n] with
            | Some [||], None ->
                span [ClassName "icon blockhead-not-selected"] [i [ClassName "mdi mdi-block-off-outline"] []]
            | _, Some (ci, _) ->
                let blockhead = getBlockheadByIndex ci
                let image = blockhead.svg
                img [Src (image)]
            | Some t, None ->
                let blockhead = getBlockheadByIndex (t.[n] |> fst)
                let image = blockhead.svg
                img [Src (image)]
        match model.team with
        | Some _ ->
            article [ClassName "media"]
                [figure [ClassName "media-left"] [img]
                 div [ClassName "media-content"]
                    [div [ClassName "content"] [title]
                     btn
                     parentSelect]]
            
    let row i =
        tr []
            [td [] [teamPositions (i + 1)]
             td []
                [rowContent i]]
    section [ClassName "section box"]
        [saveRevertBtn
         table [ClassName "table is-fullwidth team-table"]
            [thead []
                [tr []
                    [th [] [str "Position"]
                     th [] [str "Blockhead"]]]
             tbody [] (List.init 4 row)]]

let results model accountData dispatch =
    match accountData with
    | None -> connectNotification
    | Some _ ->
        match model.results with
        | None -> loadingNotification
        | Some _ when model.team = Some [||] -> notEnoughBlockheadsNotification
        | Some [] ->
            noResultsNotification
        | Some results ->
            results |> List.map (fun result ->
                let resultTxt, resultCls =
                    match result.score, result.userTeamColour with
                    | (a, b), Blue when a > b -> "WIN", "has-text-success"
                    | (a, b), Blue when a < b -> "LOSE", "has-text-danger"
                    | (a, b), Red when b > a -> "WIN", "has-text-success"
                    | (a, b), Red when b < a -> "LOSE", "has-text-danger"
                    | _ -> "DRAW", "has-text-dark"
                let dateTxt = result.date.ToLongDateString()
                let redColour = Blockhead.getTrophyColour result.trophyType Red
                let blueColour = Blockhead.getTrophyColour result.trophyType Blue
                let redTeam = if result.userTeamColour = Red then result.userTeam else result.oppTeam
                let blueTeam = if result.userTeamColour = Blue then result.userTeam else result.oppTeam
                let redteamName = if result.userTeamColour = Red then "YOUR TEAM" else "OPPOSITION TEAM"
                let blueTeamName = if result.userTeamColour = Blue then "YOUR TEAM" else "OPPOSITION TEAM"
                let child =
                    match resultTxt with
                    | "WIN" ->
                       [div [ClassName "child-block"]
                           [div [ClassName "has-text-centered"] [span [ClassName "icon"] [i [ClassName "mdi mdi-arrow-down-bold-hexagon-outline"] []]]
                            ViewComponents.blockheadsMini result.childBlock dispatch]]
                            |> ofList
                    | _ -> ofOption None
                div [ClassName "box result"]
                    [h1 [ClassName resultCls] [b [] [str resultTxt]]
                     h2 [] [b [] [str dateTxt]]
                     div [ClassName "columns"]
                        [div [ClassName "column is-4"]
                            [div [ClassName "has-text-centered mb4"] [b [Style [BackgroundColor blueColour; BorderRadius "0.5rem"; Padding "0 0.5rem"; Color "white"]] [str blueTeamName]]
                             div [ClassName "result-team"] (blueTeam |> Array.map (fun c -> ViewComponents.blockheadsMini c dispatch) |> Array.toList)
                             ]
                         div [ClassName "column is-4 has-text-centered"]
                            [div [ClassName "game"]
                                [img [ClassName (match resultTxt with | "WIN" -> "win" | _ -> ""); Src result.trophySrc]]
                             div [ClassName "score"]
                                [div [] [span [Style [BackgroundColor blueColour]] [str <| (fst result.score).ToString()]; span [Style [BackgroundColor redColour]] [str <| (snd result.score).ToString()]]]
                             child]
                         div [ClassName "column is-4"]
                            [div [ClassName "has-text-centered mb4"] [b [Style [BackgroundColor redColour; BorderRadius "0.5rem"; Padding "0 0.5rem"; Color "white"]] [str redteamName]]
                             div [ClassName "result-team"] (redTeam |> Array.map (fun c -> ViewComponents.blockheadsMini c dispatch) |> Array.toList)]]
                     ]) |> ofList

let blockheadsTab model accountData dispatch =
    match accountData with
    | None -> connectNotification
    | Some _ ->
        match model.blockheads with
        | None -> loadingNotification
        | Some [||] -> noBlockheadsNotification
        | Some blockheads ->
            Elmish.React.Common.lazyView2With refEquals (fun blockheads dispatch ->
                let elements = blockheads |> Array.map (fun wc -> ViewComponents.blockheadsCompact wc.blockhead dispatch) |> Array.toList
                ViewComponents.blockheadsGrid elements) blockheads dispatch

let teamTab model accountData dispatch =
    let savingTxt =
        match model.saving with
        | true ->
            div [ClassName "notification has-text-centered"]
                [div [ClassName "lds-dual-ring"] []
                 div [ClassName "is-size-4 has-text-white"] [b [] [str "Saving team to blockchain"]]]
        | false -> ofOption None
    match accountData with
    | None -> connectNotification
    | Some { signedMessage = None } ->
        signMsgNotification dispatch
    | Some _ ->
        match model.team with
        | None -> loadingNotification
        | Some _ ->
            match model.blockheads with
            | None -> loadingNotification
            | Some [||] -> noBlockheadsNotification
            | Some x when x.Length < 4 -> notEnoughBlockheadsNotification
            | Some _ ->
                div [] 
                    [savingTxt
                     team model dispatch]
            

let resultsTab model accountData dispatch =
    match accountData with
    | None -> connectNotification
    | Some _ ->
        results model accountData dispatch

let selectBlockhead n model accountData dispatch =
    match accountData with
    | None -> failwith "ni"
    | Some _ ->
        match model.blockheads with
        | None -> failwith "ni"
        | Some [||] -> failwith "ni"
        | Some blockheads ->
            let alreadyIncluded =
                match model.team with
                | None | Some [||] -> model.draftTeam |> Array.choose id |> Array.map fst
                | Some team -> Array.zip team model.draftTeam |> Array.map (fun (t, dt) -> match dt with Some (i, _) -> i | _ -> fst t)
            div [] 
               [section [ClassName "section"]
                    [div [ClassName "box"]
                       [section [ClassName "back-breadcrumb mb8"]
                            [a [OnClick (fun _ -> dispatch CancelSelectBlockhead)]
                                [span [ClassName "icon"] [i [ClassName "mdi mdi-chevron-left"] []]
                                 span [] [b [] [str "Back to Team"]]]
                             span [Style [MarginLeft "1rem"]; ClassName "icon"] [i [ClassName "mdi mdi-circle-medium"] []]
                             span [Style [MarginLeft "1rem"]] [b [] [str <| "Select for Pos " + (n + 1).ToString()]]]
                        div [] (blockheads
                            |> Array.toList
                            |> List.filter (fun wc-> not (alreadyIncluded |> Array.contains (wc.blockhead.index)))
                            |> List.map (fun wc ->
                        let image = wc.blockhead.svg
                        article [ClassName "media"]
                            [figure [ClassName "media-left"]
                                [img [Style [Height "12rem"; Width "12rem"]; Src (image)]]
                             div [ClassName "media-content"]
                                [div [ClassName "content"]
                                    [div [ClassName "mb4"] [ViewComponents.blockheadDetail wc.blockhead]
                                     nav [ClassName "level"]
                                         [div [ClassName "level-left"] []
                                          div [ClassName "level-right"]
                                             [div [ClassName "level-item"]
                                                [button
                                                    [Disabled model.saving
                                                     OnClick (fun _ -> dispatch <| SelectBlockhead (n, wc.blockhead.index)); ClassName "button is-primary"]
                                                        [b [] [str <| "Select"]]]]]]]]))]]]

let root model accountData dispatch =
    match model.selectingBlockhead with
    | None ->
        div []
            [div [ClassName "tabs is-centered is-toggle is-toggle-rounded blockball-tabs is-hidden-desktop is-small"]
                [ul []
                    [li [ClassName (if model.activeTab = AllBlockheads then "is-active" else "")]
                        [a  [OnClick (fun _ -> dispatch <| SelectTab AllBlockheads)]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-cube-scan mdi-24px"] []]
                             ]]
                     li [ClassName (if model.activeTab = Team then "is-active" else "")]
                        [a [OnClick (fun _ -> dispatch <| SelectTab Team)]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-soccer-field mdi-24px"] []]
                             ]]
                     li [ClassName (if model.activeTab = Results then "is-active" else "")]
                        [a [OnClick (fun _ -> dispatch <| SelectTab Results)]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-trophy mdi-24px"] []]
                             ]]]]
             div [ClassName "tabs is-centered is-toggle is-toggle-rounded blockball-tabs is-hidden-touch"] 
                [ul []
                    [li [ClassName (if model.activeTab = AllBlockheads then "is-active" else "")]
                        [a  [OnClick (fun _ -> dispatch <| SelectTab AllBlockheads)]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-cube-scan mdi-24px"] []]
                             span [] [str "Blockheads"]]]
                     li [ClassName (if model.activeTab = Team then "is-active" else "")]
                        [a [OnClick (fun _ -> dispatch <| SelectTab Team)]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-soccer-field mdi-24px"] []]
                             span [] [str "Team"]]]
                     li [ClassName (if model.activeTab = Results then "is-active" else "")]
                        [a [OnClick (fun _ -> dispatch <| SelectTab Results)]
                            [span [ClassName "icon"] [i [ClassName "mdi mdi-trophy mdi-24px"] []]
                             span [] [str "Results"]]]]]
             match model.activeTab with
             | AllBlockheads -> blockheadsTab model accountData dispatch
             | Team -> teamTab model accountData dispatch
             | Results -> resultsTab model accountData dispatch]
    | Some n ->
        selectBlockhead n model accountData dispatch
