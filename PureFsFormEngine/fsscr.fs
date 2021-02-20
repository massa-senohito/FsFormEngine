#r "GameBaseLib.dll"
open GameBaseLib
open Types
let init self=
  self.RenderType <- Button
let playerUpdate input =
  let x = input.Input.X * 5
  let y = input.Input.Y * 5
  addPos input.Self x y

let ac1 = makeActor "we2" 125 111 init playerUpdate
let ac2 = makeFixedActor "we3" 240 243

let initScr =
  let acList = [ac1;ac2]
  acList

let initWorld () =
  ()
let update input =
  ()
let mutable cnt = 10000
let upd () = 
  cnt <- cnt - 1
  //cnt
  //hoge
  []
let updData = ("update" , upd)
makeWorld initScr [] initWorld update
