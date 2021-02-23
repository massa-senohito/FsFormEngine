namespace GameBaseLib
module SharpDXUtil =
  open SharpDX
  let makeVec x y = new Vector2(x,y)
  let invX (v:Vector2) =
    makeVec -v.X v.Y

  let invY (v:Vector2) =
    makeVec v.X -v.Y
  let rectVert p (s:Vector2) =
    [
      p +     (s * 0.5f)
      p - invY(s * 0.5f)
      p -     (s * 0.5f)
      p + invY(s * 0.5f)
      p +     (s * 0.5f)
    ]

