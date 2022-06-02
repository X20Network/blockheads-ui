module Blockball.Types

open Common

type Tab =
    | AllBlockheads
    | Team
    | Results

type Msg =
    | SelectTab of Tab
    | SelectingBlockhead of int
    | CancelSelectBlockhead
    | SelectBlockhead of int * int
    | CancelTeamChanges
    | SaveTeamChanges
    | CommitTeamSucceeded of string * int[] * obj * string
    | CommitTeamFailed of exn
    | SaveSucceeded
    | SaveFailed
    | SetCaptainState of int * bool
    | SignMessage

type Model =
    { activeTab: Tab
      blockheads: WalletBlockhead[] option
      team: (int * bool)[] option
      draftTeam: ((int * bool) option)[]
      selectingBlockhead: int option
      results: (WalletResult list) option
      saving: bool
      messageToSign: string option }
