module Global

type Page =
    | Home
    | Counter
    | About
    | Cubeball
    | Whitepaper
    | Cubehead of int
    | Gallery
    | UserGuide

let toHash page =
    match page with
    | About -> "/about"
    | Counter -> "/counter"
    | Home -> "/home"
    | Cubeball -> "/cubeball"
    | Whitepaper -> "/whitepaper"
    | Gallery -> "/gallery"
    | UserGuide -> "/guide"
    | Cubehead index -> "/cubehead/" + index.ToString()
