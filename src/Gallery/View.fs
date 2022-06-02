module Gallery.View

open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Types
open Global
open Common
open Imports.InfiniteScroll

let filters =
    traits
        |> Map.toList
        |> List.map (fun (trait, values) -> trait, values |> Map.toList |> List.map (fun (v, _) -> trait, v))

let filter selectedTrait traitName traitOptions dispatch =
    let selectedValue =
        match selectedTrait with
        | None -> -1
        | Some trait ->
            filters |> List.find (fun (n, _) -> n = traitName) |> snd |> List.findIndex (fun t -> t = trait)
    div [ClassName "field"]
        [label [ClassName "label"] [str traitName]
         div [ClassName "control"]
            [div [ClassName "select is-fullwidth"] [
                select [Value selectedValue; OnChange (fun e ->
                    let trait = 
                        match e.target?value with
                        | -1 -> None
                        | i ->
                            filters |> List.find (fun (n, _) -> n = traitName) |> snd |> List.item i |> Some
                    dispatch (SelectTrait (traitName, trait)))]
                    [option [Value -1] []
                     ofList <| (traitOptions |> List.mapi (fun i opt ->
                        option [Value i] [str <| Blockhead.getTraitName opt]))]]]]

let search model dispatch =
    div [ClassName "field"]
        [label [ClassName "label"] [str "ID"]
         div [ClassName "control"]
            [input [Value model.idSearch; OnChange(fun e -> dispatch <| SetSearch e.Value); ClassName "input"; Type "text"; Placeholder "#ID"]]]

let reset dispatch =
    div [ClassName "field"]
        [div [ClassName "control reset"]
            [button [OnClick (fun _ -> dispatch ResetFilters); ClassName "button is-primary is-fullwidth"] [str "Reset Filters"]]]

let root model accountData dispatch =
    infiniteScroll [LoadMore (fun _ -> dispatch LoadPage); HasMore (Some true)]
        [div [ClassName "columns"]
            [div [ClassName "column is-3"]
                [section [ClassName "box section gallery-filter"]
                    [ofList <| (filters |> List.map (fun (name, options) -> filter (model.filter |> Map.tryFind name) name options dispatch))
                     search model dispatch
                     reset dispatch]]
             div [ClassName "column is-9"]
                [section [ClassName "section"] [Elmish.React.Common.lazyView2With refEquals (fun model dispatch ->
                    model.blockheads |> List.map (fun blockheads ->
                        let elements = blockheads |> List.map (fun c -> ViewComponents.blockheadsCompact c dispatch)
                        div [ClassName "gallery-page"] [ViewComponents.blockheadsGrid elements]) |> ofList) model dispatch]]]]
