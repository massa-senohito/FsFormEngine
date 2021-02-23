namespace GameBaseLib
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
    mutable Pos : Vector2
    mutable Scale : Vector2
    mutable Render:IRender option
    Init : Actor -> unit
    Update : UpdateInfo -> GameEnv -> unit
    ChildList : Actor list
    mutable RenderType : RenderType
    OnClick : EventHandler
  }
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
    let pos = a.Pos
    a.Pos.X <- pos.X + x
    a.Pos.Y <- pos.Y + y
  let addScale (a:Actor) x y = 
    let sca = a.Scale
    a.Scale.X <- sca.X + x
    a.Scale.Y <- sca.Y + y

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
    {Name = name ; Pos = makeVec x y ; Scale = makeVec 40.0f 40.0f ; Init = init ; Update = nullUpdate ;
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
