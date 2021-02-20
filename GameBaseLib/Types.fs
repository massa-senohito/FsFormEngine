namespace GameBaseLib
type Input =
  {
    X : int
    Y : int
  }

type IRender =
  abstract member Draw : x:int -> y:int -> unit

type Actor =
  {
    Name : string
    mutable X : int
    mutable Y : int
    mutable Render:IRender option
    Init : Actor -> unit
    Update : UpdateInfo -> unit
    ChildList : Actor list
    mutable RenderType : RenderType
  }
and UpdateInfo =
  {
    Self : Actor
    Input : Input
    ActorList : Actor list
    WindowList : Window list
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
type FuncInfo =
  {
    Init   : unit -> unit
    Update : UpdateInfo -> unit
  }
type GameEnv =
  {
    ActorList  : Actor list
    WindowList : Window list
    Func      : FuncInfo
  }
module Types=
  let init self=
    ()
  let addPos (a:Actor) x y = 
    a.X <- a.X + x
    a.Y <- a.Y + y
  let controllerUpdate (input:UpdateInfo) =
    addPos input.Self input.Input.X input.Input.Y
    ()
  let update (input:UpdateInfo) =
    ()
  let onInput input =
    ()
  let makeFixedActor name x y = 
    {Name = name ; X = x ; Y = y ; Init = init ; Update = update ;
     Render = None ; ChildList = [] ; RenderType = Button}
  let makeControllableActor name x y = { makeFixedActor name x y with Init = init ; Update = controllerUpdate}
  let makeActor name x y ini upd = { makeFixedActor name x y with Init = ini ; Update = upd}
  let makeTextBox name x y = {Text = "" ; Actor = makeFixedActor name x y; OnInput = onInput}
  let makeWorld actorList windowList init upd = {ActorList = actorList ; WindowList = windowList ; Func = {Init = init ;Update = upd} }
  let makeInput self x y actorList windowList = { Self = self ; Input = {X = x ; Y = y} ; ActorList = actorList ; WindowList = windowList }
