module UserGuide.View

open Fable.React
open Fable.React.Props

let midnightLocalStr = System.DateTime(2022, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime().ToShortTimeString()

let middayLocalStr = System.DateTime(2022, 1, 1, 12, 0, 0, System.DateTimeKind.Utc).ToLocalTime().ToShortTimeString()

let drawBlockballDiag (squares: (int * int * bool) list[][]) =
    let mapPlayer (p, t, b) =
        span [classList ["blue", t = 0; "red", t = 1]] [str <| p.ToString(); ofOption <| if b then Some <| span [ClassName "ball"] [] else None]
    let pitchSquare r c =
        td [ClassName "pitch"] [div [] (squares.[r].[c] |> List.map mapPlayer)]
    div [ClassName "blockball-diagram-container"]
        [table [ClassName "blockball-diagram"]
            [tbody []
                [tr []
                    [td [] []; pitchSquare 0 1; td [] []]
                 tr [] [pitchSquare 1 0; pitchSquare 1 1; pitchSquare 1 2]
                 tr [] [pitchSquare 2 0; pitchSquare 2 1; pitchSquare 2 2]
                 tr [] [td [] []; pitchSquare 3 1; td [] []]]]]

let startingPositionsDiag =
    [| [| []; [(1, 0, true)]; [] |]
       [| [(4, 0, false)]; [(3, 0, false)]; [(2, 0, false)] |]
       [| [(2, 1, false)]; [(3, 1, false)]; [(4, 1, false)] |]
       [| []; [(1, 1, false)]; [] |] |]
            |> drawBlockballDiag

let root =
    div []
        [div [ClassName "block section content"]
            [div [ClassName "box"]
                [div [ClassName "section content"]
                    [h1 [ClassName "title"] [str "How to get a Blockhead"]
                     p [] [b [] [str "The Minting Process"]] 
                     p [] [str "Blockheads will be minted in batches of 6 in a Dutch auction. Minting fees will start at 0.2ETH and gradually get lower until each Blockhead has sold. Once the entire batch has sold, another batch will be released at the starting price and the process will repeat for all 1995 available Blockheads."]
                     p [ClassName "has-text-centered"]
                        [img [ClassName "guide"; Src "/img/mintingprocess.png"]]
                     p [] [b [] [str "Secondary Markets"]]
                     p [] [str "Blockheads will also be available on NFT marketplaces. This is the only place to get one after all 1995 of them have been minted."]
                     p [] []]]]
         div [ClassName "block section content"]
            [div [ClassName "box"] [div [ClassName "section content"]
                [h1 [ClassName "title"] [str "How to Play Blockball"]
                 p [] [b [] [str "Create a Team"]]
                 p [] [str "Blockball is an AI powered, soccer inspired team game that runs entirely on the Ethereum blockchain. To play, you need to own at least 4 Blockheads or Blocklets (Blocklets are mini Blockheads that are awarded as prizes to winning teams) with each team having at least one proper Blockhead."]
                 p [ClassName "has-text-centered"]
                    [img [ClassName "guide"; Src "/img/chooseteam.png"]]
                 p [] [str "Go to the "; b [] [str "Team"]; str " tab of the Blockball page to select your team. You may have to sign a message using your wallet provider (for example Metamask) to verify your identity for when we fetch your team later."]
                 p [] [str "After selecting your 4 Blockheads for the different starting positions, choose 2 of them to be 'parents' should your team win. These blockheads will be bred together using an evolutionary algorithm to create a Blocklet with a combination of their physical and strategy traits. In this way you can evolve better players to increase your chances of winning."]
                 p [] [str "Once your have your chosen team, click "; b [] [str "Save"]; str " in order to finalise it on the blockchain. You will need to sign a transaction with your wallet provider. "]
                 p [ClassName "has-text-centered"]
                    [img [Style [Width "unset"];ClassName "guide"; Src "/img/confirmteam.png"]]
                 p [] [b [] [str "The Match Schedule"]]
                 p [] [str <| "Matches are automatically played every day at midnight GMT (" + midnightLocalStr + " your local time). Your opposing team will be selected at random. Changes to teams should be submitted by midday GMT (" + middayLocalStr + " your local time) for matches that evening. If changes are submitted after this time (but before that day's game) they will be reflected in the game the following day. "]
                 p [] [str "If you have already chosen your team and do not wish to change it then you do not have to do anything. Your team will play a match every day until the end of the game period."]
                 ]]]
         div [ClassName "block section content"]
            [div [ClassName "box"] [div [ClassName "section content"]
                [h1 [ClassName "title"] [str "Blockball Results"]
                 p [] [b [] [str "Viewing your Results"]]
                 p [] [str "To view your results go to the "; b [] [str "Results"]; str " tab of the Blockball page."]
                 p [ClassName "has-text-centered"]
                    [img [ClassName "guide"; Src "/img/result.png"]]
                 p [] [b [] [str "NFT Trophy"]]
                 p [] [str "The winner of the match receives an NFT trophy that contains a unique animated replay of the blockball game that was played. Like Blockheads themselves, this NFT is entirely generated on the Blockchain itself."]
                 p [] [b [] [str "Blocklet Prize"]]
                 p [] [str "The winner of the match also receives a Blocklet NFT. This blocklet is bred from the selected parents of the winning team and contains a genetic combination of those parents' physical and strategy traits. "]]]]
         div [ClassName "block section content"]
             [div [ClassName "box"] [div [ClassName "section content"]
                 [h1 [ClassName "title"] [str "Blockball Game Logic"]
                  p [] [b [] [str "The Pitch"]]
                  p [] [str "Blockball is played on a pitch arranged into 8 squares. There are 4 players per team, with each player starting in a fixed position as depicted below. At the first kickoff a random team is chosen to start with the ball."]
                  startingPositionsDiag
                  p [] [b [] [str "Player Actions"]]]]]
         div [ClassName "block section content"]
             [div [ClassName "box"] [div [ClassName "section content"]
                 [h1 [ClassName "title"] [str "Blockhead Traits"]]]]
         ]
