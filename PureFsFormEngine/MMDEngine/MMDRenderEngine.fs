namespace MMDEng
open SharpDX
open System
open GameBaseLib
open System.Diagnostics
open System.Collections.Generic
open MMDEng.MMDRenderer
open MMDEng.MMDActor
open NCurGameEngine
open System.IO
open FormEng.ScriptLoader
open System.Windows.Forms
open SharpDX.Windows

module MMDRenderEngine =

  let debug s = Debug.WriteLine <| s + " logged"

  type FormHolder() = 
    let form = new MMDForm()
    let makeIRender (i:Actor) =
      let test = form.AddChara @"" 
      match i.RenderType with
        |Button ->
          new ModelActor( test.Value,i) :> AbstructActor
        |Window -> 
          new ModelActor( test.Value,i) :> AbstructActor
        |DebugDraw -> new ModelActor(test.Value, i) :> AbstructActor
    let makeActor (i:Actor) =
      i.Init i
      let actorControl = makeIRender i
      i.Render <- Some <|( actorControl.Render)
      actorControl

    let initScene() = 
      form.AddMouseCam()
      form.AddDefaultLight()
    let joystick = NCDInput.init()
    let mutable scriptChanged = None
    let mutable spawnedActor = []
    let commandQueue = new Queue<unit->unit>()
    let mayScr = 
      let mutable scr = None
      try
        let dir = Directory.GetParent( Reflection.Assembly.GetExecutingAssembly().Location).ToString()
        let scrPath = dir + (string Path.DirectorySeparatorChar) + "fsscr.fs"
        let loader = new ScriptLoader(scrPath)
        let changed () = 
          let init = loader.GameEnv 
          commandQueue.Enqueue(fun ()->
            form.ResetEnv()
            for (i:AbstructActor) in spawnedActor do
              i.Actor.Render <- None
            spawnedActor <- []
            initScene()
          )
          match init with
          |Some scr ->
            commandQueue.Enqueue(fun ()->
              scr.Func.Init()
            )
          |None -> ()
        scriptChanged <- Some changed
        // constructor
        changed()
        loader.AddScriptChangedHandler scriptChanged.Value
        scr <- Some <| loader
      with e ->
        Debug.WriteLine("script init failed")
        Debug.WriteLine(e)
      scr

    let onActorTick i input env=
      let update = Types.makeUpdate i input
      try
        i.Update update env
      with e ->
        Debug.WriteLine e
      if i.Render.IsSome then
        let ren = i.Render.Value
        ren.Draw ()
        ren.SetText i.Name
        for com in env.DebugCommand do
          match com with
          |Line line->
            //let color = line.Color
            //pen.Color <- Color.FromArgb(  color.A, color.R , color.G , color.B )
            //g.DrawLine(pen, v2ToPointF line.P1 , v2ToPointF line.P2)
          ()
      if i.Render.IsNone then
        let actor = makeActor i
        spawnedActor <- actor :: spawnedActor
    let ontick (e) = 
      if commandQueue.Count > 0 then 
        let command = commandQueue.Dequeue()
        command()

      let state = NCDInput.update(joystick)
      let dir = NCDInput.getDir state
      let mayActorList =
        Either.maybe{
          let! scr = mayScr
          let! env = scr.GameEnv
          let actList = env.ActorList
          return env,actList
        }
      if mayActorList.IsSome then
        let env,actorList = mayActorList.Value 
        let input = Types.makeInput dir state.Buttons
        for i in actorList do
          onActorTick i input env
        // heart beat
        Types.updateInput input
        env.Clean()
    do
        // こちらに初期化を移動させる 
        let onUpdate () =
          ontick null
          form.OnUpdate()
        let run e = 
          RenderLoop.Run(form , onUpdate)
        initScene()
        Application.Idle.Add run
        form.FormClosing.Add form.OnClose

        try 
          Application.Run(form)
          form.Dispose()
        with e-> 
          Debug.WriteLine e

