module Cubehead.Types

open Global
open Common

type Model =
    { previousPage: Page option
      cubehead: Cubehead option }

type Msg =
    | PreviousPage
