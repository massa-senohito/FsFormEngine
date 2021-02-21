namespace FormEng
module FormActor =
  open GameBaseLib
  open System.Windows.Forms
  open System
  open FormUtil
  type AbstructControl =
  |Button of Button
  |TextBox of TextBox

  type ActorControl (c:AbstructControl ,a:Actor)=
    member this.Control =
      match c with
      |Button  b ->b :> Control
      |TextBox b ->b :> Control
    member this.Actor =
      a
    interface IDisposable with
      member this.Dispose() =
        this.Control.Click.RemoveHandler a.OnClick
    interface IRender with
       member this.Draw x y w h=
         let x = int x
         let y = int y
         let w = int w
         let h = int h
         let c =
           match c with
           |Button  b ->
             b :> Control
           |TextBox b ->b :> Control
         c.Location <-makePoint x y
         c.Size <-makeSize w h
       member this.SetText txt=
         match c with
         |Button  b ->b.Text <- txt
         |TextBox b ->b.Text <- txt

