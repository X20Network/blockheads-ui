module Cubehead.Types

open Global
open Common

type Model =
    { previousPage: Page option
      cubehead: Cubehead }

type Msg =
    | PreviousPage
