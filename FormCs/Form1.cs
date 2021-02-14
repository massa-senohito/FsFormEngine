using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormCs
{
    public partial class Form1 : Form
    {
        public Form1( )
        {
            //InitializeComponent( );
            Size = new Size( 400 , 400 );
            this.components = new System.ComponentModel.Container();
            SuspendLayout( );
            var hp = new UserControlHP();
            hp.Location = new Point( 10 , 20 );
            var hp2 = new UserControlHP();
            hp2.Location = new Point( 150 , 20 );
            Controls.Add( hp );
            Controls.Add( hp2 );
            var button2 = new Button();
            button2.Location = new System.Drawing.Point(308, 105);
            button2.Name = "button1";
            button2.Size = new System.Drawing.Size(75, 23);
            button2.TabIndex = 0;
            button2.Text = "button1";
            button2.UseVisualStyleBackColor = true;
            Controls.Add( button2 );
            ResumeLayout( false);
            PerformLayout( );
        }

    }
}
