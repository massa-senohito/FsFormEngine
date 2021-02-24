namespace FormEng
module FormUtil =
  open System.Drawing
  open System.Windows.Forms
  let getPos (c:Control) = c.Location
  let getSize (c:Control) = c.Size
  let addPoint (p1:Point) (p2:Point) = new Point(p1.X + p2.X , p1.Y + p2.Y)
  let setImageControl (c:Control) image =
    c.BackgroundImage <- image
  let setImageButton (c:ButtonBase) image =
    c.Image <- image
  let makeImage path =
    //let im = new Image()
    Image.FromFile path

  let makePoint x y =
    new Point(x,y)
  let makePointF x y =
    new PointF(x,y)

  let makeSize x y =
    new Size(x,y)

  let setPos (c:Control) x y =
    c.Location <- makePoint x y
  let setSize (c:Control) w h =
    c.Size <- new Size(w,h)

  let setOffset (c:Control) x y=
    let pos = getPos c
    setPos c (pos.X + x) (pos.Y + y)
  let makeButton x y w h =
    let c = new Button()
    setPos c x y
    setSize c w h
    c

  let makeSlider x y w h =
    let c = new TrackBar()
    setPos c x y
    setSize c w h
    c

  let makeTextbox x y w h =
    let c = new TextBox()
    setPos c x y
    setSize c w h
    c.Multiline <- true
    c

  let makeTimer interval =
    let timer = new Timer()
    timer.Interval <- interval
    timer.Start()
    timer

  let show (c:Control) = c.Visible <-true
  let addControl (cs:Control.ControlCollection) c =
    cs.Add(c)

  let makePen (color:Color) w =
    new Pen(color,w)

  let drawLine (g:Graphics) pen (p1:Point) p2=
    g.DrawLine(pen , p1 , p2)

  open SharpDX
  let v2ToPoint (v:Vector2) =
    let ix = int v.X
    let iy = int v.Y
    makePoint ix iy
  let v4ToPoint (v:Vector4) =
    let ix = int v.X
    let iy = int v.Y
    makePoint ix iy

  let v2ToPointF (v:Vector2) =
    makePointF v.X v.Y
