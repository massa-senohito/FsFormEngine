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
  let makeIRender(i:Actor) g pen =
    let x = i.Pos.X |> int
    let y = i.Pos.Y |> int
    match i.RenderType with
    |Button ->
      let c = makeButton x y 40 40 
      c.Click.AddHandler i.OnClick
      new ActorControl( c,i) :> AbstructControl
    |Window -> 
      let c = makeTextbox x y 80 80
      new ActorControl( c,i) :> AbstructControl
    |DebugDraw -> new DebugActor(g , pen , i) :> AbstructControl

  let makeActor (i:Actor) g pen =
    i.Init i
    let actorControl = makeIRender i g pen
    i.Render <- Some <|( actorControl.Render)
    actorControl

  let debug s = Debug.WriteLine <| s + " logged"

  type StartupForm() as this = 
    inherit Form()
    let addControl (c:Control) =
      debug (" addcontrol " + c.Name)
      addControl this.Controls c
    let mutable components = null;
    let timer = makeTimer 33
    let mutable timerHa = null
    let mutable scriptChanged = None
    let mutable spawnedActor = []
    let joystick = NCDInput.init()
    let mutable (g:Graphics) = null
    let pen = makePen Color.Black 2.0f
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
              for (i:AbstructControl) in spawnedActor do
                //this.Controls.Remove(i.Control)
                i.RemoveControl this
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

    let onActorTick i input env=
      // buttonも渡してアクセスできるようにするか
      let update = Types.makeUpdate i input
      try
        i.Update update env
      with e ->
        Debug.WriteLine e
      if i.Render.IsSome then
        let ren = i.Render.Value
        pen.Color <- Color.Black
        ren.Draw ()
        ren.SetText i.Name
        for com in env.DebugCommand do
          match com with
          |Line line->
            let color = line.Color
            pen.Color <- Color.FromArgb(  color.A, color.R , color.G , color.B )
            g.DrawLine(pen, v2ToPointF line.P1 , v2ToPointF line.P2)
      if i.Render.IsNone then
        let actorControl = makeActor i g pen
        actorControl.AddControl this
        spawnedActor <- actorControl :: spawnedActor

    let ontick (e) = 
      if commandQueue.Count > 0 then 
        let command = commandQueue.Dequeue()
        command()

      let state = NCDInput.update(joystick)
      let mutable dir = Vector2.Zero
      if NCDInput.isLeft state then
        dir.X <- -1.0f
      if NCDInput.isRight state then
        dir.X <- 1.0f
      if NCDInput.isUp state then
        dir.Y <- -1.0f
      if NCDInput.isDown state then
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
          onActorTick i input env
        // heart beat
        Types.updateInput input
        env.Clean()
        //this.Invalidate()

    let mutable pos = makePoint 10 10
    do
        components <- new ComponentModel.Container( );
        // こちらに初期化を移動させる 
        this.SetStyle(ControlStyles.DoubleBuffer ||| ControlStyles.UserPaint ||| ControlStyles.AllPaintingInWmPaint , true)
        //this.Load
        this.SuspendLayout();
        this.AutoScaleMode <- AutoScaleMode.None;
        this.ClientSize <- new Size( 800 , 450 );
        this.Text <- "Form1";
        g <- this.CreateGraphics()
        timerHa <- timer.Tick.Subscribe ontick
        this.Closing.Add(fun e->timerHa.Dispose())
        let playerui = new HPWindow.HPWindow("player",30,30)
        FormUtil.setPos playerui 100 150
        let ui = new HPWindow.HPWindow("player2",30,30)
        FormUtil.setPos ui 250 150
        this.ResumeLayout(false)
        this.PerformLayout()
    override t.OnPaint (e:PaintEventArgs) =
      // todo e.Graphicsで描画するように変える
      base.OnPaint e
      g.Clear(this.BackColor)
      //let brush = new SolidBrush(Color.Black);
      //e.Graphics.DrawLine(pen, pos , makePoint 130 40)
      //pos <- makePoint (pos.X + 1) (pos.Y)
      //for i in spawnedActor do
      //    let ren = i.Render
      //    ren.Draw ()
      //    ren.SetText i.Actor.Name


  [<EntryPoint; STAThread>]
  let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    let useMMD = false
    if(useMMD) then
      let f = new MMDEng.MMDRenderEngine.FormHolder()
      ()
    else
      use form = new StartupForm()
      Application.Run(form)   
    0 // return an integer exit code
