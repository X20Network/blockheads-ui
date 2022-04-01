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
      cubeheads: Cubehead list list
      filteredCubeheads: Cubehead list list }
