module Gallery.Types

open Common

type Msg =
    | ResetFilters
    | SelectTrait of string * (VisualTrait option)
    | SetSearch of string

type Model =
    { filter: Map<string, VisualTrait>
      idSearch: string }
