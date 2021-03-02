namespace MMDEng
open GameBaseLib
open SharpDX
open MikuMikuFlex3
open System

module MMDActor =
  [<AbstractClass>]
  type AbstructActor() =
    abstract member Render  : IRender
    abstract member Actor   : Actor
    //abstract member Dispose : IDisposable
    interface IDisposable with
      member t.Dispose() = ()

  type ModelActor (model:PMXModel,a:Actor) =
    inherit AbstructActor()
    override t.Render = t :> IRender
    override t.Actor  = a
    interface IDisposable with
      override this.Dispose() =
        () // clicked
    interface IRender with
       member this.Draw ()=
         model.WorldTransformationMatrix <- a.Mat
       member this.SetText txt =
         //g.DrawString(txt , font , pen.Brush , v2ToPointF a.Pos)
         ()

