namespace FormEng
module FormActor =
  open GameBaseLib
  open System.Windows.Forms
  open System
  open System.Drawing
  open SharpDX
  open FormUtil

  [<AbstractClass>]
  type AbstructControl() =
    abstract member AddControl:Form->unit
    abstract member RemoveControl:Form->unit
    abstract member Render  : IRender
    abstract member Actor   : Actor
    //abstract member Dispose : IDisposable
    interface IDisposable with
      member t.Dispose() = ()

  type DebugActor (g:Graphics , pen:Pen , a:Actor) =
    inherit AbstructControl()
    let font = new Font("MS UI Gothic", 20.0f)
    override t.AddControl (f:Form) = ()
    override t.RemoveControl (f:Form) = ()
    override t.Render = t :> IRender
    override t.Actor  = a
    interface IRender with
       //member this.Draw (m:Matrix) =
       member t.Draw ()=
         //let t = m.TranslationVector
         //let s = m.ScaleVector
         let trans v = Vector3.Transform( v , t.Actor.Mat )
         let rectVert = SharpDXUtil.rect |> List.map trans |> List.map v4ToPoint |> List.toArray
         g.DrawLines(pen,rectVert)

       member this.SetText txt =
         g.DrawString(txt , font , pen.Brush , v2ToPointF a.Pos)

  type ActorControl (c:Control ,a:Actor)=
    inherit AbstructControl()
    override t.AddControl (form:Form) =
      form.Controls.Add c

    override t.RemoveControl (form:Form) =
      form.Controls.Remove c
    override t.Render = t :> IRender
    override t.Actor  = a

    member this.Control =
      c
    interface IDisposable with
      override this.Dispose() =
        this.Control.Click.RemoveHandler a.OnClick
    interface IRender with
       member this.Draw ()=
         let x = int a.Pos.X
         let y = int a.Pos.Y
         let w = int a.Scale.X
         let h = int a.Scale.Y
         c.Location <-makePoint x y
         c.Size <-makeSize w h
       member this.SetText txt=
         //match c with
         //|Button  b ->b.Text <- txt
         //|TextBox b ->b.Text <- txt
         c.Text <- txt

