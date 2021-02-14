namespace FormEng
module ScriptLoader=
  open System
  open System.IO
  type ScriptLoader(fileName) as t =
    let mutable funcUpdate = None
    let mutable funcInit = None
    let initEngine() =
      let evalRes = new ScrEngine.FSTypeInfo.EvalResult()
      let file = File.ReadAllText(fileName)
      evalRes.Eval(file)
      let v = evalRes.Result.Value
      match v.ReflectionValue with
      | :? FSharp.Collections.Map<string,FSharpFunc<unit,int>> as v -> 
        funcInit <- Some v.["init"]
        funcUpdate <- Some v.["update"]
    let onUpdateScript (e:FileSystemEventArgs) = initEngine()
    let watcher = 
      let w = new FileSystemWatcher()
      w.Path <- "."
      w.Filter <- "*.fs"
      w.EnableRaisingEvents <- true
      w
    do
      watcher.Changed.Add onUpdateScript
      initEngine()

    member t.CallInit() =
      match funcInit with
      | Some fu-> Some <|fu()
      | None   -> None
    member t.CallUpdate() =
      match funcUpdate with
      | Some fu-> Some <|fu()
      | None   -> None
    member t.AddScriptChangedHandler (f:unit->unit) =
      watcher.Changed.Subscribe(fun e -> f())

