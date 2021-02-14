namespace FormEng
module HPWindow =
  open System.Drawing
  open System.Windows.Forms
  type HPWindow(n,hp,mp) as t = 
    inherit UserControl()
    let nameTxt = FormUtil.makeTextbox 1 4 80 20
    let hpTxt = FormUtil.makeTextbox 1 24 80 20
    let mpTxt = FormUtil.makeTextbox 1 44 80 20
    let addControl c = FormUtil.addControl t.Controls c
    do 
      t.SuspendLayout()
      nameTxt.Text <- string n
      addControl nameTxt
      hpTxt.Text <- string hp
      addControl hpTxt
      mpTxt.Text <- string mp
      addControl mpTxt
      t.ResumeLayout(false)
      t.PerformLayout()
    override t.ToString() =
      nameTxt.Text + string t.Location
