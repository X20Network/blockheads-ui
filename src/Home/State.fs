module Home.State

open Elmish
open Types
open Common
open Blockhead

//let exampleAuction =
//    let now = System.DateTime.Now
//    let endTime = now.Date + (System.TimeSpan(now.Hour + 1, 0, 0))
//    { auctions =
//        [ { blockhead = getRndBlockhead rnd; priceSold = (if rnd.Next(2) = 0 then None else Some 0.3M); minting = None }
//          { blockhead = getRndBlockhead rnd; priceSold = (if rnd.Next(2) = 0 then None else Some 0.3M); minting = None }
//          { blockhead = getRndBlockhead rnd; priceSold = (if rnd.Next(2) = 0 then None else Some 0.3M); minting = None }
//          { blockhead = getRndBlockhead rnd; priceSold = (if rnd.Next(2) = 0 then None else Some 0.3M); minting = None }
//          { blockhead = getRndBlockhead rnd; priceSold = (if rnd.Next(2) = 0 then None else Some 0.3M); minting = None }
//          { blockhead = getRndBlockhead rnd; priceSold = (if rnd.Next(2) = 0 then None else Some 0.3M); minting = None } ]
//      endTime = endTime
//      timeRemaining = endTime - now
//      price = 1.0M
//      priceRaw = ""}

let init () : Model * Cmd<Msg> =
  { currentAuction = None }, []

let update msg model : Model * Cmd<Msg> =
    match msg with
    | ChangeStr str ->
        model, Cmd.none
