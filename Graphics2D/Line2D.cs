using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Graphics2D
{
    class Line2D
    {
        #region Class Parameters
        Point2D[] pts = new Point2D[2];
        Ball2D[] endPts = new Ball2D[2];
        Pen pen = new Pen(Color.Brown);
        #endregion

        #region Class Constructors
        public Line2D()
        {
            pts[0] = new Point2D();
            pts[1] = new Point2D();
            endPts[0] = new Ball2D();
            endPts[1] = new Ball2D();
            endPts[0].Mass = double.MaxValue;
            endPts[1].Mass = double.MaxValue;
        }
        public Line2D(Point2D p1, Point2D p2)
        {
            pts[0] = p1;
            pts[1] = p2;
            endPts[0] = new Ball2D(p1, 0.01, new Point2D());
            endPts[1] = new Ball2D(p2, 0.01, new Point2D());
            endPts[0].Mass = double.MaxValue;
            endPts[1].Mass = double.MaxValue;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// get/set the line's pen
        /// </summary>
        public Pen Pen
        {
            get => pen;
            set => pen = value;
        }
        public Point2D P1
        {
            get => pts[0];
            set => pts[0] = value;
        }
        public Point2D P2
        {
            get => pts[1];
            set => pts[1] = value;
        }
        #endregion

        #region Class Methods
        public void Rotate(Double radians)
        {
            pts[0].Rotate(radians);
            pts[1].Rotate(radians);
            endPts[0].Rotate(radians);
            endPts[1].Rotate(radians);
        }
        public void RotateD(Double degrees)
        {
            pts[0].RotateD(degrees);
            pts[1].RotateD(degrees);
            endPts[0].RotateD(degrees);
            endPts[1].RotateD(degrees);
        }
        /// <summary>
        /// Draw the line to the display
        /// </summary>
        /// <param name="gr"></param>
        public void Draw(Graphics gr)
        {          
            gr.DrawLine(pen, pts[0].ToPointF(), pts[1].ToPointF());
        }
        #endregion

        #region Line - Ball Bounce Code
        /// <summary>
        /// Determine the line that is normal from this line to a ball center
        /// </summary>
        /// <param name="ball"></param>
        /// <returns></returns>
        public Point2D NormalToBall(Ball2D ball)
        {
            Point2D v = pts[0] - pts[1];
            Point2D ballToLine = ball - pts[0];
            Point2D normal = v.Normal;
            // make normal a unit vector
            normal.Normalize();
            if (normal * ballToLine < 0)
                normal *= -1;
            return normal;
        }

        /// <summary>
        /// Determine if the bounding rectangle of the line contains point p;
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Point2D p)
        {
            RectangleF boundingRect = new RectangleF(
                (float)Math.Min(pts[0].X, pts[1].X) - 0.5f,
                (float)Math.Min(pts[0].Y, pts[1].Y) - 0.5f,
                (float)Math.Abs(pts[0].X - pts[1].X) + 1f,
                (float)Math.Abs(pts[0].Y - pts[1].Y) + 1f);
            return boundingRect.Contains(p.ToPointF());
        }
        /// <summary>
        /// Determines the points of intersection between this line and another line (otherLine)
        /// </summary>
        /// <param name="otherLine"></param>
        /// <returns></returns>
        public Point2D LineIntersectionPoint(Line2D otherLine)
        {
            // Get A,B,C of first line - points : pts[0] to pts[1]
            double A1 = pts[1].Y - pts[0].Y;
            double B1 = pts[0].X - pts[1].X;
            double C1 = A1 * pts[0].X + B1 * pts[0].Y;

            // Get A,B,C of second line - points : ps2 to pe2
            double A2 = otherLine.pts[1].Y - otherLine.pts[0].Y;
            double B2 = otherLine.pts[0].X - otherLine.pts[1].X;
            double C2 = A2 * otherLine.pts[0].X + B2 * otherLine.pts[0].Y;

            // Get delta and check if the lines are parallel
            double delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                return new Point2D(double.MaxValue, double.MaxValue);
            //throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Point2D(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }
        /// <summary>
        /// Computes the reflected segment at a point of a curve
        /// The line is the segment being reflected
        /// The normal is the normal of the reflection line and the ball
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public Point2D Reflection(Point2D normal)
        {
            double rx, ry;
            Point2D direction = pts[1] - pts[0];
            double dot = direction * normal;
            rx = direction.X - 2 * normal.X * dot;
            ry = direction.Y - 2 * normal.Y * dot;
            return new Point2D(rx, ry);
        }

        public bool Bounce(Ball2D ball)
        {
            // determine the normal vector from the line to the ball
            Point2D normal = NormalToBall(ball);
            // make a temporary line of this line moved one radius towards the ball
            // this allows us to "bounce of the center"
            Line2D aLineTemp = new Line2D(pts[0] + normal * ball.Radius,
                pts[1] + normal * ball.Radius);


            // we need to know where the ball will be in one step
            Point2D ballNextStep = ball + ball.Velocity;
            // make a line from where the ball is now to the location at the next step
            Line2D ballPath = new Line2D(ball, ballNextStep);

            // find the point of intersection between the line and the path of the ball
            Point2D intersectionPt = aLineTemp.LineIntersectionPoint(ballPath);
            // perform the bounce if necessary

            // bounce off the endpoints if necessary
            if (endPts[0].IsColliding(ball))
            {
                endPts[0].Bounce(ball);
                return true;
            }
            else if (endPts[1].IsColliding(ball))
            {
                endPts[1].Bounce(ball);
                return true;
            }
            // if the point of intersection is within the line segment 
            // AND the ball is moving towards the line
            else if (ball.Velocity.Magnitude < (intersectionPt - ball).Magnitude
                && normal * ball.Velocity < 0)
                return false;
            else if (aLineTemp.Contains(intersectionPt)
                && normal * ball.Velocity < 0)
            {
                Line2D reflectionLine = new Line2D(ball + ball.Velocity, intersectionPt);
                Point2D velocityDirection = -1 * reflectionLine.Reflection(NormalToBall(ball));
                velocityDirection.Normalize();
                ball.Velocity = velocityDirection * ball.Velocity.Magnitude;
                Point2D ballLocation =
                    intersectionPt -
                    reflectionLine.Reflection(NormalToBall(ball)) * ball.Elasticity;
                ball.X = ballLocation.X;
                ball.Y = ballLocation.Y;

                return true;
            }
            return false;
        }
        #endregion
    }
}
