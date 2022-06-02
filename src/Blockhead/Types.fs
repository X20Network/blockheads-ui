module Blockhead.Types

open Global
open Common

type Model =
    { previousPage: Page option
      blockhead: Blockhead option }

type Msg =
    | PreviousPage
