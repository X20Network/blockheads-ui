module UserGuide.View

open Fable.React
open Fable.React.Props

let root =
    div []
        [div [ClassName "block section content"]
            [div [ClassName "box"]
                [div [ClassName "section content"]
                    [h1 [ClassName "title"] [str "How to get a Cubehead"]
                     div [ClassName "columns"]
                        [div [ClassName "column is-four-fifths content"]
                            [p [] [b [] [str "The Minting Process"]] 
                             p [] [str "Cubeheads will be minted in batches of 6 in a Dutch auction. Minting fees will start at 0.2ETH and gradually get lower until each Cubehead has sold. Once the entire batch has sold, another batch will be released at the starting price and the process will repeat for all 1995 available Cubeheads."]
                             p [] [b [] [str "Secondary Markets"]]
                             p [] [str "Cubeheads will also be available on NFT marketplaces. This is the only place to get one after all 1995 of them have been minted."]]
                         div [ClassName "column"]
                            [i [ClassName "mdi mdi-cube-send"] []]]]]]
         div [ClassName "block section content"]
            [div [ClassName "box"] [div [ClassName "section content"]
                []]]
         ]
