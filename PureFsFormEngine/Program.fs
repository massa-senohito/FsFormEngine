namespace FormEng
module Main =
  open System.Windows.Forms
  open System
  open System.Collections.Generic
  open System.Drawing
  open NCurGameEngine
  open System.Diagnostics
  open FormUtil
  type StartupForm() as this = 
    inherit Form()
    let button1 = makeButton 1 4 40 40
    let textBox = makeTextbox 1 80 90 40
    let addControl c = addControl this.Controls c
    let mutable components = null;
    let timer = makeTimer 16
    let mutable timerHa = null
    let mutable scriptChanged = None
    let joystick = NCDInput.init()
    let commandQueue = new Queue<unit->unit>()
    let scr = 
      let mutable scr = None
      try  
        let loader = new ScriptLoader.ScriptLoader("fsscr.fs")
        let changed () = 
          let init = loader.CallInit() 
          match init with
          |Some scr ->
            let f = scr
            commandQueue.Enqueue(fun ()->
              button1.Text <- string f
              )
          |None -> ()
        scriptChanged <- Some changed
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
      match scr with
      |Some scr ->
        let f = scr.CallUpdate()
        textBox.Text <- string f.Value
      |None -> ()
      let state = NCDInput.update(joystick)
      if NCDInput.isXMin state then
        FormUtil.setOffset button1 -1 0 
      if NCDInput.isXMax state then
        FormUtil.setOffset button1 1 0 
      if NCDInput.isYMin state then
        FormUtil.setOffset button1 0 -1 
      if NCDInput.isYMax state then
        FormUtil.setOffset button1 0 1 
    do
        components <- new ComponentModel.Container( );
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
        addControl button1
        addControl textBox
        addControl playerui
        addControl ui
        this.ResumeLayout(false)
        this.PerformLayout()

  [<EntryPoint; STAThread>]
  let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    use form = new StartupForm()
    Application.Run(form)   
    0 // return an integer exit code
