module App.Types

open Global
open Imports
open Common

type AccountData =
    { selectedAccount: string }

type WalletData =
    { cubeheads: WalletCubehead[]
      team: (int * bool)[]
      results: WalletResult list }

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
    | TimerTick

type GlobalObjs =
    { web3Modal: Web3Modal
      window: obj }

type Model =
    { CurrentPage: Page
      Counter: Counter.Types.Model
      Home: Home.Types.Model
      cubeball: Cubeball.Types.Model
      accountData: AccountData option
      cubehead: Cubehead.Types.Model option
      gallery: Gallery.Types.Model }
