namespace FormEng
module FormActor =
  open GameBaseLib
  open System.Windows.Forms
  open FormUtil
  type ActorControl (c:Control) =
    interface IRender with
       member this.Draw x y = c.Location <-makePoint x y

