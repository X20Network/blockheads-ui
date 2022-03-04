module Cubeball.Types

open Common

type Tab =
    | AllCubeheads
    | Team
    | Results

type Msg =
    | SelectTab of Tab
    | SelectingCubehead of int
    | CancelSelectCubehead
    | SelectCubehead of int * int
    | CancelTeamChanges
    | SaveTeamChanges
    | SaveSucceeded
    | SaveFailed
    | SetCaptainState of int * bool

type Model =
    { activeTab: Tab
      cubeheads: WalletCubehead[] option
      team: (int * bool)[] option
      draftTeam: ((int * bool) option)[]
      selectingCubehead: int option
      results: (WalletResult list) option
      saving: bool }
