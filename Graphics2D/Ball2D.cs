using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics2D
{
    class Ball2D : Circle2D
    {
        #region Class Parameters
        /// <summary>
        /// Outline pen with a default white color
        /// </summary>
        Pen pen = new Pen(Color.White);
        /// <summary>
        /// Fill brush with a default white color
        /// </summary>
        Brush brush = new SolidBrush(Color.White);
        /// <summary>
        /// The ball's velocity with default zero speed
        /// </summary>
        Point2D velocity = new Point2D();
        /// <summary>
        /// Physical parameters for the ball
        /// </summary>
        double mass = 1;
        double elasticity = 1; // a value between 0 and 1
        double friction = 0.97; // a value between 0 and 1
        #endregion

        #region Class Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Ball2D() { }
        /// <summary>
        /// Creates a ball
        /// </summary>
        /// <param name="center">Center of the ball</param>
        /// <param name="radius">Radius of the ball</param>
        /// <param name="velocity">Velocity of the ball</param>
        public Ball2D(Point2D center, double radius, Point2D velocity)
        {
            this.Center = center;
            this.Radius = radius;
            this.Velocity = velocity;
            mass = Math.Pow(radius, 3);
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// get/set the ball's pen
        /// </summary>
        public Pen Pen
        {
            get { return pen; }
            set { pen = value; }
        }
        /// <summary>
        /// get/set the ball's brush
        /// </summary>
        public Brush Brush
        {
            get { return brush; }
            set { brush = value; }
        }
        /// <summary>
        /// get/set the ball's velocity
        /// </summary>
        public Point2D Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        /// <summary>
        /// get/set the ball's mass
        /// </summary>
        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        /// <summary>
        /// get/set the ball's elasticity
        /// </summary>
        public double Elasticity
        {
            get { return elasticity; }
            set { elasticity = value; }
        }
        /// <summary>
        /// get/set the ball's elasticity
        /// </summary>
        public double Friction
        {
            get { return friction; }
            set { friction = value; }
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Move the ball by it's velocity
        /// </summary>
        public void Move()
        {
            Center += velocity;
            velocity *= friction;
            if (velocity.Magnitude < 0.5)
                velocity = new Point2D(0, 0);
        }
        /// <summary>
        /// Make the bounce against the four walls of the screen
        /// </summary>
        /// <param name="ClientRectangle"></param>
        public void Bounce(RectangleF ClientRectangle)
        {
            if (X + Radius > ClientRectangle.Right)
            {
                velocity.X *= -1;
                X -= (X + Radius - ClientRectangle.Right) * 2;
            }
            if (X - Radius < ClientRectangle.Left)
            {
                velocity.X *= -1;
                X -= (X - Radius - ClientRectangle.Left) * 2;
            }
            if (Y + Radius > ClientRectangle.Bottom)
            {
                velocity.Y *= -1;
                Y -= (Y + Radius - ClientRectangle.Bottom) * 2;
            }
            if (Y - Radius < ClientRectangle.Top)
            {
                velocity.Y *= -1;
                Y -= (Y - Radius - ClientRectangle.Top) * 2;
            }
        }
        /// <summary>
        /// Bounce this ball off of another ball
        /// </summary>
        /// <param name="otherBall"></param>
        public void Bounce(Ball2D otherBall)
        {
            if (!IsColliding(otherBall))
                return;

            Point2D difference = this - otherBall;
            double distance = difference.Magnitude;
            // mtd = minimum translation distance
            // we fudge the mtd by a small factor 1.1 to force them to move apart by at least slight gap
            Point2D mtd = difference * (this.Radius + otherBall.Radius - distance) / distance * 1.1;

            // get the reciprocal of the masses
            double thisMassReciprocal = 1 / mass;
            double otherMassReciprocal = 1 / otherBall.Mass;

            // push the balls apart by the minimum translation distance
            Point2D center = mtd * (thisMassReciprocal / (thisMassReciprocal + otherMassReciprocal));
            this.X += center.X;
            this.Y += center.Y;
            Point2D otherBallCenter = mtd * (otherMassReciprocal / (thisMassReciprocal + otherMassReciprocal));
            otherBall.X -= otherBallCenter.X;
            otherBall.Y -= otherBallCenter.Y;

            // now we "normalize" the mtd to get a unit vector of length 1 in the mtd direction
            mtd.Normalize();

            // impact the velocity due to the collision
            Point2D v = this.velocity - otherBall.velocity;
            double vDotMtd = v * mtd;
            if (double.IsNaN(vDotMtd))
                return;
            if (vDotMtd > 0)
                return; // the balls are already moving in opposite direction

            // work the collision effect
            double i = -(1 + elasticity) * vDotMtd / (thisMassReciprocal + otherMassReciprocal);
            Point2D impulse = mtd * i;

            // change the balls velocities
            this.velocity += impulse * thisMassReciprocal;
            otherBall.velocity -= impulse * otherMassReciprocal;

        }
        /// <summary>
        /// Determines if this ball is colliding with the other ball
        /// </summary>
        /// <param name="otherBall"></param>
        /// <returns>true if colliding, false otherwise</returns>
        public bool IsColliding(Ball2D otherBall)
        {
            return ((this.Center - otherBall.Center).Magnitude < this.Radius + otherBall.Radius);
        }

        /// <summary>
        /// Draw the ball to the graphics device
        /// </summary>
        /// <param name="gr"></param>
        new public void Draw(Graphics gr, Color color)
        {
            brush = new SolidBrush(color);
            pen = new Pen(color);
            gr.FillEllipse(brush, (float)(X - Radius), (float)(Y - Radius), 2 * (float)Radius, 2 * (float)Radius);
            gr.DrawEllipse(pen, (float)(X - Radius), (float)(Y - Radius), 2 * (float)Radius, 2 * (float)Radius);
        }
        #endregion
    }
}
