using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flock
{
    internal class RandomPoint
    {
        public static (float X, float Y) GenerateRandomPoint(double radius, Random random)
        {
            // Generate random angle (theta) between 0 and 2π
            double theta = random.NextDouble() * 2 * Math.PI;

            // Calculate corresponding radius (r) using inverse CDF of uniform distribution
            Random rnd = new Random();// new random value to choose between sircle and classic explosion
            double r = 0;
            if (rnd.Next(0, 2) == 0)//using the 0, 1 generated to switch between circular (r = 0.5 ) and random explosions
            {
                r = Math.Sqrt(random.NextDouble());//set a random value for r
            }
            else
            {
                r = 0.5; //set r to 0.5
            }

            // Convert polar coordinates (r, theta) to Cartesian coordinates (x, y)
            double x = radius * r * Math.Cos(theta);
            double y = radius * r * Math.Sin(theta);

            return ((float)x, (float)y);
        }
    }
}
