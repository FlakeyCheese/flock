using System;
using System.Collections.Generic;
using System.Drawing;
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
           

        }
        public void Update()
        {
            //Avoidance();

            //birdPosition= Vector2.Add( birdPosition, Alignment());
            vector = vector + Alignment() + Avoidance() + Cohesion() +StayOnScreen();
            birdPosition = birdPosition + vector ;
            
            
            
            
            
        }
        public Vector2 Cohesion()// the average direction of the flock
        {   Vector2 aveDirection = Vector2.Zero;
            foreach (Bird b in _form1.flockers)
            {
                aveDirection += b.vector;
            }
            aveDirection /= 200;
            return aveDirection/_form1.flockers.Count;
        }
        public Vector2 Alignment()//move towards average position of flock
        {
            Vector2 destination = new Vector2(0f, 0f);
            foreach (Bird b in _form1.flockers)
            {
                destination = destination + b.birdPosition; 
            }
            destination = destination/ _form1.flockers.Count ;
            Vector2 newVector = new Vector2(0,0);
                
                newVector = new Vector2((destination.X-birdPosition.X)/500,(destination.Y - birdPosition.Y)/500);
                return newVector;
            
            
        }
        public Vector2 Avoidance()//avoid hitting neighbours
        {
            
            Vector2 avoidFactor = new Vector2(0, 0);
            foreach (Bird b in _form1.flockers)
            {
                if (b != this)
                {
                    if (Vector2.Distance(b.birdPosition, this.birdPosition) < 5)
                    {
                        
                        avoidFactor.X = avoidFactor.X - b.birdPosition.X;
                        avoidFactor.Y = avoidFactor.Y - b.birdPosition.Y;
                    }
                }
                
            }
            avoidFactor.X /= 500;
            avoidFactor.Y /= 500;
            return avoidFactor;

        }

        public Vector2 StayOnScreen() //keep all birds on screen
        {
            Vector2 avoidFactor = new Vector2(0f, 0f);
            if (this.birdPosition.X < 100) { avoidFactor.X += 1.0f; }
            if (this.birdPosition.Y < 100) { avoidFactor.Y += 1.0f; }
            if (this.birdPosition.X > _form1.Width - 100) { avoidFactor.X -= 1.0f; }
            if (this.birdPosition.Y > _form1.Height - 100) { avoidFactor.Y -= 1.0f; }
            return avoidFactor;
        }
        public void DrawPointingTriangle(Graphics g, Vector2 origin, Vector2 directionVector,  Color color)
        {
            // Normalize the direction vector
            float length = (float)Math.Sqrt(directionVector.X * directionVector.X + directionVector.Y * directionVector.Y);
            PointF normalizedDirection = new PointF(directionVector.X / length, directionVector.Y / length);

            // Calculate perpendicular vector (for the base of the triangle)
            PointF perpendicular = new PointF(-normalizedDirection.Y, normalizedDirection.X);

            // Define triangle dimensions
            float height = 15;
            float baseWidth = 15 * 0.6f; // Adjust this ratio to change triangle shape

            // Calculate the three points
            PointF tip = new PointF(
                origin.X + normalizedDirection.X * height,
                origin.Y + normalizedDirection.Y * height);

            PointF basePoint1 = new PointF(
                origin.X - normalizedDirection.X * height * 0.2f - perpendicular.X * baseWidth / 2,
                origin.Y - normalizedDirection.Y * height * 0.2f - perpendicular.Y * baseWidth / 2);

            PointF basePoint2 = new PointF(
                origin.X - normalizedDirection.X * height * 0.2f + perpendicular.X * baseWidth / 2,
                origin.Y - normalizedDirection.Y * height * 0.2f + perpendicular.Y * baseWidth / 2);

            // Draw the triangle
            using (Brush brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, new PointF[] { tip, basePoint1, basePoint2 });
            }
        }
    }
}
