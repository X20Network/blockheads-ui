module ViewComponents

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Common
open System.Globalization

let emphasise s = span [ClassName "emphasis"] [str s]

let elikely = 0.21
let wvlikely = 0.18
let wlikely = 0.14

let getProbStr strategy strategyType strategyWeightType str =
    let weighting = strategy.weightings |> Map.find strategyType |> Map.find strategyWeightType
    let s =
        match weighting with
        | x when x > elikely -> "Extremely likely" + str
        | x when x > wvlikely -> "Very likely" + str
        | x when x > wlikely -> "Likely" + str
        | _ -> ""
    let icon =
        match strategyType with
        | WithBallPass | WithBallMove -> "mdi mdi-soccer"
        | InPoss -> "mdi mdi-transfer-up"
        | OutPoss -> "mdi mdi-transfer-down"
    s, icon

let withBallTags strategy =
    [getProbStr strategy WithBallMove DistanceGoal " to move forward"
     getProbStr strategy WithBallMove NumAttackers " to run into space"
     getProbStr strategy WithBallMove NumDefenders " to avoid defenders"
     getProbStr strategy WithBallPass DistanceGoal " to pass forward"
     getProbStr strategy WithBallPass NumAttackers " to pass to a player in space"
     getProbStr strategy WithBallPass NumDefenders " to pass to an unmarked player"
     getProbStr strategy WithBallPass InvPossStrength " to pass to an agile player"
     getProbStr strategy WithBallPass InvPassStrength " to pass to an accurate player"
     getProbStr strategy WithBallPass PassNotRequested " to pass to a request"]

let attackTags strategy =
    [getProbStr strategy InPoss DistanceGoal " to move forward"
     getProbStr strategy InPoss DistanceBall " to follow the ball"
     getProbStr strategy InPoss NumAttackers " to move into space"
     getProbStr strategy InPoss NumDefenders " to avoid defenders"]

let defendTags strategy =
    [getProbStr strategy OutPoss DistanceGoal " to move back"
     getProbStr strategy OutPoss DistanceBall " to chase the ball"
     getProbStr strategy OutPoss NumAttackers " to mark attackers"
     getProbStr strategy OutPoss NumDefenders " to cover space"]

let strategy tags =
        //weighting.ToString()
    let tags =
        tags
            |> List.filter (fun (s, _) -> s |> String.length > 0)
            |> List.map (fun (s, icon) ->
                div [Class "control"]
                    [div [Class "tags has-addons strategy"]
                        [span [ClassName "tag"] [str s]
                         span [ClassName "tag is-dark"]
                            [span [ClassName "icon is-small"]
                                [i [ClassName icon] []]]]])
    div [Class "field is-grouped is-grouped-multiline"] tags

let strengths blockhead =
    let row name value =
        [td []
            [str name]
         td [Class <| if value >= 0 then "bar-on" else "bar-off"] []
         td [Class <| if value >= 1 then "bar-on" else "bar-off"] []
         td [Class <| if value >= 2 then "bar-on" else "bar-off"] []
         td [Class <| if value >= 3 then "bar-on" else "bar-off"] []]
    table [Class "strengths"]
        [tr [] (row "Strength" blockhead.strength)
         tr [] (row "Speed" blockhead.speed)
         tr [] (row "Agility" blockhead.agility)
         tr [] (row "Accuracy" blockhead.accuracy)]

let visualTrait vtrait =
    let rarity = Blockhead.getRarity vtrait
    div [Class "control"] 
        [div [Class "tags has-addons visual-trait"]
            [span [Class "tag"] [b [] [str <| sprintf "%s:" (Blockhead.getTraitTypeName vtrait)]; str <| Blockhead.getTraitName vtrait]
             span [Class "tag is-primary"] [str <| sprintf "%.0f%%" (rarity * 100.0) ]]]

let visualTraits vtraits =
    div [Class "field is-grouped is-grouped-multiline"] (vtraits |> List.map visualTrait)

let blockheadDetailCustomName blockhead name =
    let scoreStr = sprintf "%.1f%%" (System.Math.Round(blockhead.strategy.score * 100.0, 1))
    div [ClassName "content"]
        [name
         p [] [b [] [str "Visual Traits"]]
         visualTraits blockhead.visualTraits
         p [] [b [] [str "Physical Traits"]]
         strengths blockhead
         p [] [b [] [str "Blockball Strategy Score"]]
         p [ClassName "is-size-3 has-text-info"] [b [] [str scoreStr]]
         p [] [b [] [str "Blockball Behavior Traits"]]
         p [ClassName "is-size-7"] [b [] [str "With the ball:"]]
         (strategy (withBallTags blockhead.strategy))
         p [ClassName "is-size-7"] [b [] [str "When Attacking:"]]
         (strategy (attackTags blockhead.strategy))
         p [ClassName "is-size-7"] [b [] [str "When Defending:"]]
         (strategy (defendTags blockhead.strategy))]

let blockheadDetail blockhead = blockheadDetailCustomName blockhead (h1 [] [str blockhead.name])

type BlockheadCompactComponent(initialProps) =
    inherit PureStatelessComponent<KeyedItem<Blockhead>>(initialProps)

    override this.render() =
        let image = JS.encodeURIComponent this.props.Item.svg
        //let imageSrc = """data:image/svg+xml, """ + image
        let imageSrc = this.props.Item.svg
        article [ClassName "box blockhead-tile"]
            [a [Href <| "blockhead/" + this.props.Item.index.ToString()] [img [Src imageSrc]]
             h1 [] [str this.props.Item.name]]

let blockheadsCompact blockhead dispatch =
    ofType<BlockheadCompactComponent, _, _> { Key = blockhead.name; Item = blockhead } []

let blockheadsMini blockhead dispatch =
    let image = JS.encodeURIComponent blockhead.svg
    let imageSrc = ("""data:image/svg+xml, """ + image)
    let imageSrc = blockhead.svg
    article [ClassName "blockhead-mini"]
        [a [Href <| "blockhead/" + blockhead.index.ToString()] [img [Style []; Src imageSrc ]]
         h1 [] [str blockhead.name]]

let blockheadsGrid blockheadElements =
    div [ClassName "blockhead-grid"] blockheadElements
