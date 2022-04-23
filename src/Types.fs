module App.Types

open Global
open Imports
open Common

type WalletData =
    { cubeheads: WalletCubehead[]
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
    | CubeballMsg of Cubeball.Types.Msg
    | CubeheadMsg of Cubehead.Types.Msg
    | GalleryMsg of Gallery.Types.Msg
    | SetProvider of obj
    | SetAccountData of AccountData
    | SetWalletData of WalletData
    | SetWeb3InitData of Web3InitData
    | SetAuctionPrice of string * decimal
    | SetSignedMessage of string
    | SetTeam of (int * bool)[]
    | SetCubehead of Cubehead
    | MintEvent of obj
    | AuctionEvent of obj
    | SetAuction of Home.Types.AuctionBatch
    | TimerTick

type GlobalObjs =
    { web3Modal: Web3Modal
      window: obj
      mutable web3: Web3
      mutable contracts: Contracts option
      cubeheadsMerkleTree: string[][] }

type Model =
    { CurrentPage: Page
      Counter: Counter.Types.Model
      Home: Home.Types.Model
      cubeball: Cubeball.Types.Model
      accountData: AccountData option
      cubehead: Cubehead.Types.Model option
      gallery: Gallery.Types.Model }
