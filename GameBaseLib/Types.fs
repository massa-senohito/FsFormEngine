﻿namespace GameBaseLib
open System
open System.Diagnostics
open SharpDXUtil
open SharpDX
type f32 = float32
type Input =
  {
    X : f32
    Y : f32
    Buttons : bool array
  }
type IRender =
  abstract member Draw : unit -> unit
  abstract member SetText : string -> unit
//type Component ()=
  

type Actor =
  {
    mutable Name : string
    //mutable Pos : Vector2
    //mutable Scale : Vector2
    mutable Mat : Matrix
    mutable Render:IRender option
    Init : Actor -> unit
    Update : UpdateInfo -> GameEnv -> unit
    ChildList : Actor list
    mutable RenderType : RenderType
    OnClick : EventHandler
  }
  member t.Pos3 = t.Mat.TranslationVector
  member t.Scale3 = t.Mat.ScaleVector
  member t.Pos = makeVec t.Pos3.X t.Pos3.Y
  member t.Scale = makeVec t.Scale3.X t.Scale3.Y
  member t.SetPos x y =
    t.Mat.TranslationVector <-
      makeV3 x y 0.0f
  member t.AddPos x y =
    t.Mat.TranslationVector <- t.Pos3 + 
      makeV3 x y 0.0f
  member t.AddScale x y =
    //Debug.Assert( t.Scale.X > 0.0f)
    // https://forums.cgsociety.org/t/getting-negative-scale-from-matrix3/1879261/2
    t.Mat.ScaleVector <- absV3 t.Mat.ScaleVector
    t.Mat.ScaleVector <- t.Scale3 + 
      makeV3 x y 0.0f
    Debug.Assert( t.Scale.X > 0.0f)
  member t.AddRot z =
    let preScale =  t.Scale.X 
    t.Mat <- Matrix.RotationZ z * t.Mat

  member t.Rot = Quaternion.RotationMatrix t.Mat
  // pitch yaw roll
  member t.EulerRot = 
    let rot = t.Rot
    let X,Y,Z,W = rot.X,rot.Y,rot.Z,rot.W 
    let ww = W * W;
    let xx = X * X;
    let yy = Y * Y;
    let zz = Z * Z;
    let Singularity = 0.499f
    let lengthSqd = xx + yy + zz + ww;
    let singularityTest = Y * W - X * Z;
    let singularityValue = Singularity * lengthSqd;
    if singularityTest > singularityValue
      then new Vector3(-2.0f * atan2 Z W , 90.0f, 0.0f)
      else if singularityTest < -singularityValue
        then new Vector3(2.0f * atan2 Z  W , -90.0f, 0.0f)
        else new Vector3(
                    atan2 (2.0f * (Y * Z + X * W)) (1.0f - 2.0f * (xx + yy)) ,
                    asin(2.0f * singularityTest / lengthSqd),
                    atan2(2.0f * (X * Y + Z * W)) (1.0f - 2.0f * (yy + zz)))

and UpdateInfo =
  {
    Self : Actor
    Input : Input
  }
and Window =
  {
    Text : string
    Actor : Actor
    OnInput : string -> unit
  }
and RenderType =
  |Button
  |Window
  |DebugDraw

and GameEnv =
  {
    mutable ActorList  : Actor list
    WindowList : Window list
    Func      : FuncInfo
    mutable IsDirty : bool
  }
  member t.AddActor a =
    t.ActorList <- a :: t.ActorList
  member t.Clean() =
    t.IsDirty <- false

and FuncInfo =
  {
    Init   : unit -> unit
    Update : UpdateInfo -> unit
  }

type DebugColor =
  {
    R : int
    G : int
    B : int
    A : int
  }
type DebugLine =
  {
    Color : DebugColor
    P1 : Vector2
    P2 : Vector2
  }

module Types=
  let init self=
    ()
  let addPos (a:Actor) x y = 
    a.AddPos x y
  let addScale (a:Actor) x y = 
    a.AddScale x y
  let addRot (a:Actor) z = 
    a.AddRot z

  let controllerUpdate (input:UpdateInfo) env =
    addPos input.Self input.Input.X input.Input.Y
    ()
  let nullUpdate (input:UpdateInfo) env =
    ()
  let onInput input =
    ()
  let nullClicked =
    new EventHandler( fun s e ->())

  let makeFixedActor name x y = 
    {Name = name ; Mat = makeMtx ( makeV3Z x y ) (makeV3Z 40.0f 40.0f ) (Quaternion.Identity); Init = init ; Update = nullUpdate ;
     Render = None ; ChildList = [] ; RenderType = Button ; OnClick = nullClicked}
  let makeControllableActor name x y = { makeFixedActor name x y with Init = init ; Update = controllerUpdate}
  let makeClickableActor name x y ini upd cl = { makeFixedActor name x y with Init = ini ; Update = upd ; OnClick = new EventHandler( cl)}
  let makeActor name x y ini upd = { makeFixedActor name x y with Init = ini ; Update = upd}
  let makeTextBox name x y = {Text = "" ; Actor = makeFixedActor name x y; OnInput = onInput}
  let makeWorld actorList windowList init upd = {ActorList = actorList ; WindowList = windowList ; Func = {Init = init ;Update = upd} ; IsDirty = false}
  let makeInput (dir:Vector2) buttons = {X = dir.X ; Y = dir.Y ; Buttons = buttons}
  let makeUpdate self update = { Self = self ; Input = update ; }
  let addActor (w:GameEnv) a =
    w.IsDirty <- true
    w.AddActor a
  let mutable buttons = [||]
  let mutable stick = 0.0f , 0.0f
  let updateInput input =
    buttons <- input.Buttons
    stick <- input.X , input.Y
  let isPressedThisFrame i (input:Input) =
    input.Buttons.[i] && not buttons.[i]
  let isAxisXThisFrame (input:Input) =
    input.X <> fst stick

  let isAxisYThisFrame (input:Input) =
    input.Y <> snd stick

  // envに突っ込むか
  // 通常actorはDebugDraw
  let debugDrawLine color x1 y1 x2 y2 =
    { Color = color ; P1 = makeVec x1 y1 ; P2 = makeVec x2 y2  }
