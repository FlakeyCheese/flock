using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flock
{
    internal class RandomPoint
    {
        public static (float X, float Y) GenerateRandomPoint(double maxSpeed, Random random)
        {
            // Generate random angle
            double theta = random.NextDouble() * 2 * Math.PI;

            // Generate random speed between 0 and maxSpeed
            double speed = random.NextDouble() * maxSpeed;

            // Convert to Cartesian coordinates
            double x = speed * Math.Cos(theta);
            double y = speed * Math.Sin(theta);

            return ((float)x, (float)y);
        }
    }
}
