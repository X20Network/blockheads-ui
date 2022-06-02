module Gallery.Types

open Common

type Msg =
    | ResetFilters
    | SelectTrait of string * (VisualTrait option)
    | SetSearch of string
    | LoadPage

type Model =
    { filter: Map<string, VisualTrait>
      idSearch: string
      blockheads: Blockhead list list
      filteredBlockheads: Blockhead list list }
