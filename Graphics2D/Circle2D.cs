using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics2D
{
    class Circle2D : Point2D
    {
        #region Class Parameter
        double radius = 0;
        #endregion

        #region Class Constructors
        /// <summary>
        /// Construct a default circle, center(0,0), radius 0
        /// </summary>
        public Circle2D() { }
        public Circle2D(Point2D center, double radius)
        {
            this.X = center.X;
            this.Y = center.Y;
            this.radius = radius;
        }
        public Circle2D(double x, double y, double radius)
        {
            this.X = x;
            this.Y = y;
            this.radius = radius;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Get/Set the radius of the circle
        /// </summary>
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        /// <summary>
        /// Get/Set the center of the circle
        /// </summary>
        public Point2D Center
        {
            get { return new Point2D(this.X, this.Y); }
            set { this.X = value.X; this.Y = value.Y; }
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Draw the circle to the output device
        /// </summary>
        /// <param name="gr">the output device</param>
        /// <param name="color">Color to draw the circle</param>
        new public void Draw(Graphics gr, Color color)
        {
            gr.DrawEllipse(new Pen(color), (float)(X-radius), (float)(Y - radius), 2*(float)radius, 2*(float)radius);
        }
        /// <summary>
        /// Fill the circle to the output device
        /// </summary>
        /// <param name="gr">the output device</param>
        /// <param name="color">Color to fill the circle</param>
        public void Fill(Graphics gr, Color color)
        {
            gr.FillEllipse(new SolidBrush(color), (float)(X - radius), (float)(Y - radius), 2 * (float)radius, 2 * (float)radius);
        }
        #endregion
    }
}
