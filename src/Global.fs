module Global

type Page =
    | Home
    | Counter
    | About
    | Cubeball
    | Whitepaper
    | Cubehead of string
    | Gallery

let toHash page =
    match page with
    | About -> "#about"
    | Counter -> "#counter"
    | Home -> "#home"
    | Cubeball -> "#cubeball"
    | Whitepaper -> "#whitepaper"
    | Gallery -> "#gallery"
    | Cubehead name -> "#" + name
