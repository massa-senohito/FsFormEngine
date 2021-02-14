open System.Diagnostics
let init () = 123
let upd () = 42
Map.ofList [("update" , upd);("init", init)]
