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
            birdPosition = Vector2.Add(birdPosition, vector);
            
            
            
            
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
                   
                }
            }
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
