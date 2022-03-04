module Gallery.View

open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Types
open Global
open Common

let filters =
    ["Cube Type",
        [CubeType CubeType.BlueWithRedFace
         CubeType CubeType.GreyWithOrangeFace
         CubeType CubeType.Camo
         CubeType CubeType.Psychedelic
         CubeType CubeType.GradientGrey
         CubeType CubeType.GradientGold
         CubeType CubeType.Wireframe
         CubeType CubeType.Zombie
         CubeType CubeType.Alien
         CubeType CubeType.Steampunk]
     "Side Detail",
        [SideDetail SideDetail.Titanium
         SideDetail SideDetail.Silicon
         SideDetail SideDetail.Aluminium
         SideDetail SideDetail.CircuitBoard
         SideDetail SideDetail.Headphones]
     "Eyes",
        [Eyes Eyes.RedEye
         Eyes Eyes.Normal
         Eyes Eyes.Visor
         Eyes Eyes.Cyclops
         Eyes Eyes.Rainbow
         Eyes Eyes.CyclopsVisor
         Eyes Eyes.Glasses3D
         Eyes Eyes.Sunglasses]
     "Head Detail",
        [HeadDetail HeadDetail.Electronic
         HeadDetail HeadDetail.Mohawk
         HeadDetail HeadDetail.Plain]
     "Mouth Detail",
        [MouthDetail MouthDetail.Smile
         MouthDetail MouthDetail.SmileWithCigarette
         MouthDetail MouthDetail.Straight
         MouthDetail MouthDetail.StraightWithCigarette]]

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
                        option [Value i] [str <| Cubehead.getTraitName opt]))]]]]

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
    div [ClassName "columns"]
        [div [ClassName "column is-3"]
            [section [ClassName "box section gallery-filter"]
                [ofList <| (filters |> List.map (fun (name, options) -> filter (model.filter |> Map.tryFind name) name options dispatch))
                 search model dispatch
                 reset dispatch]]
         div [ClassName "column is-9"]
            [section [ClassName "section"] [Elmish.React.Common.lazyView2With refEquals (fun model dispatch ->
                let filter = model.filter |> Map.toList |> List.map snd
                let elements = Cubehead.getAllCubeheads filter model.idSearch |> List.map (fun c -> ViewComponents.cubeheadsCompact c dispatch)
                ViewComponents.cubeheadsGrid elements) model dispatch]]]
