#r "GameBaseLib.dll"
open GameBaseLib
open System
open Types
let initAsWindow self=
  self.RenderType <- DebugDraw

let mutable cnt = 0
let texts =
  [
    "hoge"
    "fuga"
    "ho"
  ]
let getPage ()=
  let cur = texts.[cnt]
  cnt <- cnt + 1
  let len = List.length texts
  if cnt = len then
    cnt <- 0
  cur

let mutable yPos = 0.0f
let speed = 
  //0.01f // mmd
  5.0f // form
let rand = new Random()
let flRand() = rand.NextDouble() * 400.0 |> float32
let ac3 x y = makeActor "spawnedActor2" x y init nullUpdate
let playerUpdate updInfo env =
  let inputb = updInfo.Input
  let x = inputb.X * speed
  let y = inputb.Y * speed
  let pos = updInfo.Self.Pos
  let select = isPressedThisFrame 2 updInfo.Input
  //if isPressedThisFrame 2 updInfo.Input then
  //  updInfo.Self.Name <- getPage ()
  if inputb.Buttons.[1] then
    addRot updInfo.Self 1.0f
    let line = debugDrawLine debugRed (flRand()) (flRand()) (flRand()) (flRand())
    env.DebugCommand <- line :: env.DebugCommand
  if inputb.Buttons.[3] then
    addScale updInfo.Self 1.0f 1.0f
  if isAxisYThisFrame updInfo.Input then
    //yPos <- yPos + inputb.Y
    //updInfo.Self.Name <- string yPos
    addActor env <| ac3 (flRand()) (flRand())
    ()
  addPos updInfo.Self x y

//let p1x,p1y = 125.0f , 111.0f
let p1x,p1y = 0.0f , 0.0f
let mutable ac1 = makeActor "actor1" p1x p1y initAsWindow playerUpdate
ac1.Scale <- SharpDXUtil.makeVec 1.0f 1.0f
let mutable ac2 = None
let oncl s e =
  ac2.Value.Name <- "cled"
//let p2x,p2y = 240.0f , 243.0f
let p2x,p2y = 21.0f , 1.0f
ac2 <- Some <| makeClickableActor "actor2" p2x p2y ignore nullUpdate oncl
ac2.Value.Scale <- SharpDXUtil.makeVec 1.0f 1.0f

let initScr =
  let acList = [ac1;ac2.Value]
  acList
let initWorld w = ()
let nullWorldUpd e = ()
makeWorld initScr [] initWorld nullWorldUpd

