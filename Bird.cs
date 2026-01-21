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
    /// <summary>
    /// Represents a single bird (boid) in a flocking simulation.
    /// Implements Craig Reynolds' Boids algorithm with three main behaviors:
    /// 1. Alignment - steer towards the average heading of neighbors
    /// 2. Cohesion - steer to move toward the average position of neighbors
    /// 3. Separation - avoid crowding neighbors (avoidance)
    /// </summary>
    public class Bird
    {
        // ============================ PUBLIC FIELDS ============================

        /// <summary>
        /// Current position of the bird in 2D space
        /// </summary>
        public Vector2 birdPosition;

        /// <summary>
        /// Current velocity vector of the bird (direction and speed)
        /// </summary>
        public Vector2 vector;

        // ============================ PRIVATE FIELDS ============================

        /// <summary>
        /// Reference to the main form containing the simulation
        /// </summary>
        private readonly Form1 _form1;

        /// <summary>
        /// Maximum speed limit for the bird (prevents unrealistic movement)
        /// </summary>
        private const float MAX_SPEED = 3f;

        /// <summary>
        /// Maximum steering force that can be applied in one update
        /// (controls how quickly birds can change direction)
        /// </summary>
        private const float MAX_FORCE = 0.1f;

        // ============================ CONSTRUCTOR ============================

        /// <summary>
        /// Initializes a new bird with random position and velocity
        /// </summary>
        /// <param name="form1">Reference to the main form for accessing other birds and form properties</param>
        public Bird(Form1 form1)
        {
            this._form1 = form1;

            // Create a highly random seed using GUID to ensure different random sequences
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            // Random starting position within form bounds (with 10px margin)
            int tempX = rand.Next(10, _form1.Width - 10);
            int tempY = rand.Next(10, _form1.Height - 10);
            birdPosition = new Vector2(tempX, tempY);

            // Generate random initial velocity with magnitude up to MAX_SPEED
            var (x, y) = RandomPoint.GenerateRandomPoint(MAX_SPEED, rand);
            vector = new Vector2(x, y);
        }

        // ============================ PUBLIC METHODS ============================

        /// <summary>
        /// Main update method called each frame to update bird's position and velocity
        /// Applies all flocking rules and constraints
        /// </summary>
        public void Update()
        {
            // Apply flocking rules with individual weights for behavior tuning
            Vector2 alignment = Alignment() * 1.0f;       // Standard weight for alignment
            Vector2 cohesion = Cohesion() * 1.0f;        // Standard weight for cohesion
            Vector2 avoidance = Avoidance() * 1.5f;      // Higher weight - separation is most important
            Vector2 stayOnScreen = StayOnScreen() * 1.0f; // Screen boundary handling

            // Limit the magnitude of each steering force to prevent erratic behavior
            alignment = LimitForce(alignment);
            cohesion = LimitForce(cohesion);
            avoidance = LimitForce(avoidance);
            stayOnScreen = LimitForce(stayOnScreen);

            // Combine all steering forces to calculate new velocity
            // Euler integration: new velocity = current velocity + sum of forces
            vector = vector + alignment + cohesion + avoidance + stayOnScreen;

            // Enforce maximum speed constraint
            vector = LimitSpeed(vector);

            // Update position based on velocity (Euler integration)
            // new position = current position + velocity
            birdPosition = birdPosition + vector;
        }

        // ============================ PRIVATE HELPER METHODS ============================

        /// <summary>
        /// Limits the magnitude of a steering force to MAX_FORCE
        /// </summary>
        /// <param name="force">The steering force to limit</param>
        /// <returns>The force vector, normalized to MAX_FORCE if it exceeds the limit</returns>
        private Vector2 LimitForce(Vector2 force)
        {
            if (force.Length() > MAX_FORCE)
            {
                // Normalize to unit vector, then scale to max force
                return Vector2.Normalize(force) * MAX_FORCE;
            }
            return force;
        }

        /// <summary>
        /// Limits the speed (magnitude of velocity) to MAX_SPEED
        /// </summary>
        /// <param name="velocity">The velocity vector to limit</param>
        /// <returns>The velocity vector, normalized to MAX_SPEED if it exceeds the limit</returns>
        private Vector2 LimitSpeed(Vector2 velocity)
        {
            if (velocity.Length() > MAX_SPEED)
            {
                // Normalize to unit vector, then scale to max speed
                return Vector2.Normalize(velocity) * MAX_SPEED;
            }
            return velocity;
        }

        // ============================ FLOCKING BEHAVIOR METHODS ============================

        /// <summary>
        /// Alignment behavior (Rule 1 of Boids algorithm)
        /// Birds steer towards the average heading of their nearby neighbors
        /// Creates directional harmony in the flock
        /// </summary>
        /// <returns>Steering force to align with neighbors</returns>
        public Vector2 Alignment()
        {
            Vector2 averageVelocity = Vector2.Zero;
            int count = 0;

            // Calculate average velocity of nearby birds within perception radius
            foreach (Bird other in _form1.flockers)
            {
                if (other != this)  // Don't include self in calculations
                {
                    float distance = Vector2.Distance(birdPosition, other.birdPosition);
                    if (distance < 50) // Perception radius for alignment
                    {
                        averageVelocity += other.vector;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                // Calculate average velocity
                averageVelocity /= count;

                // Convert to desired velocity (at max speed in that direction)
                averageVelocity = Vector2.Normalize(averageVelocity) * MAX_SPEED;

                // Calculate steering force: desired velocity minus current velocity
                Vector2 steer = averageVelocity - vector;
                return steer;
            }

            return Vector2.Zero; // No neighbors nearby, no alignment needed
        }

        /// <summary>
        /// Cohesion behavior (Rule 2 of Boids algorithm)
        /// Birds steer to move toward the average position of nearby neighbors
        /// Keeps the flock together as a group
        /// </summary>
        /// <returns>Steering force toward the center of mass of nearby neighbors</returns>
        public Vector2 Cohesion()
        {
            Vector2 centerOfMass = Vector2.Zero;
            int count = 0;

            // Calculate center of mass (average position) of nearby birds
            foreach (Bird other in _form1.flockers)
            {
                if (other != this)  // Don't include self in calculations
                {
                    float distance = Vector2.Distance(birdPosition, other.birdPosition);
                    if (distance < 250) // Larger radius for cohesion than alignment
                    {
                        centerOfMass += other.birdPosition;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                // Calculate average position (center of mass)
                centerOfMass /= count;

                // Calculate desired velocity toward the center of mass
                Vector2 desired = centerOfMass - birdPosition;
                desired = Vector2.Normalize(desired) * MAX_SPEED;

                // Calculate steering force: desired velocity minus current velocity
                Vector2 steer = desired - vector;
                return steer;
            }

            return Vector2.Zero; // No neighbors nearby, no cohesion needed
        }

        /// <summary>
        /// Separation behavior (Rule 3 of Boids algorithm) - called Avoidance here
        /// Birds steer to avoid crowding nearby neighbors
        /// Maintains personal space and prevents collisions
        /// </summary>
        /// <returns>Steering force away from nearby neighbors</returns>
        public Vector2 Avoidance()
        {
            Vector2 steer = Vector2.Zero;
            int count = 0;

            // Calculate repulsion from nearby birds
            foreach (Bird other in _form1.flockers)
            {
                if (other != this)  // Don't include self in calculations
                {
                    float distance = Vector2.Distance(birdPosition, other.birdPosition);
                    if (distance < 50) // Personal space radius (smallest of the three)
                    {
                        // Calculate vector away from this neighbor
                        Vector2 diff = birdPosition - other.birdPosition;
                        diff = Vector2.Normalize(diff);

                        // Weight by inverse distance: closer neighbors = stronger repulsion
                        diff /= distance;

                        steer += diff;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                // Average the repulsion vectors
                steer /= count;

                // Convert to desired velocity (at max speed away from neighbors)
                steer = Vector2.Normalize(steer) * MAX_SPEED;

                // Calculate steering force: desired velocity minus current velocity
                steer -= vector;
            }

            return steer;
        }

        /// <summary>
        /// Screen boundary handling behavior
        /// Implements wrap-around: birds reappear on opposite side when leaving screen
        /// Alternative approach: could apply steering force toward center
        /// </summary>
        /// <returns>Always returns Vector2.Zero as wrap-around modifies position directly</returns>
        public Vector2 StayOnScreen()
        {
            // Wrap around horizontally
            if (birdPosition.X < -10)
                birdPosition.X = _form1.Width + 10;
            else if (birdPosition.X > _form1.Width + 10)
                birdPosition.X = -10;

            // Wrap around vertically
            if (birdPosition.Y < -10)
                birdPosition.Y = _form1.Height + 10;
            else if (birdPosition.Y > _form1.Height + 10)
                birdPosition.Y = -10;

            return Vector2.Zero; // No steering force for wrapping (position modified directly)
        }

        // ============================ RENDERING METHOD ============================

        /// <summary>
        /// Draws the bird as a triangle pointing in its direction of movement
        /// Visualizes both position and orientation
        /// </summary>
        /// <param name="g">Graphics object for drawing</param>
        /// <param name="origin">Position to draw the triangle (bird's position)</param>
        /// <param name="directionVector">Velocity vector indicating direction</param>
        /// <param name="color">Color of the bird</param>
        public void DrawPointingTriangle(Graphics g, Vector2 origin, Vector2 directionVector, Color color)
        {
            // Normalize the direction vector to get unit vector
            float length = (float)Math.Sqrt(directionVector.X * directionVector.X + directionVector.Y * directionVector.Y);
            PointF normalizedDirection = new PointF(directionVector.X / length, directionVector.Y / length);

            // Calculate perpendicular vector (90-degree rotation)
            // Used to create the base of the triangle
            PointF perpendicular = new PointF(-normalizedDirection.Y, normalizedDirection.X);

            // Define triangle dimensions
            float height = 10;           // Length of triangle from base to tip
            float baseWidth = 10 * 0.6f; // Width of triangle base

            // Calculate three vertices of the triangle:

            // 1. Tip - extends in the direction of movement
            PointF tip = new PointF(
                origin.X + normalizedDirection.X * height,
                origin.Y + normalizedDirection.Y * height);

            // 2. First base point - offset backward and perpendicular left
            PointF basePoint1 = new PointF(
                origin.X - normalizedDirection.X * height * 0.2f - perpendicular.X * baseWidth / 2,
                origin.Y - normalizedDirection.Y * height * 0.2f - perpendicular.Y * baseWidth / 2);

            // 3. Second base point - offset backward and perpendicular right
            PointF basePoint2 = new PointF(
                origin.X - normalizedDirection.X * height * 0.2f + perpendicular.X * baseWidth / 2,
                origin.Y - normalizedDirection.Y * height * 0.2f + perpendicular.Y * baseWidth / 2);

            // Draw the filled triangle
            using (Brush brush = new SolidBrush(color))
            {
                g.FillPolygon(brush, new PointF[] { tip, basePoint1, basePoint2 });
            }
        }
    }
}