#r "GameBaseLib.dll"
open GameBaseLib
open System
open Types
let initAsWindow self=
  self.RenderType <- Window

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

let rand = new Random()
let flRand() = rand.NextDouble() * 400.0 |> float32
let ac3 x y = makeActor "spawnedActor1" x y init nullUpdate
let playerUpdate updInfo env =
  let inputb = updInfo.Input
  let x = inputb.X * 5.0f
  let y = inputb.Y * 5.0f
  let pos = updInfo.Self.Pos
  let select = isPressedThisFrame 2 updInfo.Input
  //if isPressedThisFrame 2 updInfo.Input then
  //  updInfo.Self.Name <- getPage ()
  if inputb.Buttons.[1] then
    updInfo.Self.Pos.X <- pos.X + 1.0f
  //addPos self x y
  if isAxisYThisFrame updInfo.Input then
    //yPos <- yPos + inputb.Y
    //updInfo.Self.Name <- string yPos
    //Debug.WriteLine("thisFrame")
    addActor env <| ac3 (flRand()) (flRand())
  addScale updInfo.Self x y

let ac1 = makeActor "actor1" 125.0f 111.0f initAsWindow playerUpdate
let mutable ac2 = None
let oncl s e =
  ac2.Value.Name <- "cled"
ac2 <- Some <| makeClickableActor "actor2" 240.0f 243.0f ignore nullUpdate oncl

let initScr =
  let acList = [ac1;ac2.Value]
  acList
let initWorld w = ()
let nullWorldUpd e = ()
makeWorld initScr [] initWorld nullWorldUpd

