module Info.View

open Fable.React
open Fable.React.Props

let root =
  let bullet icon content =
    article [ClassName "media"]
        [figure [ClassName "media-left"] [span [ClassName "icon is-medium"] [i [ClassName <| "mdi mdi-36px " + icon] []]]
         div [ClassName "media-content"]
            [div [ClassName "content"]
                content]]

  let roadmapLabel title sub statusTxt isComplete =
    let status =
        match isComplete with
        | true -> p [ClassName "has-text-success"] [span [] [str "Complete"]; span [ClassName "icon"] [i [ClassName "mdi mdi-check-bold"] []]]
        | false -> p [ClassName "has-text-primary"] [str statusTxt]
    div [ClassName "roadmap-label"]
        [p [] [str title]
         p [ClassName "is-size-6"] [i [] [str sub]]
         status]
  let rmlabel1 = roadmapLabel "Blockheads Launch" "Pushing NFT on-chain to the limits" "Complete" true
  let rmlabel2 = roadmapLabel "Blockball Upgrades" "Training camp, personalization, full 3D" "Q2 2022" false
  let rmlabel3 = roadmapLabel "Blockheads Chapter 2" "Discovery" "Q3 2022" false
  let rmlabel4 = roadmapLabel "Blockheads Chapter 3" "Governance" "Q4 2022" false
  let rmlabel5 = roadmapLabel "Blockheads Chapter 4" "Voxel Metaverse" "2023" false
  div []
      [div [ClassName "block section content about"]
          [div [ClassName "box"]
            [section [ClassName "has-text-centered section content"]
                [h1 [] [str "CUBEHEADS"]
                 p [] [b [] [str "Welcome to the Blockverse."]]]
             div [ClassName "hero"] [div [ClassName "blockheads-hero"] []]
             section [ClassName "section content has-text-centered container"]
                [h2 [] [str "0xBlocko & 0xBlockd"]
                 p [] [str "Blockheads hails from the imagination of two passionate blockchain enthusiasts who had a desire to push the limits of what was possible with an on-chain NFT collection and form a lasting community with strong crypto values and an ambitious roadmap."]
                 img [Src "/img/teamblocks.png"]]
             div [ClassName "hero2 content section has-text-centered"]
                [h2 [] [str "Our Values"]
                 div [ClassName "container has-text-white"]
                    [bullet "mdi-check-decagram-outline"
                        [p [] [b [] [str "No royalty fees on sales"]]
                         p [] [str "NFTs should be free to exchange"]]
                     bullet "mdi-check-decagram-outline"
                        [p [] [b [] [str "No profits from minting"]]
                         p [] [str "Minting fees go into a gas pool to pay for gaming"]]
                     bullet "mdi-check-decagram-outline"
                        [p [] [b [] [str "No marketing BS"]]
                         p [] [str "We're coders and our code does the talking"]]]]
             div [ClassName "hero3 content section has-text-centered"]
                [h2 [] [str "The Collection"]
                 div [ClassName "container"]
                    [bullet "mdi-block-outline"
                        [p [] [b [] [str "First fully on-chain 3D voxel based artwork"]]
                         p [] [str "No IPFS or external storage"]]
                     bullet "mdi-block-outline"
                        [p [] [b [] [str "First fully on-chain animated artwork"]]
                         p [] [str "Pushing the limits of what's possible on-chain"]]
                     bullet "mdi-block-outline"
                        [p [] [b [] [str "First on-chain game with complex emergent behavior"]]
                         p [] [str "Enter 4 Blockheads in the team game of Blockball"]]
                     bullet "mdi-block-outline"
                        [p [] [b [] [str "First on-chain game with evolvable AI strategies"]]
                         p [] [str "Improve your chances of winning by breeding Blockheads"]]]]
             div [ClassName "hero4 content section has-text-centered"]
                [h2 [] [str "The Roadmap"]
                 div [ClassName "container"]
                    [div [ClassName "columns roadmap is-hidden-mobile"]
                        [div [ClassName "column"]
                            [rmlabel1
                             rmlabel3
                             rmlabel5]
                         div [ClassName "column"]
                            [svg [ViewBox "-2 0 104 300"]
                                [path [D "M 50 0 L 0 28.87 L 100 86.61 L 0 144.35 L 100 202.09 0 259.85"; SVGAttr.Stroke "white"; SVGAttr.Fill "none"] []
                                 path [D "M 50 0 L 0 28.87 L 50 57.75 Z"; SVGAttr.Stroke "none"; SVGAttr.Fill "rgba(255, 255, 255, 0.15)"] []
                                 path [D "M 0 28.87 L 0 144.35 L 100 86.61"; SVGAttr.Stroke "none"; SVGAttr.Fill "rgba(0, 0, 0, 0.35)"] []
                                 path [D "M 100 86.61 L 0 144.35 L 100 202.09"; SVGAttr.Stroke "none"; SVGAttr.Fill "rgba(255, 255, 255, 0.15)"] []
                                 path [D "M 0 144.35 L 0 259.85 L 100 202.09"; SVGAttr.Stroke "none"; SVGAttr.Fill "rgba(0, 0, 0, 0.35)"] []
                                 path [D "M 50 0 L 0 28.87 L 100 86.61 L 0 144.35 L 100 202.09 0 259.85"; SVGAttr.Stroke "white"; SVGAttr.Fill "none"] []
                                 circle [Cx "0"; Cy "28.87"; R "2"; SVGAttr.Fill "white"; SVGAttr.Stroke "none" ] []
                                 circle [Cx "100"; Cy "86.61"; R "2"; SVGAttr.Fill "white"; SVGAttr.Stroke "none" ] []
                                 circle [Cx "0"; Cy "144.35"; R "2"; SVGAttr.Fill "white"; SVGAttr.Stroke "none" ] []
                                 circle [Cx "100"; Cy "202.09"; R "2"; SVGAttr.Fill "white"; SVGAttr.Stroke "none" ] []
                                 circle [Cx "0"; Cy "259.85"; R "2"; SVGAttr.Fill "white"; SVGAttr.Stroke "none" ] []]]
                         div [ClassName "column"]
                            [rmlabel2
                             rmlabel4 ]]
                     div [ClassName "is-hidden-tablet roadmap"]
                        [span [ClassName "icon is-large"] [i [ClassName "mdi mdi-arrow-down-bold-hexagon-outline mdi-36px"] []]
                         rmlabel1
                         span [ClassName "icon is-large"] [i [ClassName "mdi mdi-arrow-down-bold-hexagon-outline mdi-36px"] []]
                         rmlabel2
                         span [ClassName "icon is-large"] [i [ClassName "mdi mdi-arrow-down-bold-hexagon-outline mdi-36px"] []]
                         rmlabel3
                         span [ClassName "icon is-large"] [i [ClassName "mdi mdi-arrow-down-bold-hexagon-outline mdi-36px"] []]
                         rmlabel4
                         span [ClassName "icon is-large"] [i [ClassName "mdi mdi-arrow-down-bold-hexagon-outline mdi-36px"] []]
                         rmlabel5]]]]]]
