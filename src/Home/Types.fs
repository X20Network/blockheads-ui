module Home.Types

open Common

type Auction =
    { cubehead: Cubehead
      priceSold: decimal option }

type AuctionBatch =
    { auctions: Auction list
      timeRemaining: System.TimeSpan
      endTime: System.DateTime
      price: decimal
      priceRaw: string }

type Model =
    { currentAuction: AuctionBatch option }

type Msg =
    | ChangeStr of string
    | MintCubehead of int
    | MintSuccess of obj
    | MintFail of exn
