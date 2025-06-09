using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace flock
{
    public partial class Form1 : Form
    {
        public  List<Bird> flockers = new List<Bird>();
        public Form1()
        {
            this.DoubleBuffered = true;//smooths the animation
            InitializeComponent();
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        { Graphics graphics = e.Graphics;
            SolidBrush brush = new SolidBrush(Color.Black);
            bool isNullOrEmpty = flockers?.Any() != true;
            if (isNullOrEmpty) {return; }
            else
            {
                foreach (Bird b in flockers)
                {
                    
                    //e.Graphics.FillEllipse(brush,b.birdPosition.X,b.birdPosition.Y,5,5);
                    b.DrawPointingTriangle(graphics,b.birdPosition,b.vector,Color.Red);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bird tempBird;
            for (int i = 0; i < 1000; i++)
            {
                tempBird = new Bird(this);
                flockers.Add(tempBird);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach(Bird b in flockers)
            {
                b.Update();
            }
            this.Invalidate();
        }
    }
}
