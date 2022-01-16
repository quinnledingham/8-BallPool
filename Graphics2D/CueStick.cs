using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics2D
{
    class CueStick
    {
        #region Parameters
        /// <summary>
        /// The tip of the cue stick and the stick part of the cue stick
        /// </summary>
        Ball2D tip = new Ball2D();
        Line2D stick = new Line2D();
        #endregion

        #region Constructors
        /// <summary>
        /// Default cuestick constructor
        /// </summary>
        public CueStick() { }
        /// <summary>
        /// Creates a cuestick and the tip for the cuestick
        /// </summary>
        /// <param name="p1">Starting point of the stick and where the tip goes. Where the mouse is clicked</param>
        /// <param name="p2">Where the stick extends to.</param>
        public CueStick(Point2D p1, Point2D p2)
        {
            stick.P1 = p1;
            stick.P2 = p2;

            tip = new Ball2D(stick.P1, 3, new Point2D(0, 0));
            tip.Mass = double.MaxValue;
            tip.Friction = 1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets and Sets the point of the end of the stick
        /// </summary>
        public Point2D Hand
        {
            get { return stick.P2; }
            set { stick.P2 = value; }
        }
        /// <summary>
        /// Gets and Sets the tip of the stick
        /// </summary>
        public Ball2D Tip
        {
            get { return tip; }
            set { tip = value; }
        }
        #endregion

        #region Method
        /// <summary>
        /// Draws the cue stick
        /// </summary>
        /// <param name="gr">Graphics</param>
        public void Draw(Graphics gr)
        {
            stick = new Line2D(stick.P1 += tip.Velocity, Hand += tip.Velocity);
            tip.Draw(gr, Color.White);
            stick.Pen = new Pen(Color.Brown);
            stick.Pen.Width = 7;
            stick.Draw(gr);          
        }
        /// <summary>
        /// Sets the velocity of the tip based on how far away from the tip the mouse is. Max velocity is 25.
        /// </summary>
        /// <param name="clickUp"></param>
        public void Velocity(Point2D clickUp)
        {
            tip.Velocity = new Point2D((stick.P1.X - stick.P2.X) / 10, (stick.P1.Y - stick.P2.Y) / 10);
            if (tip.Velocity.Magnitude > 25)
            {
                tip.Velocity.Normalize();
                tip.Velocity *= 25;
            }
        }
        /// <summary>
        /// Moves the tip like it is a ball.
        /// </summary>
        public void Move()
        {
            tip.Move();
        }
        #endregion
    }
}
