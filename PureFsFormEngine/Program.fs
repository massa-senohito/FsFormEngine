namespace FormEng

open SharpDX

module Main =
  open System.Windows.Forms
  open System
  open System.IO
  open FormActor
  open System.Collections.Generic
  open System.Drawing
  open NCurGameEngine
  open System.Diagnostics
  open FormUtil
  open GameBaseLib
  let makeActor (i:Actor) =
    i.Init i
    let x = i.Pos.X |> int
    let y = i.Pos.Y |> int
    let c,control = match i.RenderType with
    |Button ->
      let c = makeButton x y 40 40 
      c.Click.AddHandler i.OnClick
      c :> Control,AbstructControl.Button c
    |Window -> 
      let c = makeTextbox x y 80 80
      c :> Control,AbstructControl.TextBox c
    let actorControl = new ActorControl(control, i)
    i.Render <- Some <|( actorControl :> IRender)
    actorControl.Control.Text <- i.Name
    actorControl

  let debug s = Debug.WriteLine <| s + " logged"

  type StartupForm() as this = 
    inherit Form()
    let button1 = makeButton 1 4 40 40
    let textBox = makeTextbox 1 80 90 40
    let addControl (c:Control) =
      debug (" addcontrol " + c.Name)
      addControl this.Controls c
    let mutable components = null;
    let timer = makeTimer 16
    let mutable timerHa = null
    let mutable scriptChanged = None
    let mutable spawnedActor = []
    let joystick = NCDInput.init()
    let commandQueue = new Queue<unit->unit>()
    let mayScr = 
      let mutable scr = None
      try
        let dir = Directory.GetParent( Reflection.Assembly.GetExecutingAssembly().Location).ToString()
        let scrPath = dir + (string Path.DirectorySeparatorChar) + "fsscr.fs"
        let loader = new ScriptLoader.ScriptLoader(scrPath)
        let changed () = 
          let init = loader.GameEnv 
          match init with
          |Some scr ->
            commandQueue.Enqueue(fun ()->
              scr.Func.Init()
              this.SuspendLayout();
              for (i:ActorControl) in spawnedActor do
                this.Controls.Remove(i.Control)
                debug(i.Actor.Name + "will remove")
                let d = i :> IDisposable
                d.Dispose()
              spawnedActor <- []
              this.ResumeLayout(false)
              this.PerformLayout()
            )
            //commandQueue.Enqueue(fun ()->
            //  this.SuspendLayout();
            //  for i in scr.ActorList do
            //    let actorControl = makeActor i
            //    addControl actorControl.Control
            //    debug(i.Name + " will add")
            //    spawnedActor <- actorControl :: spawnedActor
            //  this.ResumeLayout(false)
            //  this.PerformLayout()
            //)
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

    let ontick (e) = 
      if commandQueue.Count > 0 then 
        let command = commandQueue.Dequeue()
        command()

      let state = NCDInput.update(joystick)
      let mutable dir = Vector2.Zero
      if NCDInput.isXMin state then
        dir.X <- -1.0f
      if NCDInput.isXMax state then
        dir.X <- 1.0f
      if NCDInput.isYMin state then
        dir.Y <- -1.0f
      if NCDInput.isYMax state then
        dir.Y <- 1.0f

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
          // buttonも渡してアクセスできるようにするか
          let update = Types.makeUpdate i input
          try
            i.Update update env
          with e ->
            Debug.WriteLine e
          if i.Render.IsSome then
            let x = i.Pos.X
            let y = i.Pos.Y
            let w = i.Scale.X
            let h = i.Scale.Y
            let ren = i.Render.Value
            ren.Draw x y w h
            ren.SetText i.Name
          if i.Render.IsNone then
            let actorControl = makeActor i
            addControl actorControl.Control
            spawnedActor <- actorControl :: spawnedActor
        // heart beat
        Types.updateInput input
        env.Clean()

    do
        components <- new ComponentModel.Container( );
        // こちらに初期化を移動させる 
        //this.Load
        this.SuspendLayout();
        this.AutoScaleMode <- AutoScaleMode.None;
        this.ClientSize <- new Size( 800 , 450 );
        this.Text <- "Form1";

        button1.Name <- "button1"
        timerHa <- timer.Tick.Subscribe ontick
        this.Closing.Add(fun e->timerHa.Dispose())
        let playerui = new HPWindow.HPWindow("player",30,30)
        FormUtil.setPos playerui 100 150
        let ui = new HPWindow.HPWindow("player2",30,30)
        FormUtil.setPos ui 250 150
        this.ResumeLayout(false)
        this.PerformLayout()

  [<EntryPoint; STAThread>]
  let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    use form = new StartupForm()
    Application.Run(form)   
    0 // return an integer exit code
