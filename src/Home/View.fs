module Home.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Types
open Common
open System.Globalization
open ViewComponents

let carouselImgs =
    [  "2C29473968.svg"
       "C7E318B909.svg"
       "A000EF309E.svg"
       "A5B28A1541.svg"
       "EDA1EFEE48.svg"
       "16BBC20B53.svg"
       "33BC003B0D.svg"
       "4C2CB82A2D.svg"
       "00F6E1010A.svg"
       "075243C90A.svg" ]

let carouselImgs2 =
    [ "3dblockhead1.webp"
      "3dblockhead2.webp"
      "3dblockhead3.webp"
      "3dblockhead4.webp"
      "3dblockhead5.webp"
      "3dblockhead6.webp"]

let bullet icon content =
   article [ClassName "media"]
       [figure [ClassName "media-left"] [span [ClassName "icon is-medium"] [i [ClassName <| "mdi mdi-36px " + icon] []]]
        div [ClassName "media-content"]
           [div [ClassName "content"]
               content]]

let carousel index =
    let index = index % carouselImgs.Length
    let prev = if index = 0 then carouselImgs.Length - 1 else index - 1
    let next = if index = carouselImgs.Length - 1 then 0 else index + 1
    div [ClassName "carousel-wrapper"]
        [div [ClassName "carousel"]
            (carouselImgs |> List.mapi (fun i src -> img [classList ["active", i = index; "prev", i = prev; "next", i = next];Src <| "/img/blockhead-svgs/" + src]))]

let carousel2 index =
    let index = index % carouselImgs2.Length
    let prev = if index = 0 then carouselImgs2.Length - 1 else index - 1
    let next = if index = carouselImgs2.Length - 1 then 0 else index + 1
    div [ClassName "carousel-wrapper"]
        [div [ClassName "carousel"]
            (carouselImgs2 |> List.mapi (fun i src -> img [classList ["active", i = index; "prev", i = prev; "next", i = next];Src <| "/img/" + src]))]

let countdown (days, hours, mins, secs) =
    nav [ClassName "level countdown"]
        [div [ClassName "level-item has-text-centered"]
            [div []
                [p [ClassName "heading"] [str "Days"]
                 p [ClassName "title"] [str <| days.ToString() ]]]
         div [ClassName "level-item has-text-centered"]
             [div []
                 [p [ClassName "heading"] [str "Hours"]
                  p [ClassName "title"] [str <| hours.ToString() ]]]
         div [ClassName "level-item has-text-centered"]
             [div []
                 [p [ClassName "heading"] [str "Minutes"]
                  p [ClassName "title"] [str <| mins.ToString() ]]]
         div [ClassName "level-item has-text-centered"]
             [div []
                 [p [ClassName "heading"] [str "Seconds"]
                  p [ClassName "title"] [str <| secs.ToString() ]]]]

let introTxt timeToLaunch carouselIndex =
    ofList
        [//h1 [ClassName "title"] [str "BLOCKHEADS"]
         img [ClassName "header-logo"; Src "/img/blockheadslogo.png"] 
         p [ClassName "has-text-centered content slogan"] [b [] [str "What happens onchain, stays onchain."]]
         div [ClassName "hero"] [div [ClassName "blockheads-hero home"] []]
         //div [ClassName "section container"]
         //   [div [ClassName "launch-date"] [p [] [str "Whitelist Opens 1st August 2022, 16:00 UTC"]]
         //    countdown timeToLaunch]
         carousel carouselIndex
         div [ClassName "hero3 content section has-text-centered"]
             [div [ClassName "container"]
                 [bullet "mdi-cube-outline"
                     [p [] [b [] [str "First fully on-chain 3D voxel-based artwork"]]
                      p [] [str "No IPFS or external storage"]]
                  bullet "mdi-cube-outline"
                     [p [] [b [] [str "First fully on-chain animated artwork"]]
                      p [] [str "Pushing the limits of what's possible on-chain"]]
                  bullet "mdi-cube-outline"
                     [p [] [b [] [str "First on-chain game with complex emergent behavior"]]
                      p [] [str "Enter 4 Blockheads in the team game of Blockball"]]
                  bullet "mdi-cube-outline"
                     [p [] [b [] [str "First on-chain game with evolvable AI strategies"]]
                      p [] [str "Improve your chances of winning by breeding Blockheads"]]]]
         div [ClassName "hero2 content section has-text-centered"]
             [div [ClassName "container has-text-white"]
                 [bullet "mdi-check-decagram-outline"
                     [p [] [b [] [str "No royalty fees on sales"]]
                      p [] [str "NFTs should be free to exchange"]]
                  bullet "mdi-check-decagram-outline"
                     [p [] [b [] [str "No profits from minting"]]
                      p [] [str "Minting fees go into a gas pool to pay for gaming"]]
                  bullet "mdi-check-decagram-outline"
                     [p [] [b [] [str "No marketing BS"]]
                      p [] [str "We're coders and our code does the talking"]]]]
         div [ClassName "mailing-list"]
            [div [ClassName "ml-embedded"; HTMLAttr.Custom("data-form", "gsrD07")] []]]

let intro =
    introTxt

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

let buyButton accountData auctionBatch auction dispatch =
    match accountData with
    | None ->
        a [ClassName "button is-primary"; Disabled true] [str "Connect Account"]
    | Some _ ->
        match auction.priceSold, auctionBatch with
        | None, Some auctionBatch ->
            match auction.minting with
            | None ->
                let price = sprintf "%.4f" auctionBatch.price 
                a [ClassName "button is-primary"; OnClick (fun _ -> dispatch <| MintBlockhead auction.blockhead.index)] [
                    b[] [str ("Buy for " + price + " ETH")]]
            | Some _ ->
                div [ClassName "lds-dual-ring"] []
        | Some price, _ ->
            a [ClassName "button"; Disabled true] [
                b[] [str <| sprintf "Sold"]]

let auctionView accountData auctionBatch dispatch (auction :Auction) =
    let image = JS.encodeURIComponent auction.blockhead.svg
    let imageSrc = """data:image/svg+xml, """ + image
    let imageIndex = auction.blockhead.index % 305
    let imageSrc = auction.blockhead.svg
    let name =
        match auction.priceSold with
        | Some _ -> h1 [] [span [Style [TextDecoration "line-through"]] [str auction.blockhead.name]; str " - "; span [ClassName "has-text-danger"] [str "SOLD"]]
        | None -> h1 [] [str auction.blockhead.name]
    article [ClassName "media auction-blockhead"]
        [figure [ClassName "media-left"]
            [img [Src imageSrc]]
         div [ClassName "media-content"]
            [div [ClassName "content"]
                [div [ClassName "mb4"] [ViewComponents.blockheadDetailCustomName auction.blockhead name]
                 nav [ClassName "level"]
                     [div [ClassName "level-left"] []
                      div [ClassName "level-right"]
                         [div [ClassName "level-item"]
                            [buyButton accountData auctionBatch auction dispatch]]]]]]


let auctions model =
    match model.currentAuction with
    | None ->
        div [ClassName "has-text-centered"] [div [ClassName "lds-dual-ring"] []]
    | Some auction ->
        let hours = int <| auction.timeRemaining.TotalHours
        let mins = int <| auction.timeRemaining.TotalMinutes
        let secs = int <| auction.timeRemaining.Seconds
        let priceStr = auction.price.ToString()
        let price = priceStr.Substring(0, min 10 (priceStr.Length))
        div []
            [nav [ClassName "level auction-countdown"]
                [div [ClassName "level-item has-text-centered"]
                    [div [] 
                        [p [ClassName "heading"] [str "Current Price (ETH)"]
                         h1 [ClassName "title"] [str price]]]
                 div [ClassName "level-item has-text-centered"]
                    [div []
                        [p [ClassName "heading"] [str "Time Remaining"]
                         h1 [ClassName "title"] [str <| sprintf "%02i:%02i:%02i" hours mins secs] ]]]]

let blockheads model =
    match model.currentAuction with
    | None -> div [] []
    | Some auction ->
        nav [ClassName "level is-hidden-mobile"]
            (auction.auctions |> List.map (fun auction ->
                let imageIndex = auction.blockhead.index % 305
                let imageSrc = auction.blockhead.svg
                div [ClassName "level-item"] [img [Src imageSrc]]))

let root timeToLaunch carouselIndex dispatch =
    //div []
    //    [div [ClassName "block section intro"]
    //        [div [ClassName "box has-text-centered"] [intro timeToLaunch carouselIndex]]
    //     ]
    div [ClassName "home"]
        [h1 [] [str "Blockheads"]
         h2 [] [str "What happens on-chain, stays on-chain"]
         div [Id "blockworld-container"]
            [img [Src "/img/blockworld.webp"; Id "blockworld-image"]
             img [Src "/img/pubglow.webp"; Id "pubglow-image"; ClassName "blockworld-highlight"]
             img [Src "/img/stadiumglow.webp"; Id "stadiumglow-image"; ClassName "blockworld-highlight"]
             img [Src "/img/galleryglow.webp"; Id "galleryglow-image"; ClassName "blockworld-highlight"]
             // factory area
             a [Style [Top "15%"; Left "21.5%"; Width "15%"; Height "28%"]; ClassName "blockworld-area"; Id "factory-area"; Href "#factory"]
                []
             div [] [
                img [Src "/img/factoryglow.webp"; Id "factoryglow-image"; ClassName "blockworld-highlight"]
                a [Style [Top "13%"; Left "21.5%"];ClassName "blockworld-label"] [str "Blockheads Factory"]]
             // research lab area
             a [Style [Top "20.5%"; Left "36.5%"; Width "14%"; Height "28%"]; ClassName "blockworld-area"; Id "lab-area"; Href "#lab"]
                []
             div [] [
                 img [Src "/img/labglow.webp"; Id "labglow-image"; ClassName "blockworld-highlight"]
                 a [Style [Top "17%"; Left "36.5%"]; ClassName "blockworld-label"] [str "Block Labs"]]
             // pub area
             a [Style [Top "20.5%"; Left "36.5%"; Width "14%"; Height "28%"]; ClassName "blockworld-area"; Id "pub-area"; Href "#pub"]
                []
             ]
         section [ClassName "vlight"]
            [div [ClassName "blockheadsgrid"] []]
         section [ClassName "vlight"; Id "section1"]
            [div [ClassName "container"] 
                [div [ClassName "left"]
                    [h1 [] [str "Full 3D, 100% On-chain NFT"]
                     p [] [str "Blockheads is a collection of 1995 unique, fully on-chain 3D NFTs. Instead of storing a simple picture file on a server or IPFS, we built a custom voxel rendering engine inside an Ethereum smart contract. This means your art will last as long as the blockchain itself."]]
                 div [ClassName "right"]
                    [img [Src "/img/gridrenderblockhead.webp"]]]]
         section [ClassName "vlight"]
            [div [ClassName "container"]
                [div [ClassName "left"]
                    [carousel carouselIndex]
                 div [ClassName "right"]
                    [h1 [] [str "Displayed as SVG"]
                     p [] [str "Blockheads NFTs can directly output isometric svg images. We really pushed the NFT smart contract to the limit."]]]]
         section [ClassName "vlight"]
            [div [ClassName "container"]
                [div [ClassName "left"]
                    [h1 [] [str "Output as 3D GLTF File"]
                     p [] [str "Blockheads NFTs can also output standard 3D GLTF files directly from the contract. These files are ready for model viewers, 3D rendering and metaverses."]]
                 div [ClassName "right"]
                    [carousel2 carouselIndex]]]
         section [ClassName "vlight"]
            [div [ClassName "blockheadsgrid"] []]
         section [ClassName "purple has-text-centered"; Id "factory"]
            [img [ClassName "title-image"; Src "/img/factory2.webp"]
             h1 [] [str "The Blockheads Factory"]
             p [] [str "Follow us on Twitter for updates on when minting will start"]
             img [Src "/img/minting.svg"; Id "minting-image"]
             div [ClassName "icon-point"]
                [p [] [span [ClassName "icon is-medium"] [i [ClassName "mdi mdi-36px mdi-check-decagram-outline"] []]]
                 p [ClassName "point-title"] [str "Fair Minting Process"]
                 p [] [str "We will mint in a dutch auction process in batches of 6 blockheads"]]
             div [ClassName "icon-point"]
                 [p [] [span [ClassName "icon is-medium"] [i [ClassName "mdi mdi-36px mdi-check-decagram-outline"] []]]
                  p [ClassName "point-title"] [str "No Royalties"]
                  p [] [str "NFTs should be free to exchange"]]
             div [ClassName "icon-point"]
                 [p [] [span [ClassName "icon is-medium"] [i [ClassName "mdi mdi-36px mdi-check-decagram-outline"] []]]
                  p [ClassName "point-title"] [str "No Profits from Minting"]
                  p [] [str "Minting fees go into a gas pool to pay for on-chain games"]]]
         section [ClassName "vlight"]
            [div [ClassName "blockheadsgrid"] []]
         section [ClassName "vlight has-text-centered"]
            [img [Src "/img/stadium.webp"; Id "stadium-image"]
             h1 [] [str "The Blockball Stadium"]]
         section [ClassName "vlight"]
            [div [ClassName "container"]
                [div [ClassName "left"]
                    [h1 [] [str "100% On-chain Game"]
                     p [] [str "Blockheads can form teams of 4 to play games of Blockball - an AI powered, cellular automata inspired football game that happens entirely on-chain and with full complex emergent behavior and unpredictable results."]]
                 div [ClassName "right"]
                    [img [Src "/img/blockball.svg"; Id "blockball-image"]]]]
         section [ClassName "vlight"]
             [div [ClassName "container"]
                 [div [ClassName "left"]
                    [img [Src "/img/gridrendertrophy.webp"]]
                  div [ClassName "right"]
                     [h1 [] [str "On-chain Winner Trophy NFT"]
                      p [] [str "Winners of Blockball receive a fully 3D, on-chain, animated NFT trophy that shows all the action in the Blockball game. There are 6 different color-schemes to collect."]]
                  ]]
         section [ClassName "vlight"]
             [div [ClassName "container"]
                 [div [ClassName "left"]
                     [h1 [] [str "Evolvable AI"]
                      p [] [str "Winners of a Blockball game can breed 2 Blockheads together to create a Blocklet NFT. This Blocklet can be included in future games and is a combination of the AI of its parents. In this way it is possible to evolve better teams to increase your chances of winning Blockball."]]
                  div [ClassName "right"]
                     [img [Src "/img/evolve.svg"; Id "evolve-image"]]]]
         section [ClassName "vlight"]
            [div [ClassName "blockheadsgrid"] []]
         section [Id "team-section"; ClassName "blue has-text-centered "]
            [h1 [] [str "About Us"]
             img [Src "/img/mechanism.png"; Id "mechanism-image"]
             p [] [str "Blockheads were created by two developers with a belief that engineering and code is a form of art."; br []; str "No matter what happens in this space we will put technology and a desire to build genuine value above all else."; br []; str "These NFTs reflect our values and passion."]
             img [Src "/img/kyubu.png"]
             img [Src "/img/bloxor.png"]
             ]]
