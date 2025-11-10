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
        public Vector2 birdPosition;
        public Vector2 vector;

        private readonly Form1 _form1;
        private const float MAX_SPEED = 3f;
        private const float MAX_FORCE = 0.1f;

        public Bird(Form1 form1)
        {
            this._form1 = form1;
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int tempX = rand.Next(10, _form1.Width - 10);
            int tempY = rand.Next(10, _form1.Height - 10);
            birdPosition = new Vector2(tempX, tempY);
            var (x, y) = RandomPoint.GenerateRandomPoint(MAX_SPEED, rand);
            vector = new Vector2(x, y);
        }

        public void Update()
        {
            // Apply flocking rules with proper weights
            Vector2 alignment = Alignment() * 1.0f;
            Vector2 cohesion = Cohesion() * 1.0f;
            Vector2 avoidance = Avoidance() * 1.5f; // Avoidance is most important
            Vector2 stayOnScreen = StayOnScreen() * 1.0f;

            // Limit the forces
            alignment = LimitForce(alignment);
            cohesion = LimitForce(cohesion);
            avoidance = LimitForce(avoidance);
            stayOnScreen = LimitForce(stayOnScreen);

            // Apply all forces
            vector = vector + alignment + cohesion + avoidance + stayOnScreen;

            // Limit speed
            vector = LimitSpeed(vector);

            // Update position
            birdPosition = birdPosition + vector;
        }

        private Vector2 LimitForce(Vector2 force)
        {
            if (force.Length() > MAX_FORCE)
            {
                return Vector2.Normalize(force) * MAX_FORCE;
            }
            return force;
        }

        private Vector2 LimitSpeed(Vector2 velocity)
        {
            if (velocity.Length() > MAX_SPEED)
            {
                return Vector2.Normalize(velocity) * MAX_SPEED;
            }
            return velocity;
        }

        public Vector2 Alignment()
        {
            Vector2 averageVelocity = Vector2.Zero;
            int count = 0;

            foreach (Bird other in _form1.flockers)
            {
                if (other != this)
                {
                    float distance = Vector2.Distance(birdPosition, other.birdPosition);
                    if (distance < 50) // Perception radius
                    {
                        averageVelocity += other.vector;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                averageVelocity /= count;
                averageVelocity = Vector2.Normalize(averageVelocity) * MAX_SPEED;
                Vector2 steer = averageVelocity - vector;
                return steer;
            }

            return Vector2.Zero;
        }

        public Vector2 Cohesion()
        {
            Vector2 centerOfMass = Vector2.Zero;
            int count = 0;

            foreach (Bird other in _form1.flockers)
            {
                if (other != this)
                {
                    float distance = Vector2.Distance(birdPosition, other.birdPosition);
                    if (distance < 150) // Larger radius for cohesion
                    {
                        centerOfMass += other.birdPosition;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                centerOfMass /= count;
                Vector2 desired = centerOfMass - birdPosition;
                desired = Vector2.Normalize(desired) * MAX_SPEED;
                Vector2 steer = desired - vector;
                return steer;
            }

            return Vector2.Zero;
        }

        public Vector2 Avoidance()
        {
            Vector2 steer = Vector2.Zero;
            int count = 0;

            foreach (Bird other in _form1.flockers)
            {
                if (other != this)
                {
                    float distance = Vector2.Distance(birdPosition, other.birdPosition);
                    if (distance < 50) // Personal space radius
                    {
                        Vector2 diff = birdPosition - other.birdPosition;
                        diff = Vector2.Normalize(diff);
                        diff /= distance; // Weight by distance (closer = stronger repulsion)
                        steer += diff;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                steer /= count;
                steer = Vector2.Normalize(steer) * MAX_SPEED;
                steer -= vector;
            }

            return steer;
        }

        public Vector2 StayOnScreen()
        {
            // Wrap around screen edges instead of applying force
            if (birdPosition.X < -10)
                birdPosition.X = _form1.Width + 10;
            else if (birdPosition.X > _form1.Width + 10)
                birdPosition.X = -10;

            if (birdPosition.Y < -10)
                birdPosition.Y = _form1.Height + 10;
            else if (birdPosition.Y > _form1.Height + 10)
                birdPosition.Y = -10;

            return Vector2.Zero; // No steering force for wrapping
        }



        public void DrawPointingTriangle(Graphics g, Vector2 origin, Vector2 directionVector,  Color color)
        {
            // Normalize the direction vector
            float length = (float)Math.Sqrt(directionVector.X * directionVector.X + directionVector.Y * directionVector.Y);
            PointF normalizedDirection = new PointF(directionVector.X / length, directionVector.Y / length);

            // Calculate perpendicular vector (for the base of the triangle)
            PointF perpendicular = new PointF(-normalizedDirection.Y, normalizedDirection.X);

            // Define triangle dimensions
            float height = 10;//changed from 15 to 10
            float baseWidth = 10 * 0.6f; // Adjust this ratio to change triangle shape

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
