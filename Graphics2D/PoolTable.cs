using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Graphics2D
{
    class PoolTable
    {
        #region Parameters
        /// <summary>
        /// Variables for the pooltable and some gameplay
        /// </summary>
        List<Line2D> lines = new List<Line2D>();
        List<Circle2D> holes = new List<Circle2D>();
        Pen pen = new Pen(Color.Blue);
        int scoreRed = 0;
        int scoreBlue = 0;
        bool cueBallOut = true;
        bool gotBallIn = false;
        bool checkWin = false;
        Point2D cueHand = new Point2D(0, 0);
        string playerTurn = "";
        int player = 1;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates all of the bumbers and pockets for the balls to interact with
        /// </summary>
        public PoolTable()
        {
            #region Lines
            lines.Add(new Line2D(new Point2D(76, 130), new Point2D(76, 722)));
            lines.Add(new Line2D(new Point2D(132, 779), new Point2D(741, 779)));
            lines.Add(new Line2D(new Point2D(820, 779), new Point2D(1430, 779)));
            lines.Add(new Line2D(new Point2D(1486, 132), new Point2D(1486, 722)));
            lines.Add(new Line2D(new Point2D(820, 76), new Point2D(1429, 76)));
            lines.Add(new Line2D(new Point2D(132, 76), new Point2D(741, 76)));

            lines.Add(new Line2D(new Point2D(76, 130), new Point2D(57, 113)));
            lines.Add(new Line2D(new Point2D(132, 76), new Point2D(113, 55)));
            lines.Add(new Line2D(new Point2D(740, 76), new Point2D(745, 56)));
            lines.Add(new Line2D(new Point2D(822, 76), new Point2D(816, 56)));
            lines.Add(new Line2D(new Point2D(1429, 76), new Point2D(1449, 56)));
            lines.Add(new Line2D(new Point2D(1486, 131), new Point2D(1504, 113)));
            lines.Add(new Line2D(new Point2D(1486, 723), new Point2D(1503, 741)));
            lines.Add(new Line2D(new Point2D(1430, 779), new Point2D(1448, 798)));
            lines.Add(new Line2D(new Point2D(821, 779), new Point2D(817, 799)));
            lines.Add(new Line2D(new Point2D(741, 779), new Point2D(745, 799)));
            lines.Add(new Line2D(new Point2D(132, 779), new Point2D(115, 798)));
            lines.Add(new Line2D(new Point2D(76, 722), new Point2D(58, 740)));
            #endregion

            holes.Add(new Circle2D(new Point2D(70, 70), 34));
            holes.Add(new Circle2D(new Point2D(70, 782), 34));
            holes.Add(new Circle2D(new Point2D(780, 810), 34));
            holes.Add(new Circle2D(new Point2D(1488, 782), 34));
            holes.Add(new Circle2D(new Point2D(1490, 69), 34));
            holes.Add(new Circle2D(new Point2D(780, 44), 34));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get/Set the Red score
        /// </summary>
        public int ScoreRed
        {
            get { return scoreRed; }
            set { scoreRed = value; }
        }
        /// <summary>
        /// Get/Set the Blue score
        /// </summary>
        public int ScoreBlue
        {
            get { return scoreBlue; }
            set { scoreBlue = value; }
        }
        /// <summary>
        /// Get/Set if the cue ball is in play
        /// </summary>
        public bool CueBallOut
        {
            get { return cueBallOut; }
            set { cueBallOut = value; }
        }
        /// <summary>
        /// Get/Set if a ball went into a pocket this cue ball hit
        /// </summary>
        public bool GotBallIn
        {
            get { return gotBallIn; }
            set { gotBallIn = value; }
        }
        /// <summary>
        /// Get/Set the location of the mouse on the cuestick
        /// </summary>
        public Point2D CueHand
        {
            get { return cueHand; }
            set { cueHand = value; }
        }
        /// <summary>
        /// Get/Set whose turn it is (Red or Blue)
        /// </summary>
        public string PlayerTurn
        {
            get { return playerTurn; }
            set { playerTurn = value; }
        }
        /// <summary>
        /// Get/Set which player is hitting the ball (1 or 2)
        /// </summary>
        public int Player
        {
            get { return player; }
            set { player = value; }
        }
        /// <summary>
        /// Get/Set if the program should check who won
        /// </summary>
        public bool CheckWin
        {
            get { return checkWin; }
            set { checkWin = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draw the pool tables lines and holes
        /// </summary>
        /// <param name="gr">Graphics</param>
        public void PoolTableDraw(Graphics gr)
        {
            foreach (Line2D line in lines)
            {
                line.Pen = new Pen(Color.Transparent);
                line.Draw(gr);
            }
            foreach (Circle2D hole in holes)
                hole.Draw(gr, Color.Transparent);
        }

        /// <summary>
        /// Checks if a ball goes into one of the pockets.
        /// </summary>
        /// <param name="balls">All the balls on the pooltable</param>
        public void RemoveBall(List<Ball2D> balls, RectangleF inPlayZone)
        {
            for (int i = 0; i < holes.Count; i++)
            {
                for (int j = 0; j < balls.Count; j++)
                {
                    // If a ball goes in pocket and the cueball is in play
                    if ((holes[i] - balls[j]).Magnitude < holes[i].Radius && cueBallOut == false)
                    {
                        // If the ball isn't the cue ball or the eight ball
                        if (j != 0 && j != 8)
                        {
                            // If the playerturn hasnt been set sets it to the color of the ball that went in. Switches turns if the ball they hit in isn't there color.
                            if (playerTurn == "")
                            {
                                if (j < 8)
                                    playerTurn = "Red";
                                if (j > 8)
                                    playerTurn = "Blue";
                            }

                            // If the player got one of there balls in it will stay their turn
                            if ((j < 8 && playerTurn == "Red") || (j > 8 && playerTurn == "Blue"))
                                gotBallIn = true;

                            // Gets the ball off of the pooltable.
                            balls[j].Center = new Point2D(-100, -100);
                            balls[j].Velocity = new Point2D(0, 0);
                            balls[j].Mass = double.MaxValue;

                            if (j < 8)
                            {
                                scoreRed++;
                            }
                            if (j > 8)
                            {
                                scoreBlue++;
                            }
                        }
                        // If the cue ball went in. Switches turns right away.
                        else if (j == 0)
                        {
                            SwitchTurns();

                            if (player == 1) // If its the players turn to place the cueball
                            {
                                cueBallOut = true;
                                balls[j].Center = new Point2D(-1000, -1000);
                                balls[j].Velocity = new Point2D(0, 0);
                            }
                            else if (player == 2) // If its the computers turn to place the cueball
                            {
                                cueBallOut = true;
                                balls[j].Center = new Point2D(-1000, -1000);
                                balls[j].Velocity = new Point2D(0, 0);
                            }
                        }
                        // If the eight ball went in.
                        else if (j == 8)
                        {
                            checkWin = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Switches whos turn it is to hit the ball
        /// </summary>
        public void SwitchTurns()
        {
            if (player == 1)
                player = 2;
            else if (player == 2)
                player = 1;

            if (playerTurn == "Red")
                playerTurn = "Blue";
            else if (playerTurn == "Blue")
                playerTurn = "Red";
        }

        /// <summary>
        /// Bounces the balls off the pooltable bumpers
        /// </summary>
        /// <param name="balls">Balls on the pooltable</param>
        public void Bounce(List<Ball2D> balls)
        {
            foreach (Line2D line in lines)
                foreach (Ball2D ball in balls)
                    line.Bounce(ball);
        }
        #endregion
    }
}
