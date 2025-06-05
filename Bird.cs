using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace flock
{
    public class Bird     

    {
        public Vector2 birdPosition;//position of this bird instance
        public Vector2 vector;//vector representing direction and speed
        private readonly Form1 _form1;
        public Bird(Form1 form1) 
        {
            this._form1 = form1;
            Random rand = new Random(Guid.NewGuid().GetHashCode());//avery random seed
            int tempX = rand.Next(10, _form1.Width-10);//generate random start point for the bird
            int tempY = rand.Next(10, _form1.Height-10); //inside the form bounds
            birdPosition = new Vector2(tempX, tempY);
            var (x, y) = RandomPoint.GenerateRandomPoint(5, rand);
            vector = new Vector2(x, y);
            _form1.Invalidate();

        }
        public void Update()
        {
            //Avoidance();
            
            //birdPosition= Vector2.Add( birdPosition, Alignment());
            birdPosition = Vector2.Add(birdPosition, vector);
            //if (birdPosition.X < 0 || birdPosition.X > _form1.Width) {vector.X = -vector.X;}
            //if (birdPosition.Y < 0 || birdPosition.Y > _form1.Height) { vector.Y = -vector.Y;}
            
            
            //_form1.Invalidate();
        }
        public void Cohesion()// the average direction of the flock
        {
            
        }
        public Vector2 Alignment()//move towards average position of flock
        {
            //list of bird positions
            List<Vector2> positionList = new List<Vector2>();
            foreach (Bird b in _form1.flockers)
            {
                if (Vector2.Distance(b.birdPosition, this.birdPosition) < 50)
                //move BirdPosition to a list
                { positionList.Add(b.birdPosition); }
            }
            Vector2 avePos = new Vector2((positionList.Average(x => x.X)), (positionList.Average(y => y.Y)));
            avePos.X = avePos.X / 200;
            avePos.Y = avePos.Y / 200;
            return avePos;
        }
        public void Avoidance()//avoid hitting neighbours
        {
            foreach (Bird b in _form1.flockers)
            {
                if (Vector2.Distance(b.birdPosition, this.birdPosition) < 5)
                {
                    vector.X = -vector.X;
                    vector.Y = -vector.Y;
                }
            }
        }
    }
}
