module BlockballLogic

open Imports

module Util =

    let swap x y (a: 'a []) =
        let tmp = a.[x]
        a.[x] <- a.[y]
        a.[y] <- tmp

    let shuffle (rnd :System.Random) a =
        let b = a |> Array.copy
        Array.iteri (fun i _ -> a |> swap i (rnd.Next(i, Array.length a))) b
        b

type Player =
    { tackleStrength: int
      interceptStrength: int
      possStrength: int
      passStrength: int
      moveWithoutBallDistanceGoal: int
      moveWithoutBallDistanceBall: int
      moveWithoutBallNumAttackers: int
      moveWithoutBallNumDefenders: int
      moveWithBallDistanceGoal: int
      moveWithBallNumAttackers: int
      moveWithBallNumDefenders: int
      passDistanceGoal: int
      passNumAttackers: int
      passNumDefenders: int
      passInvPass: int
      passInvPoss: int
      passNotRequest: int
      shootAlways: bool }

type GameState =
    { teams: Player[,]
      positions: (int)[][]
      ballPlayerTeam: int
      ballPlayerIndex: int
      ballPos: int
      ballPlayerNextPosition: int option
      tackleLastRound: bool
      score: int[] }

type GameStep =
    | Tackle of (int * int) * (int * int)
    | Move of (int * int) * int * int
    | Stay of (int * int)
    | DribbleStart of (int * int) * int * int
    | DribbleEnd of (int * int) * int * int
    | Goal of (int * int)
    | Miss of (int * int)
    | Pass of (int * int) * (int * int)
    | PassIntercept of (int * int) * (int * int) * (int * int)
    | RequestPass of (int * int)

let neighbours =
    [| [| 1; 2; 3 |]
       [| 0; 2; 4 |]
       [| 0; 1; 3; 5 |]
       [| 0; 2; 6 |]
       [| 1; 5; 7 |]
       [| 2; 4; 6; 7 |]
       [| 3; 5; 7 |]
       [| 4; 5; 6 |]|]

let isNeighbour square1 square2 =
    neighbours.[square1] |> Array.contains square2

let getDistance square1 square2 =
    if square1 = square2 then 0 else
        if isNeighbour square1 square2 then 1 else
            if ((square1 = 0 && square2 = 7) || (square1 = 7 && square2 = 0)) then 3 else 2

let getInitState (teams :Player[,]) (rnd :System.Random) =
    let ballTeam = rnd.Next(2)
    { teams = teams
      ballPlayerTeam = ballTeam
      ballPlayerIndex = 0
      ballPos = if ballTeam = 0 then 0 else 7
      ballPlayerNextPosition = None
      tackleLastRound = false
      score = [| 0; 0 |]
      positions =
        [| [| 0; 1; 2; 3 |]
           [| 7; 6; 5; 4 |] |] }

module GameState =

    let containsPlayer team square state =
        state.positions.[team] |> Array.exists (fun s -> s = square)

    let countPlayers team square state =
        state.positions.[team] |> Array.filter (fun s -> s = square) |> Array.length

    let getSumTackle team square state =
        state.positions.[team] |> Array.mapi (fun p s -> if s = square then state.teams.[team, p].tackleStrength else 0) |> Array.sum

    let getSumPass team square state =
        state.positions.[team] |> Array.mapi (fun p s -> if s = square then state.teams.[team, p].passStrength else 0) |> Array.sum

    let getSumIntercept team square state =
        state.positions.[team] |> Array.mapi (fun p s -> if s = square then state.teams.[team, p].interceptStrength else 0) |> Array.sum

let step1 (rnd: IBN) state =
    let oppTeam = if state.ballPlayerTeam = 0 then 1 else 0
    let ballPlayer = state.teams.[state.ballPlayerTeam, state.ballPlayerIndex]
    // tackles first
    if (not state.tackleLastRound) &&
       (state |> GameState.countPlayers oppTeam state.ballPos > 0) &&
       (state |> GameState.getSumTackle oppTeam state.ballPos >= ballPlayer.possStrength) then
       failwith "ni"
    else
        ()
    failwith "ni"

//let step (rnd :System.Random) logStep state =
//    let oppTeam = if state.ballTeam = 0 then 1 else 0
//    match state with
//    | { tackleLastRound = false }       // tackle
//        when
//            state |> GameState.containsPlayer oppTeam state.ballPos &&
//            state |> GameState.getSumTackle oppTeam state.ballPos >= (fst state.players.[state.ballTeam].[state.ballPlayer]).possStrength &&
//            rnd.Next(2) = 0 ->
//        let (maxTacklePlayer, _) =
//            state.players.[oppTeam]
//                |> Array.mapi (fun i (p, s) -> i, p, s)
//                |> Array.fold (fun (mt, mti) (i, p, s) ->
//                    if s = state.ballPos && p.tackleStrength > mt then (p.tackleStrength, i) else (mt, mti))
//                (-1, -1)
//        logStep <| Tackle((state.ballTeam, state.ballPlayer), (oppTeam, maxTacklePlayer))
//        { state with tackleLastRound = true; ballTeam = oppTeam; ballPlayer = maxTacklePlayer; ballPlayerNextPosition = None }
//    | _ ->
//        let goal = if state.ballTeam = 0 then 7 else 0
//        let nplayers =
//            state.players |> Array.mapi (fun t ps -> ps |> Array.mapi (fun p (player, s) ->
//                if t = state.ballTeam && p = state.ballPlayer then
//                    match state with
//                    | { ballPlayerNextPosition = Some nextPosition } ->
//                        logStep <| DribbleEnd ((state.ballTeam, state.ballPlayer), state.ballPos, nextPosition)
//                        player, nextPosition
//                    | _ ->
//                        player, s
//                else
//                    let getWeightingForSquare square =
//                        let numAttackers =
//                            let num = state |> GameState.countPlayers state.ballTeam square
//                            if state.ballTeam = t then num else 3 - num
//                        let numDefenders = state |> GameState.countPlayers oppTeam square
//                        let sum =
//                            (getDistance state.ballPos square) * player.moveWithoutBallDistanceBall +
//                            (getDistance goal square) * player.moveWithoutBallDistanceGoal +
//                            numAttackers * player.moveWithoutBallNumAttackers +
//                            numDefenders * player.moveWithoutBallNumDefenders
//                        sum
//                    let minPos =
//                        Array.append (neighbours.[s] |> Util.shuffle rnd) [| s |] |> Array.minBy getWeightingForSquare
//                    if s = minPos then
//                        logStep <| Stay (t, p)
//                    else
//                        logStep <| Move ((t, p), s, minPos)
//                    player, minPos
//                ))
//        let ballPlayer, _ = state.players.[state.ballTeam].[state.ballPlayer]
//        match state with
//        | { ballPlayerNextPosition = Some nextPosition } ->
//            { state with ballPlayerNextPosition = None; ballPos = nextPosition; players = nplayers }
//        | { ballTeam = 0; ballPos = 7} | { ballTeam = 1; ballPos = 0 } ->
//            let sc = 4 - (state |> GameState.getSumPass state.ballTeam state.ballPos) + (state |> GameState.getSumIntercept oppTeam state.ballPos)
//            let shotChance = if sc < 1 then 1 else sc
//            if ballPlayer.shootAlways || shotChance <= 3 then
//                let initState = getInitState state.teams rnd
//                if rnd.Next(shotChance) = 0 then
//                    // goal
//                    let score = if state.ballTeam = 0 then [| state.score.[0] + 1; state.score.[1] |] else [| state.score.[0]; state.score.[1] + 1 |]
//                    logStep <| Goal (state.ballTeam, state.ballPlayer)
//                    { initState with score = score }
//                else
//                    // miss
//                    logStep <| Miss (state.ballTeam, state.ballPlayer)
//                    { initState with score = state.score }
//            else
//                { state with players = nplayers }
//        | _ ->
//            // dribble or pass

//            // calculate pass requests first
//            let numAttackersBall = state |> GameState.countPlayers state.ballTeam state.ballPos
//            let numDefendersBall = state |> GameState.countPlayers oppTeam state.ballPos
//            let distGoalBall = getDistance state.ballPos goal
//            let invPassBall = 3 - ballPlayer.passStrength
//            let invPossBall = 3 - ballPlayer.possStrength
//            let requestPass =
//                nplayers.[state.ballTeam] |> Array.map (fun (p, s) ->
//                    let invPass = 3 - p.passStrength
//                    let invPoss = 3 - p.possStrength
//                    let ballPosSum =
//                        distGoalBall * p.passDistanceGoal +
//                        numAttackersBall * p.passNumAttackers +
//                        numDefendersBall * p.passNumDefenders +
//                        invPassBall * p.passInvPass +
//                        invPossBall * p.passInvPoss
//                    let playerPosSum =
//                        (getDistance goal s) * p.passDistanceGoal +
//                        (state |> GameState.countPlayers state.ballTeam s) * p.passNumAttackers +
//                        (state |> GameState.countPlayers oppTeam s) * p.passNumDefenders +
//                        invPass * p.passInvPass +
//                        invPoss * p.passInvPoss
//                    playerPosSum < ballPosSum)
            
//            let getMoveWeightingForSquare square =
//                let goal = if state.ballTeam = 0 then 7 else 0
//                let numAttackers = state |> GameState.countPlayers state.ballTeam square
//                let numDefenders = state |> GameState.countPlayers oppTeam square
//                ((getDistance goal square) * ballPlayer.moveWithBallDistanceGoal +
//                 numAttackers * ballPlayer.moveWithBallNumAttackers +
//                 numDefenders * ballPlayer.moveWithBallNumDefenders) * 2
//            let getPassWeightingForPlayer playerIndex (player, square) =
//                let goal = if state.ballTeam = 0 then 7 else 0
//                let numAttackers = state |> GameState.countPlayers state.ballTeam square
//                let numDefenders = state |> GameState.countPlayers oppTeam square
//                let invPass = 3 - player.passStrength
//                let invPoss = 3 - player.possStrength
//                let notRequestedPass = if requestPass.[playerIndex] then 0 else 1
//                let distGoal = getDistance goal square
//                distGoal * ballPlayer.passDistanceGoal +
//                numAttackers * ballPlayer.passNumAttackers +
//                numDefenders * ballPlayer.passNumDefenders +
//                invPass * ballPlayer.passInvPass +
//                invPoss * ballPlayer.passInvPoss +
//                notRequestedPass * ballPlayer.passNotRequest
//            let moveOptions =
//                neighbours.[state.ballPos] |> Array.map (fun s -> Choice1Of2 s, getMoveWeightingForSquare s) |> Util.shuffle rnd
//            let passOptions =
//                [| for i in 0..3 do
//                    if i <> state.ballPlayer then yield i |]
//                        |> Array.map (fun i ->
//                            let p, s = nplayers.[state.ballTeam].[i]
//                            Choice2Of2 i, getPassWeightingForPlayer i (p, s)) |> Util.shuffle rnd
//            let movePassOptions = Array.append passOptions moveOptions
//            let allOptions = Array.append movePassOptions [| Choice1Of2 state.ballPos, getMoveWeightingForSquare state.ballPos |]
//            let minOpt, _ = allOptions |> Array.minBy snd
//            match minOpt with
//            | Choice1Of2 moveSquare ->
//                { state with players = nplayers; ballPlayerNextPosition = Some moveSquare }
//            | Choice2Of2 passPlayerIndex ->
//                // check for interception
//                let maxInterceptPlayerIndex, (maxInterceptPlayer, maxInterceptPos) = state.players.[oppTeam] |> Array.mapi (fun i p -> i, p) |> Array.maxBy (fun (_, (p, _)) -> p.interceptStrength)
//                if maxInterceptPlayer.interceptStrength >= ballPlayer.passStrength then
//                    // interception
//                    { state with players = nplayers; ballTeam = oppTeam; ballPlayer = maxInterceptPlayerIndex; ballPlayerNextPosition = None; ballPos = maxInterceptPos }
//                else
//                    // no interception
//                    { state with players = nplayers; ballPlayer = passPlayerIndex; ballPos = nplayers.[state.ballTeam].[passPlayerIndex] |> snd }
