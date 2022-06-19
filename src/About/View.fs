module About.View

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open System.Globalization

let root =
    div []
        [div [ClassName "block section intro"]
            [div [ClassName "box has-text-centered content container"]
                [h1 [ClassName "section"] [str "Fully On-chain"]
                 p [] [str "Most NFTs simply store an ownership record in the blockchain that points to an image file that is stored somewhere else."; br []; str "Usually on IPFS or a cloud server."; b [] [str " If those disappear then your artwork is gone."]]
                 img [Src "img/nft-ipfs.svg"; Style [Width "32rem"]]
                 p [] [str "Blockheads are on-chain NFTs. They generate an image inside a blockchain smart contract. We are the first 3D voxel-based on-chain NFT."]
                 img [Src "img/nft-onchain.svg"; Style [Width "32rem"]]
                 h1 [ClassName "section"] [str "Fair Minting"]
                 p [] [str "Blockheads are minted in batches of 6, chosen at random from the collection. The price starts high and gradually lowers until the Blockhead is sold. "; br []; str "The auction time for each batch is at maximum 6 hours, and minting the whole collection is expected to take between 3-6 weeks."]
                 img [Src "img/minting.svg"; Style [Width "32rem"]]
                 h1 [ClassName "section"] [str "Blockball"]
                 p [] [str "Blockheads can form teams of 4 to play games of Blockball. This is an AI-powered, cellular automata inspired football game that also happens entirely on-chain with full complex emergent behaviour and unpredictable results."]
                 img [Src "img/blockball.svg"; Style [Width "32rem"]]
                 h1 [ClassName "section"] [str "Evolvable AI"]
                 p [] [str "Winners of a Blockball game can breed 2 Blockheads together to create a Blocklet NFT. This Blocklet can be included in future games and is a combination of the AI of its parents. In this way it is possible to evolve better teams to increase your chances of winning Blockball."]
                 img [Src "img/evolve.svg"; Style [Width "32rem"]]
                 h1 [ClassName "section"] [str "Tournament Winner"]
                 p [] [str "Games of Blockball will run for a predetermined length of time. At the end of this period we will release 3 special limited edition Blockhead NFTs that will be given to the top 3 players in terms of win count."]
                 img [Src "img/winner.svg"; Style [Width "32rem"]]
                 h1 [ClassName "section"] [str "Ethos"]
                 p [] [str "We believe engineering and code is a form of art. No matter what happens in this space, we are passionate blockchain and technology enthusiasts."; br []; str "These NFTs reflect our values."]
                 img [Src "img/mechanism.png"; Style [Width "32rem"]]]]
         ]
