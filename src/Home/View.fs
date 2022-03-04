module Home.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Types
open Common
open System.Globalization

let introTxt =
    ofList
        [h1 [ClassName "title"] [str "CUBEHEADS"]
         p [] [b [] [str "Welcome to the Cubeverse. Spacetime is now cubed."]]
         p [] [str "Cubeheads are a limited collection of fully on-chain, gamified NFTs that push the limits of what is possible in an NFT smart contract."]
         p [] [str "Each Cubehead can participate with 3 others in a team sport called Cubeball, where the on-chain emergent AI embedded inside each Cubehead NFT compete together for NFT trophies and the right to breed more Cubeheads (the Droneheads)."]]

let intro =
    div [ClassName "columns"]
        [div [ClassName "column is-8"] [introTxt]
         div [ClassName "column is-4"] [img [Src "/img/cubeheadsgrid.png"]]]

let strategy strategy =
    let elikely = 0.21
    let wvlikely = 0.18
    let wlikely = 0.14
    let getProbStr strategyType strategyWeightType str =
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
        //weighting.ToString()
    let tags =
        [getProbStr WithBallMove DistanceGoal " to move forward with the ball"
         getProbStr WithBallMove NumAttackers " to run into space with the ball"
         getProbStr WithBallMove NumDefenders " to avoid defenders with the ball"
         getProbStr WithBallPass DistanceGoal " to pass forward"
         getProbStr WithBallPass NumAttackers " to pass to a player in space"
         getProbStr WithBallPass NumDefenders " to pass to an unmarked player"
         getProbStr WithBallPass InvPossStrength " to pass to an agile player"
         getProbStr WithBallPass InvPassStrength " to pass to an accurate player"
         getProbStr WithBallPass PassNotRequested " to pass to a request for ball"
         getProbStr InPoss DistanceGoal " to move forward when attacking"
         getProbStr InPoss DistanceBall " to follow the ball when attacking"
         getProbStr InPoss NumAttackers " to move into space when attacking"
         getProbStr InPoss NumDefenders " to avoid defenders when attacking"
         getProbStr OutPoss DistanceGoal " to move back when defending"
         getProbStr OutPoss DistanceBall " to chase the ball when defending"
         getProbStr OutPoss NumAttackers " to mark attackers when defending"
         getProbStr OutPoss NumDefenders " to cover space when defending"]
            |> List.filter (fun (s, _) -> s |> String.length > 0)
            |> List.map (fun (s, icon) ->
                div [Class "control"]
                    [div [Class "tags has-addons strategy"]
                        [span [ClassName "tag"] [str s]
                         span [ClassName "tag is-dark"]
                            [span [ClassName "icon is-small"]
                                [i [ClassName icon] []]]]])
    div [Class "field is-grouped is-grouped-multiline"] tags

let strengths cubehead =
    let row name value =
        [td []
            [str name]
         td [Class <| if value >= 0 then "bar-on" else "bar-off"] []
         td [Class <| if value >= 1 then "bar-on" else "bar-off"] []
         td [Class <| if value >= 2 then "bar-on" else "bar-off"] []
         td [Class <| if value >= 3 then "bar-on" else "bar-off"] []]
    table [Class "strengths"]
        [tr [] (row "Strength" cubehead.strength)
         tr [] (row "Speed" cubehead.speed)
         tr [] (row "Agility" cubehead.agility)
         tr [] (row "Accuracy" cubehead.accuracy)]

let visualTrait vtrait =
    let rarity = Cubehead.getRarity vtrait
    div [Class "control"] 
        [div [Class "tags has-addons visual-trait"]
            [span [Class "tag"] [b [] [str <| sprintf "%s:" (Cubehead.getTraitTypeName vtrait)]; str <| Cubehead.getTraitName vtrait]
             span [Class "tag is-primary"] [str <| sprintf "%.0f%%" (rarity * 100.0) ]]]

let visualTraits vtraits =
    div [Class "field is-grouped is-grouped-multiline"] (vtraits |> List.map visualTrait)

let buyButton accountData auctionBatch auction =
    match accountData with
    | None ->
        a [ClassName "button is-primary"; Disabled true] [str "Connect Account"]
    | Some _ ->
        match auction.priceSold with
        | None ->
            a [ClassName "button is-primary"] [
                b[] [str "Buy for 0.2 ETH"]]
        | Some price ->
            a [ClassName "button"; Disabled true] [
                b[] [str <| sprintf "Sold for %M ETH" price]]

let auctionView accountData auctionBatch (auction :Auction) =
    let image = JS.encodeURIComponent auction.cubehead.svg
    let imageSrc = """data:image/svg+xml, """ + image
    let imageIndex = auction.cubehead.index % 305
    let imageSrc = "/img/cubeheads/cubehead (" + imageIndex.ToString() + ").png"
    article [ClassName "media auction-cubehead"]
        [figure [ClassName "media-left"]
            [img [Src imageSrc]]
         div [ClassName "media-content"]
            [div [ClassName "content"]
                [div [ClassName "mb4"] [ViewComponents.cubeheadDetail auction.cubehead]
                 nav [ClassName "level"]
                     [div [ClassName "level-left"] []
                      div [ClassName "level-right"]
                         [div [ClassName "level-item"]
                            [buyButton accountData auctionBatch auction]]]]]]


let auctions model =
    match model.currentAuction with
    | None ->
        div [ClassName "notification is-info"]
            [p [] [str "All cubehead auctions have finished. Cubeheads must be purchased from secondary markets."]
             br []
             a [ClassName "button"] [str "Buy on Opensea"]]
    | Some auction ->
        let hours = int <| auction.timeRemaining.TotalHours
        let mins = int <| auction.timeRemaining.TotalMinutes
        let secs = int <| auction.timeRemaining.Seconds
        div []
            [nav [ClassName "level auction-countdown"]
                [div [ClassName "level-item has-text-centered"]
                    [div [] 
                        [p [ClassName "heading"] [str "Current Price (ETH)"]
                         h1 [ClassName "title"] [str "0.2"]]]
                 div [ClassName "level-item has-text-centered"]
                    [div []
                        [p [ClassName "heading"] [str "Time Remaining"]
                         h1 [ClassName "title"] [str <| sprintf "%02i:%02i:%02i" hours mins secs] ]]]]

let cubeheads model =
    match model.currentAuction with
    | None -> div [] []
    | Some auction ->
        nav [ClassName "level is-hidden-mobile"]
            (auction.auctions |> List.map (fun auction ->
                let imageIndex = auction.cubehead.index % 305
                let imageSrc = "/img/cubeheads/cubehead (" + imageIndex.ToString() + ").png"
                div [ClassName "level-item"] [img [Src imageSrc]]))

let root model accountData dispatch =
    div []
        [div [ClassName "block section content intro"]
            [div [ClassName "box"] [div [ClassName "section content"] [intro]]]
         div [ClassName "block section content"]
            [div [ClassName "box"]
                [section [ClassName "section content"]
                    [h1 [ClassName "title"] [str "Minting"]
                     p [] [b [] [str "Be part of the Cubeverse"]]
                     p [] [str "Cubeheads are sold in batches of 6 every 8 hours. The price starts high and goes lower every second. Be quick - once they're gone, they're gone!"]]
                 section [ClassName "auction-batch-imgs"]
                    [cubeheads model]
                 section [ClassName "section"]
                    [auctions model]
                 ofOption (model.currentAuction |> Option.map (fun auction ->
                 section [ClassName "section"] (auction.auctions |> List.map (auctionView accountData model.currentAuction))))]]]

let root2 model dispatch =
  div
    [ ]
    [ p
        [ ClassName "control" ]
        [ input
            [ ClassName "input"
              Type "text"
              Placeholder "Type your name"
              DefaultValue model
              AutoFocus true
              OnChange (fun ev -> !!ev.target?value |> ChangeStr |> dispatch ) ] ]
      br [ ]
      span
        [ ]
        [ str (sprintf "Hello %s" model) ] ]