module App.Types

open Global
open Imports
open Common

type WalletData =
    { blockheads: WalletBlockhead[]
      team: (int * bool)[]
      teamFetched: bool
      results: WalletResult list
      messageToSign: string }

type Web3InitData =
    { auction: Home.Types.AuctionBatch
      auctionBlock: int64 }

type Msg =
    | CounterMsg of Counter.Types.Msg
    | HomeMsg of Home.Types.Msg
    | NavMsg of Navbar.Types.Msg
    | BlockballMsg of Blockball.Types.Msg
    | BlockheadMsg of Blockhead.Types.Msg
    | GalleryMsg of Gallery.Types.Msg
    | SetProvider of obj
    | SetAccountData of AccountData
    | SetWalletData of WalletData
    | SetWeb3InitData of Web3InitData
    | SetAuctionPrice of string * decimal
    | SetSignedMessage of string
    | SetTeam of (int * bool)[]
    | SetBlockhead of Blockhead
    | MintEvent of obj
    | AuctionEvent of obj
    | SetAuction of Home.Types.AuctionBatch
    | TimerTick

type GlobalObjs =
    { web3Modal: Web3Modal
      window: obj
      mutable web3: Web3
      mutable contracts: Contracts option
      blockheadsMerkleTree: string[][] }

type Model =
    { CurrentPage: Page
      Counter: Counter.Types.Model
      Home: Home.Types.Model
      blockball: Blockball.Types.Model
      accountData: AccountData option
      blockhead: Blockhead.Types.Model option
      gallery: Gallery.Types.Model
      navbarMenuActive: bool
      timeToLaunch: int * int * int * int
      carousel: int }
