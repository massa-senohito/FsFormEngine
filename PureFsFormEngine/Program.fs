namespace FormEng
module Main =
  open System.Windows.Forms
  open System
  open FormActor
  open System.Collections.Generic
  open System.Drawing
  open NCurGameEngine
  open System.Diagnostics
  open FormUtil
  open GameBaseLib
  type StartupForm() as this = 
    inherit Form()
    let button1 = makeButton 1 4 40 40
    let textBox = makeTextbox 1 80 90 40
    let addControl c = addControl this.Controls c
    let mutable components = null;
    let timer = makeTimer 16
    let mutable timerHa = null
    let mutable scriptChanged = None
    let mutable spawnedActor = []
    let joystick = NCDInput.init()
    let commandQueue = new Queue<unit->unit>()
    let scr = 
      let mutable scr = None
      try  
        let loader = new ScriptLoader.ScriptLoader("fsscr.fs")
        let changed () = 
          let init = loader.GameEnv 
          match init with
          |Some scr ->
            commandQueue.Enqueue(fun ()->
              scr.Func.Init()
              this.SuspendLayout();
              for (i) in spawnedActor do
                this.Controls.Remove(i)
                Debug.WriteLine(i.Name + "will remove")
              this.ResumeLayout(false)
              this.PerformLayout()
            )
            commandQueue.Enqueue(fun ()->
              this.SuspendLayout();
              for i in scr.ActorList do
                i.Init i
                let control = 
                  match i.RenderType with
                  |Button ->
                    let c = makeButton i.X i.Y 40 40 
                    c:> Control
                  |Window -> 
                    let c = makeTextbox i.X i.Y 80 80
                    c:> Control
                i.Render <- Some <|(new ActorControl(control) :> IRender)
                addControl control
                control.Text <- i.Name
                Debug.WriteLine(i.Name + " will add")
                spawnedActor <- control :: spawnedActor

              this.ResumeLayout(false)
              this.PerformLayout()
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

    let ontick (e) = 
      let state = NCDInput.update(joystick)
      let mutable xdir = 0
      let mutable ydir = 0
      if NCDInput.isXMin state then
        xdir <- -1 
      if NCDInput.isXMax state then
        xdir <- 1 
      if NCDInput.isYMin state then
        ydir <- -1 
      if NCDInput.isYMax state then
        ydir <- 1 

      let actorList = 
        match scr with
        |Some scr -> match scr.GameEnv with |Some env->env.ActorList | None -> []
        |None ->[]
      for i in actorList do
        // buttonも渡してアクセスできるようにするか
        i.Update(Types.makeInput i xdir ydir actorList [])
        if i.Render.IsSome then
          i.Render.Value.Draw i.X i.Y
      if commandQueue.Count > 0 then 
        let command = commandQueue.Dequeue()
        command()
      match scr with
      |Some scr ->
        let f = scr.GameEnv
        if f.IsSome then
          textBox.Text <- string f.Value
      |None -> ()
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
        //addControl button1
        //addControl textBox
        //addControl playerui
        //addControl ui
        this.ResumeLayout(false)
        this.PerformLayout()

  [<EntryPoint; STAThread>]
  let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    use form = new StartupForm()
    Application.Run(form)   
    0 // return an integer exit code
