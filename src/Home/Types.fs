module Home.Types

open Common

type Auction =
    { blockhead: Blockhead
      priceSold: decimal option
      minting: string option }

type AuctionBatch =
    { auctions: Auction list
      timeRemaining: System.TimeSpan
      endTime: System.DateTime
      repeatTimeSecs: int
      price: decimal
      priceRaw: string }

type Model =
    { currentAuction: AuctionBatch option }

type Msg =
    | ChangeStr of string
    | MintBlockhead of int
    | MintSuccess of int * obj
    | MintFail of int * exn
