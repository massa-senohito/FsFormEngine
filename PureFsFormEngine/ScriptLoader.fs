namespace FormEng
module ScriptLoader=
  open System.Diagnostics
  open System.IO
  open GameBaseLib
  open System.Windows.Forms
  type ScriptLoader(fileName) as t =
    let mutable gameEnv = None
    let initEngine() =
      let evalRes = new ScrEngine.FSTypeInfo.EvalResult()
      let file = File.ReadAllText(fileName)
      evalRes.Eval(file)
      let v = evalRes.Result
      match v with
      |Some v ->
        match v.ReflectionValue with
        | :? GameEnv as v -> 
          // todo initはゲーム処理へ
          v.Func.Init()
          gameEnv <- Some v
          //funcUpdate <- Some v.["update"]
      |None -> //Debug.WriteLine 
        let err = evalRes.Error |> Array.fold(fun a i -> a + "\n" + i.ToString()) ""
        // todo textboxに書く
        //MessageBox.Show( err ) |> ignore
        Debug.WriteLine err
    let onUpdateScript (e:FileSystemEventArgs) = initEngine()
    let watcher = 
      let w = new FileSystemWatcher()
      // todo fileNameの親フォルダに
      w.Path <- "."
      w.Filter <- "*.fs"
      w.EnableRaisingEvents <- true
      w
    do
      watcher.Changed.Add onUpdateScript
      initEngine()

    member t.GameEnv = gameEnv
    member t.AddScriptChangedHandler (f:unit->unit) =
      watcher.Changed.Subscribe(fun e -> f())

