module Either
open System

type Either<'a,'b> =
  |Right of 'a
  |Left of 'b
  member t.Case (onsuc:System.Action<'a>) (onfail:System.Action<'b>) =
    match t with
    |Right x-> onsuc.Invoke x
    |Left x->onfail.Invoke x

let (>>=) m f =
  match m with
  |Right x->f x
  |Left  e->Left e

type EitherBuilder() =
  let mutable isExit = false
  member x.Bind(m,f) = m >>= f
  member x.Return a  = isExit <- true;Right a
  member x.Zero() = Unchecked.defaultof<_>
  member x.Combine(m,f) = if (isExit ) then m else f()
  member this.Delay(f) = f

let either = new EitherBuilder()

let someBind m f =
  match m with
  | Some x -> f x
  | None -> None

type MaybeBuilder() =
  member x.Bind(m,f) = someBind m f
  member x.Return a  = Some a

let maybe = new MaybeBuilder()

