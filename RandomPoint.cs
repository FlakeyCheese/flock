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
            
            double r = 0;
            
            
                r = 0.5; //set r to 0.5
            

            // Convert polar coordinates (r, theta) to Cartesian coordinates (x, y)
            double x = radius * r * Math.Cos(theta);
            double y = radius * r * Math.Sin(theta);

            return ((float)x, (float)y);
        }
    }
}
