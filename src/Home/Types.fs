module Home.Types

open Common

type Auction =
    { cubehead: Cubehead
      priceSold: decimal option }

type AuctionBatch =
    { auctions: Auction list
      timeRemaining: System.TimeSpan
      endTime: System.DateTime
      price: decimal }

type Model =
    { currentAuction: AuctionBatch option }

type Msg =
    | ChangeStr of string
