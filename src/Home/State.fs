module Home.State

open Elmish
open Types
open Common
open Cubehead

let exampleAuction =
    let now = System.DateTime.Now
    let endTime = now.Date + (System.TimeSpan(now.Hour + 1, 0, 0))
    { auctions =
        [ { cubehead = getRndCubehead rnd; priceSold = if rnd.Next(2) = 0 then None else Some 0.3M }
          { cubehead = getRndCubehead rnd; priceSold = if rnd.Next(2) = 0 then None else Some 0.3M }
          { cubehead = getRndCubehead rnd; priceSold = if rnd.Next(2) = 0 then None else Some 0.3M }
          { cubehead = getRndCubehead rnd; priceSold = if rnd.Next(2) = 0 then None else Some 0.3M }
          { cubehead = getRndCubehead rnd; priceSold = if rnd.Next(2) = 0 then None else Some 0.3M }
          { cubehead = getRndCubehead rnd; priceSold = if rnd.Next(2) = 0 then None else Some 0.3M } ]
      endTime = endTime
      timeRemaining = endTime - now
      price = 1.0M }

let init () : Model * Cmd<Msg> =
  { currentAuction = Some exampleAuction }, []

let update msg model : Model * Cmd<Msg> =
    match msg with
    | ChangeStr str ->
        model, Cmd.none
