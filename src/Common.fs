module Common

open Fable.Core
open Fable.Core.JsInterop

type ChainConfig =
    { infuraUrl: string
      chainId: int
      chainName: string
      moralisChainName: string
      moralisApiKey: string
      cubeheadsContractAddress: string}

module Config =

    let private kovan =
      { infuraUrl = "https://kovan.infura.io/v3/2ba89d75124a4e0baf363346d70820fb"
        chainId = 42
        chainName = "Kovan"
        moralisChainName = "kovan"
        moralisApiKey = "jpdexOGpbd1eIulwVUTz5Y1LB1dJow8ApkDhv6YukusfUIFLmVfCzagT4Dv8buWg"
        cubeheadsContractAddress = "0x87007Dde4fa46733c21c7889Fc31ddf8921c2C56" }

    let network = kovan

module Moralis =

    open Fable.SimpleHttp
    open Fable.SimpleJson

    type PagedResult<'t> =
        { total: int
          page: int
          page_size: int
          cursor: string
          result: 't[] }

    type NFT =
        { token_id: int
          owner_of: string
          token_uri: string
          symbol: string }

    let getNFTs tokenAddress account :PagedResult<NFT> Async =
        let url = sprintf "https://deep-index.moralis.io/api/v2/%s/nft?chain=kovan&format=decimal&token_addresses=%s" account tokenAddress
        async {
            let! response =
                Http.request url
                    |> Http.method GET
                    |> Http.header (Headers.accept "application/json")
                    |> Http.header (Headers.create "X-API-Key" Config.network.moralisApiKey)
                    |> Http.send
            return Json.parseAs response.responseText
        }

type AccountData =
    { selectedAccount: string
      chainId: int }

type CubeheadAttribute =
    { trait_type: string
      value: string }

type CubeheadData =
    { name: string
      description: string
      external_url: string
      attributes: CubeheadAttribute[]
      data: string }

let cubeheadsData : CubeheadData[] = import "cubeheads" "./data.js"

let cubeheadsDataByIndex =
    let arr :CubeheadData[] = Array.zeroCreate cubeheadsData.Length
    cubeheadsData |> Array.iter (fun cubehead ->
        let index = cubehead.name.Substring(10) |> System.Int32.Parse
        arr.[index] <- cubehead)
    arr

let buildCubeheadsMerkleTree web3 =
    let cubeheads =
        cubeheadsData |> Array.choose (fun c ->
           let index = c.name.Substring(10) |> System.Int32.Parse
           if index > 1994 then None
           else Some (index, c.data))
           |> Array.sortBy fst
           |> Array.map snd
    let rec create (l::ls) =
        match l |> Array.length with
        | 1 -> l::ls
        | n ->
            let n2 = n / 2 + n % 2
            (Array.init n2 (fun i ->
                let h1 = l.[i * 2]
                let h2 = let index = i * 2 + 1 in if index >= n then "0x0" else l.[index]
                let hash :string = web3?utils?soliditySha3(createObj ["type", !!"bytes32"; "value", !!h1], createObj ["type", !!"bytes32"; "value", !!h2])
                hash))::(l::ls) |> create
    create [cubeheads] |> List.skip 1 |> List.rev |> List.toArray

let getMerklePath (tree :string[][]) index =
    tree |> Array.mapi (fun i hashes ->
        let i2 = index / (1 <<< i)
        let i3 = if i2 % 2 = 0 then i2 + 1 else i2 - 1
        hashes.[i3])

let traitsData :obj = import "traits" "./data.js"

let traits :Map<string, Map<string, int>> =
    ["Type", traitsData?Type |> Array.map (fun vc -> vc?value, vc?count) |> Map.ofArray
     "Face", traitsData?Face |> Array.map (fun vc -> vc?value, vc?count) |> Map.ofArray
     "Side", traitsData?Side |> Array.map (fun vc -> vc?value, vc?count) |> Map.ofArray
     "Top", traitsData?Top |> Array.map (fun vc -> vc?value, vc?count) |> Map.ofArray
     "Eyes", traitsData?Eyes |> Array.map (fun vc -> vc?value, vc?count) |> Map.ofArray
     "Mouth", traitsData?Mouth |> Array.map (fun vc -> vc?value, vc?count) |> Map.ofArray]
        |> Map.ofList

[<Emit("parseInt($0, 16)")>]
let parseHex (str: string) : int = jsNative

let rnd = new System.Random()

let inline refEquals a b = obj.ReferenceEquals(unbox a, unbox b)

type date = System.DateTime

type KeyedItem<'t> =
    { Key: string; Item: 't }

type VisualTrait = string * string

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

    let getFromHexString (str :string) =
        let i = parseHex (str.Substring(50, 16))
        let wbmDistGoal = (i >>> 8) &&& 7
        let wbmNumAtck = (i >>> 11) &&& 7
        let wbmNumDef = (i >>> 14) &&& 7
        let wbpDistGoal = (i >>> 17) &&& 7
        let wbpNumAtck = (i >>> 20) &&& 7
        let wbpNumDef = (i >>> 23) &&& 7
        let wbpInvPos = (i >>> 26) &&& 7
        let wbpInvPas = (i >>> 29) &&& 7
        let wbpNotReq = (i >>> 32) &&& 7
        let ipDistBall = (i >>> 35) &&& 3
        let ipDistGoal = (i >>> 37) &&& 3
        let ipNumAtck = (i >>> 39) &&& 3
        let ipNumDef = (i >>> 41) &&& 3
        let opDistBall = (i >>> 43) &&& 3
        let opDistGoal = (i >>> 45) &&& 3
        let opNumAtck = (i >>> 47) &&& 3
        let opNumDef = (i >>> 49) &&& 3
        let shoot = (i >>> 51) &&& 1
        { score = 0.0
          shooting = match shoot with | 0 -> ShootAlways | 1 -> WaitForOpportunity
          weightings =
              [WithBallMove,
                  [DistanceGoal, wbmDistGoal
                   NumAttackers, wbmNumAtck
                   NumDefenders, wbmNumDef] |> Map.ofList
               WithBallPass,
                  [DistanceGoal, wbpDistGoal
                   NumAttackers, wbpNumAtck
                   NumDefenders, wbpNumDef
                   InvPossStrength, wbpInvPos
                   InvPassStrength, wbpInvPas
                   PassNotRequested, wbpNotReq] |> Map.ofList
               InPoss,
                  [DistanceGoal, ipDistGoal
                   DistanceBall, ipDistBall
                   NumAttackers, ipNumAtck
                   NumDefenders, ipNumDef] |> Map.ofList
               OutPoss,
                  [DistanceGoal, opDistGoal
                   DistanceBall, opDistBall
                   NumAttackers, opNumAtck
                   NumDefenders, opNumDef] |> Map.ofList]
                  |> Map.ofList |> getNormalised }

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

    let fromCubeheadData (data :CubeheadData) =
        let i = parseHex (data.data.Substring(50, 16))
        { name = data.name
          index = data.name.Substring(10) |> System.Int32.Parse
          visualTraits =
            data.attributes
                |> Array.map (fun a -> a.trait_type, a.value)
                |> Array.toList
                |> List.filter (fun (t, _) ->
                    match t with
                    | "Accuracy" -> false
                    | "Agility" -> false
                    | "Speed" -> false
                    | "Strength" -> false
                    | _ -> true)
          strategy = Strategy.getFromHexString data.data
          strength = i &&& 3
          speed = (i >>> 2) &&& 3
          agility = (i >>> 4) &&& 3
          accuracy = (i >>> 6) &&& 3
          svg = "/img/cubehead-svgs/" + data.data.Substring(24, 10).ToUpper() + ".svg" }

    let getRndCubehead (rnd :Random) =
        let index = rnd.Next(1998)
        { svg = ExampleCubehead.svg
          name = sprintf "Cubehead #%i"index
          index = index
          visualTraits =
            traits |> Map.toList |> List.map (fun (trait, values) ->
                let vs = values |> Map.toArray
                let v, _ = vs.[rnd.Next(vs.Length)]
                trait, v)
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

    let getRarity (trait, value) =
        match traits |> Map.tryFind trait with
        | Some values ->
            match values |> Map.tryFind value with
            | Some count -> (float count) / 1995.0
            | None -> -1.0
        | None -> -1.0

    let getTraitTypeName (t, _) = t

    let getTraitName (_, v) = v

    let getCubeheadsForAccount cubeletsTokenAddress account =
        async {
            let! cubeheads = Moralis.getNFTs Config.network.cubeheadsContractAddress account
            let! cubelets = Moralis.getNFTs cubeletsTokenAddress account
            return 0
        }

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

    let allCubeheads = cubeheadsData |> Array.map fromCubeheadData |> Array.filter (fun c -> c.index < 1995) |> Array.toList |> List.sortBy (fun c -> c.index)

    let allCubeheadsByIndex = allCubeheads |> List.map (fun c -> c.index, c) |> Map.ofList

    let getAllCubeheads filter search =
        allCubeheads |> List.filter (fun cubehead ->
            (match search with None -> true | Some sindex -> cubehead.index = sindex) && (filter |> List.forall (fun f -> cubehead.visualTraits |> List.contains f)))

    let getAllCubeheadsPaged filter search =
        getAllCubeheads filter search |> List.chunkBySize 27

    let getCubeheadByIndex index = allCubeheadsByIndex |> Map.find index
