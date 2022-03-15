module Common

open Fable.Core

let rnd = new System.Random()

let inline refEquals a b = obj.ReferenceEquals(unbox a, unbox b)

type date = System.DateTime

type KeyedItem<'t> =
    { Key: string; Item: 't }

type CubeType =
    | GreyWithOrangeFace
    | BlueWithRedFace
    | Camo
    | Psychedelic
    | GradientGrey
    | GradientGold
    | Wireframe
    | Zombie
    | Alien
    | Steampunk

type SideDetail =
    | Titanium
    | Silicon
    | Aluminium
    | CircuitBoard
    | Headphones

type Eyes =
    | Normal
    | Visor
    | RedEye
    | Cyclops
    | Rainbow
    | CyclopsVisor
    | Glasses3D
    | Sunglasses

type HeadDetail =
    | Plain
    | Mohawk
    | Electronic

type MouthDetail =
    | Smile
    | SmileWithCigarette
    | Straight
    | StraightWithCigarette

type VisualTrait =
    CubeType of CubeType
  | SideDetail of SideDetail
  | Eyes of Eyes
  | HeadDetail of HeadDetail
  | MouthDetail of MouthDetail

type StrategyWeighingType =
    | DistanceGoal
    | DistanceBall
    | NumAttackers
    | NumDefenders
    | InvPossStrength
    | InvPassStrength
    | PassNotRequested

type StrategyType =
    | WithBallMove
    | WithBallPass
    | InPoss
    | OutPoss

type ShootingStrategy =
    | ShootAlways
    | WaitForOpportunity

type Strategy =
    { weightings: Map<StrategyType, Map<StrategyWeighingType, float>>
      shooting: ShootingStrategy
      score: float }

module Strategy =

    let getNormalised weightings =
        let sumwb =
            (weightings |> Map.find WithBallMove |> Map.toSeq |> Seq.sumBy (fun (_, w) -> float <| 8 - w)) +
            (weightings |> Map.find WithBallPass |> Map.toSeq |> Seq.sumBy (fun (_, w) -> float <| 8 - w))
        let sumip = weightings |> Map.find InPoss |> Map.toSeq |> Seq.sumBy (fun (_, w) -> float <| 4 - w)
        let sumop = weightings |> Map.find OutPoss |> Map.toSeq |> Seq.sumBy (fun (_, w) -> float <| 4 - w)
        weightings
            |> Map.map (fun st ws ->
                let sum, max =
                    match st with
                    | WithBallPass | WithBallMove -> sumwb, 8
                    | InPoss -> sumip, 4
                    | OutPoss -> sumop, 4
                ws |> Map.map (fun _ w -> (float <| max - w) / sum))

type Cubehead =
    { svg: string
      name: string
      index: int
      visualTraits: VisualTrait list
      strategy: Strategy
      strength: int
      speed: int
      agility: int
      accuracy: int }

type TeamStatus =
    | NotInTeam
    | InTeam
    | InTeamBreeding

type WalletCubehead =
    { cubehead: Cubehead
      teamStatus: TeamStatus }

type WalletResult =
    { userTeam: Cubehead[]
      oppTeam: Cubehead[]
      score: int * int
      date: date
      childCube: Cubehead
      trophySrc: string }

module Cubehead =

    open System

    let getRndCubeType (rnd :Random) =
        match rnd.Next(10) with
        | 0 -> GreyWithOrangeFace
        | 1 -> BlueWithRedFace
        | 2 -> Camo
        | 3 -> Psychedelic
        | 4 -> GradientGrey
        | 5 -> GradientGold
        | 6 -> Wireframe
        | 7 -> Zombie
        | 8 -> Alien
        | 9 -> Steampunk

    let getRndCubehead (rnd :Random) =
        let index = rnd.Next(1998)
        { svg = ExampleCubehead.svg
          name = sprintf "Cubehead #%i"index
          index = index
          visualTraits =
            [CubeType (getRndCubeType rnd)
             SideDetail Titanium
             Eyes RedEye
             HeadDetail Mohawk
             MouthDetail SmileWithCigarette]
          strategy =
            { score = rnd.NextDouble()
              shooting = match rnd.Next(2) with | 0 -> ShootAlways | 1 -> WaitForOpportunity
              weightings =
                [WithBallMove,
                    [DistanceGoal, rnd.Next(8)
                     NumAttackers, rnd.Next(8)
                     NumDefenders, rnd.Next(8)] |> Map.ofList
                 WithBallPass,
                    [DistanceGoal, rnd.Next(8)
                     NumAttackers, rnd.Next(8)
                     NumDefenders, rnd.Next(8)
                     InvPossStrength, rnd.Next(8)
                     InvPassStrength, rnd.Next(8)
                     PassNotRequested, rnd.Next(8)] |> Map.ofList
                 InPoss,
                    [DistanceGoal, rnd.Next(4)
                     DistanceBall, rnd.Next(4)
                     NumAttackers, rnd.Next(4)
                     NumDefenders, rnd.Next(4)] |> Map.ofList
                 OutPoss,
                    [DistanceGoal, rnd.Next(4)
                     DistanceBall, rnd.Next(4)
                     NumAttackers, rnd.Next(4)
                     NumDefenders, rnd.Next(4)] |> Map.ofList]
                    |> Map.ofList |> Strategy.getNormalised }
          strength = rnd.Next(4)
          speed = rnd.Next(4)
          agility = rnd.Next(4)
          accuracy = rnd.Next(4) }

    let getRarity =
        function
        | CubeType _ -> 0.06
        | SideDetail _ -> 0.11
        | Eyes _ -> 0.7
        | HeadDetail _ -> 0.21
        | MouthDetail _ -> 0.17

    let getTraitTypeName =
        function
        | CubeType _ -> "Cube type"
        | SideDetail _ -> "Side detail"
        | Eyes _ -> "Eyes"
        | HeadDetail _ -> "Head detail"
        | MouthDetail _ -> "Mouth"

    let getTraitName =
        function
        | CubeType cubeType ->
            match cubeType with
            | GreyWithOrangeFace -> "Grey w orange face"
            | BlueWithRedFace -> "Blue w red face"
            | Camo -> "Camo"
            | Psychedelic -> "Psychedelic"
            | GradientGrey -> "Gradient grey"
            | GradientGold -> "Gradient gold"
            | Wireframe -> "Wireframe"
            | Zombie -> "Zombie"
            | Alien -> "Alien"
            | Steampunk -> "Steampunk"
        | SideDetail sideDetail ->
            match sideDetail with
            | Titanium -> "Titanium element"
            | Silicon -> "Silicon"
            | Aluminium -> "Aluminium"
            | CircuitBoard -> "Circuitboard"
            | Headphones -> "Headphones"
        | Eyes eyes ->
            match eyes with
            | RedEye -> "Red eye"
            | Normal -> "Normal"
            | Visor -> "Visor"
            | Cyclops -> "Cyclops"
            | Rainbow -> "Rainbow"
            | CyclopsVisor -> "Cyclops Visor"
            | Glasses3D -> "3D Glasses"
            | Sunglasses -> "Sunglasses"
        | HeadDetail headDetail ->
            match headDetail with
            | Mohawk -> "Mohawk"
            | Plain -> "Plain"
            | Electronic -> "Electronic"
        | MouthDetail mouthDetail ->
            match mouthDetail with
            | SmileWithCigarette -> "Smile w cigarette"
            | Smile -> "Smile"
            | Straight -> "Straight"
            | StraightWithCigarette -> "Straight w cigarette"

    let getCubeheadByName name =
        { getRndCubehead rnd with name = name }

    let getCubeheadsForAccount account =
        let n = rnd.Next(160)
        Array.init n (fun _ -> { cubehead = getRndCubehead rnd; teamStatus = NotInTeam }), [||]

    let getResultsForAccount account =
        let n = rnd.Next(32)
        List.init n (fun _ ->
            let trophyIndex = rnd.Next(6) + 1
            { trophySrc = "/img/trophies/trophy" + (trophyIndex.ToString()) + ".svg"
              userTeam = [| getRndCubehead rnd; getRndCubehead rnd; getRndCubehead rnd; getRndCubehead rnd |]
              oppTeam = [| getRndCubehead rnd; getRndCubehead rnd; getRndCubehead rnd; getRndCubehead rnd |]
              score = rnd.Next(5), rnd.Next(5)
              date = date.Now.AddDays(float <| -rnd.Next(60))
              childCube = getRndCubehead rnd })
              |> List.sortByDescending (fun r -> r.date)

    let allCubeheads = List.init 400 (fun _ -> getRndCubehead rnd)

    let getAllCubeheads filter search =
        allCubeheads |> List.filter (fun cubehead ->
            (cubehead.name.Contains(search) || search = "") && (filter |> List.forall (fun f -> cubehead.visualTraits |> List.contains f)))
