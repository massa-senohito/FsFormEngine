namespace GameBaseLib
module SharpDXUtil =
  open SharpDX
  let makeVec x y = new Vector2(x,y)
  let makeV3 x y z= new Vector3(x,y,z)
  let makeV3Z x y = new Vector3(x,y,1.0f)
  let V2ToV3 (v:Vector2) = new Vector3(v.X , v.Y , 0.0f)
  let V2ToV3Z1 (v:Vector2) = new Vector3(v.X , v.Y , 1.0f)
  let absV3 (v:Vector3) = makeV3 (abs v.X) (abs v.Y) (abs v.Z)
  let invX (v:Vector2) =
    makeVec -v.X v.Y

  let makeMtx l (s:Vector3) r =
     Matrix.Scaling(s)  * Matrix.RotationQuaternion(r) * Matrix.Translation(l)
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
  let rect =
    [
      makeV3Z  1.0f  1.0f
      makeV3Z -1.0f  1.0f
      makeV3Z -1.0f -1.0f
      makeV3Z  1.0f -1.0f
      makeV3Z  1.0f  1.0f
    ]

