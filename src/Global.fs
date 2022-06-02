module Global

type Page =
    | Home
    | Counter
    | About
    | Blockball
    | Whitepaper
    | Blockhead of int
    | Gallery
    | UserGuide

let toHash page =
    match page with
    | About -> "/about"
    | Counter -> "/counter"
    | Home -> "/home"
    | Blockball -> "/blockball"
    | Whitepaper -> "/whitepaper"
    | Gallery -> "/gallery"
    | UserGuide -> "/guide"
    | Blockhead index -> "/blockhead/" + index.ToString()
