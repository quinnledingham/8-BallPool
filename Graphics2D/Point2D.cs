using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics2D
{
    /// <summary>
    /// This is a class to describe all the properties and methods of a 2-dimensional point (or vector)
    /// </summary>
    class Point2D
    {
        #region Class Parameters (class variables)
        /// <summary>
        /// the two main variables for a 2D point are x, y coordinate values
        /// </summary>
        double x = 0, y = 0;
        #endregion

        #region Class Constructors
        /// <summary>
        /// Create a class object with default initial values
        /// </summary>
        public Point2D() { }
        /// <summary>
        /// Construct a 2D point with the values passed
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        public Point2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Get/Set the x-value of this point.
        /// </summary>
        public double X { get { return x; } set { x = value; } }
        /// <summary>
        /// Get/Set the y-value of this point.
        /// </summary>
        public double Y { get { return y; } set { y = value; } }
        /// <summary>
        /// Calculate the magnitude of the vector or point (distance from the origin)
        /// </summary>
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
        }
        /// <summary>
        /// Return a vector normal to this vector (perpendicular)
        /// </summary>
        public Point2D Normal
        {
            get { return new Point2D(-y, x); }
        }
        #endregion

        #region Class Operators
        /// <summary>
        /// Define the point addition operator
        /// </summary>
        /// <param name="ptA"></param>
        /// <param name="ptB"></param>
        /// <returns>The coordinate sum of the two points</returns>
        public static Point2D operator +(Point2D ptA, Point2D ptB)
        {
            return new Point2D(ptA.x + ptB.x, ptA.y + ptB.y);
        }

        /// <summary>
        /// Define the point subtraction operator
        /// </summary>
        /// <param name="ptA"></param>
        /// <param name="ptB"></param>
        /// <returns>The coordinate difference of the two points</returns>
        public static Point2D operator -(Point2D ptA, Point2D ptB)
        {
            return new Point2D(ptA.x - ptB.x, ptA.y - ptB.y);
        }

        /// <summary>
        /// Define the point multiplication operator for a dot-product
        /// </summary>
        /// <param name="ptA"></param>
        /// <param name="ptB"></param>
        /// <returns>The dot-product calculation</returns>
        public static double operator *(Point2D ptA, Point2D ptB)
        {
            return (ptA.x * ptB.x) + (ptA.y * ptB.y);
        }

        /// <summary>
        /// Define the multiplication operator
        /// </summary>
        /// <param name="ptA"></param>
        /// <param name="ptB"></param>
        /// <returns>The coodinate scaled by the multiplier</returns>
        public static Point2D operator *(Point2D ptA, double n)
        {
            return new Point2D(ptA.x * n, ptA.y * n);
        }

        /// <summary>
        /// Define the multiplication operator
        /// </summary>
        /// <param name="ptA"></param>
        /// <param name="ptB"></param>
        /// <returns>The coodinate scaled by the multiplier</returns>

        public static Point2D operator *(double n, Point2D ptA)
        {
            return new Point2D(n * ptA.x, n * ptA.y);
        }

        /// <summary>
        /// Define the point division operator
        /// </summary>
        /// <param name="ptA"></param>
        /// <param name="n"></param>
        /// <returns>The coordinate scaled by the divisor</returns>
        public static Point2D operator /(Point2D ptA, double n)
        {
            return new Point2D(ptA.x / n, ptA.y / n);
        }
        #endregion

        #region Class Methods (class functions)
        /// <summary>
        /// Convert this Point2D to a pointF value
        /// </summary>
        /// <returns>The coordinate as a pointF</returns>
        public PointF ToPointF()
        {
            return new PointF((float)x, (float)y);
        }
        /// <summary>
        /// Rotate the point by the set amount of radians
        /// </summary>
        /// <param name="radians">the angle of rotation</param>
        public void Rotate(double radians)
        {
            double x2 = x * Math.Cos(radians) - y * Math.Sin(radians);
            double y2 = x * Math.Sin(radians) + y * Math.Cos(radians);
            // now put these values back in for x, y
            x = x2;
            y = y2;
        }
        /// <summary>
        /// Rotate the point about the origin by the set degree value
        /// </summary>
        /// <param name="degrees">the angle of rotation</param>
        public void RotateD(double degrees)
        {
            Rotate(degrees * Math.PI / 180);
        }
        /// <summary>
        /// Normalize the point/vector to have a length of 1
        /// </summary>
        public void Normalize()
        {
            double magnitude = Magnitude;
            x /= magnitude;
            y /= magnitude;
        }

        public void Draw(Graphics gr, Color color)
        {
            gr.FillEllipse(new SolidBrush(color), (float)x - 1, (float)y - 1, 2, 2);
        }
        #endregion
    }
}
