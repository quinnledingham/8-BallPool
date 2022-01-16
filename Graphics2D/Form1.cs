using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Graphics2D
{
    public partial class Form1 : Form
    {
        // Creating variables
        List<Ball2D> balls = new List<Ball2D>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };
        List<Line2D> lines = new List<Line2D>();
        Bitmap background = new Bitmap(Graphics2D.Properties.Resources.pooltable);
        PoolTable poolTable = null;
        PointF firstPoint = new PointF((float)1135.5, (float)426.5);
        CueStick stick = null;
        int radius = 17;
        Point2D clickDown = new Point2D();
        Sound sound = new Sound();

        bool gameStarted = false;
        RectangleF startingZone = new RectangleF(new PointF(76, 75), new Size(351, 703));
        RectangleF inPlayZone = new RectangleF(new PointF(76, 75), new Size(1409, 703));
        bool cueBallHit = false;

        List<Point2D> pocketTargets = new List<Point2D>();

        bool drawPossibleShots = false;
        List<Point2D> possibleShots = new List<Point2D>();

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(background.Width, background.Height);
            // write initialization code here
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Restart();
        }

        /// <summary>
        /// Resets everything so the game can be played again
        /// </summary>
        private void Restart()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i] = null;
            }

            poolTable = new PoolTable();
            stick = null;
            gameStarted = false;
            cueBallHit = false;

            // Places the score labels
            lblPlayerTurn.Location = new Point(150, 20);

            pocketTargets.Add(new Point2D(90, 95));
            pocketTargets.Add(new Point2D(781, 72));
            pocketTargets.Add(new Point2D(1467, 95));
            pocketTargets.Add(new Point2D(1467, 760));
            pocketTargets.Add(new Point2D(781, 790));
            pocketTargets.Add(new Point2D(90, 760));

            // Inserts the cue ball to the hand
            balls.Insert(0, new Ball2D(poolTable.CueHand, radius, new Point2D(0, 0)));
            balls.RemoveAt(16);
            setupBalls();
        }

        /// <summary>
        /// Checks when someone has hit the eightball in if they won or have now lost
        /// </summary>
        private void CheckWin()
        {
            if ((poolTable.PlayerTurn == "Red" && poolTable.ScoreRed == 7) || (poolTable.PlayerTurn == "Blue" && poolTable.ScoreBlue == 7))
            {
                DialogResult result = MessageBox.Show("Player " + poolTable.Player + " Wins. Do you want to play again?", "Game Over", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Restart();
                }
                else
                {
                    this.Dispose();
                }
            }
            else
            {
                poolTable.SwitchTurns();
                DialogResult result = MessageBox.Show("Player " + poolTable.Player + " Wins. Do you want to play again?", "Game Over", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    Restart();
                }
                else
                {
                    this.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates the triangle of balls
        /// </summary>
        private void setupBalls()
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    for (int j = 4 - i; j < 5; j++)
            //    {
            //        balls.Add(new Ball2D(new Point2D(firstPoint.X + (-3 * i) + (radius * i) * 2, 480 + (radius * i) + (radius * 2 * j)), radius, new Point2D(0, 0)));
            //    }
            //}

            //Saves where the eightball should be
            Point2D eightBall = new Point2D();

            //Places the balls in a triangle in a random order. Creates the list of balls in a random order as they are added
            for (int i = 0; i < 5; i++)
            {
                for (int j = 4 - i; j < 5; j++)
                {
                    //Variables reset every loop
                    int random = 0;
                    bool openSpot = false;

                    //Pick the random spot to place the ball in the list
                    do
                    {
                        //Random variable picks a number
                        Random r = new Random();
                        random = r.Next(1, 15);

                        //Checks if the spot hasnt already been filled
                        if (balls[random] == null)
                            openSpot = true;
                        else
                            openSpot = false;
                    } while (openSpot == false);

                    //Places the ball in the random spot on the list and places it in the next spot on the triangle. 
                    balls.Insert(random, new Ball2D(new Point2D(firstPoint.X + 2.00001 * radius * Math.Sin(Math.PI / 3) * i, firstPoint.Y + 0.00001 - i * radius + (4 - j) * radius * 2), radius, new Point2D(0, 0)));

                    //If the ball added is in the eightball spot save the location of it
                    if (i == 2 && j == 3)
                    {
                        eightBall = balls[random].Center;
                    }
                }
            }

            // Gets rid of any blank spots left
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i] == null)
                {
                    balls.RemoveAt(i);
                    i = i - 2;
                }
            }

            // Puts the eighth ball in the list in the eightball spot of the triangle
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].Center.Magnitude == eightBall.Magnitude)
                {
                    balls[i].Center = balls[8].Center;
                    balls[8].Center = eightBall;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Depending on what key is pressed do something
            switch (e.KeyCode)
            {
                case Keys.Escape: //Close the program
                    this.Dispose();
                    break;
                case Keys.Space:
                    timer1.Enabled = !timer1.Enabled; // toggle the timer
                    break;
                case Keys.S: // a single step
                    timer1_Tick(null, null);
                    break;
                case Keys.D: // Possible shots for the computer
                    drawPossibleShots = true;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (poolTable.CheckWin)
                CheckWin();

            if (poolTable.CueBallOut == true && IsBallsStopped() == true)
                balls[0].Center = poolTable.CueHand;

            // Every 33 milliseconds do these things
            // Update the score
            lblPlayerTurn.Text = poolTable.Player + ": " + poolTable.PlayerTurn;

            // Switch turns if the balls are stopped and a ball of the players color has not gone in after hitting the cue ball
            foreach (Ball2D ball in balls)
            {
                if (ball.Velocity.Magnitude != 0)
                    break;

                if (cueBallHit == true && IsBallsStopped() && poolTable.GotBallIn == false && poolTable.CueBallOut == false)
                {
                    poolTable.SwitchTurns();
                    cueBallHit = false;
                }
            }

            // Computers turn to shoot the ball
            if (poolTable.Player == 2 && IsBallsStopped())
            {
                // If the computer has to place the cueball it picks a random spot on the table
                if (poolTable.CueBallOut == true)
                {
                    bool openSpot;
                    double x, y;
                    do
                    {
                        openSpot = true;
                        Random r = new Random();
                        x = r.Next(Convert.ToInt16(inPlayZone.Left), Convert.ToInt16(inPlayZone.Right));
                        Random t = new Random();
                        y = t.Next(Convert.ToInt16(inPlayZone.Top), Convert.ToInt16(inPlayZone.Bottom));

                        foreach (Ball2D ball in balls)
                        {
                            if (x - balls[0].X < balls[0].Radius && y - balls[0].Y < balls[0].Radius)
                                openSpot = false;
                        }
                    } while (openSpot == false);

                    poolTable.CueBallOut = false;
                    balls[0].Center = new Point2D(x, y);
                }
                // Show where the ball is then shoot it
                this.Refresh();
                Thread.Sleep(1000);
                ComputerShoot();
                cueBallHit = true;
                poolTable.GotBallIn = false;
            }

            // Moves each ball in the balls list
            foreach (Ball2D ball in balls)
                ball.Move();

            // Checks if the tip of the cue stick hit. Bounces the ball off the tip and sets variables to tell other code this 
            if (stick != null)
            {
                for (int i = 0; i < balls.Count; i++)
                {
                    if (balls[i].IsColliding(stick.Tip))
                    {
                        PlaySound();
                        cueBallHit = true;
                        poolTable.GotBallIn = false;
                        if (i == 0)
                            balls[i].Bounce(stick.Tip);
                        stick = null;
                        break;
                    }
                }
                if (stick != null)
                    stick.Move();
            }

            // If the cue ball is in play the balls with bounce of each other. Plays sound.
            if (poolTable.CueBallOut == false || IsBallsStopped() == false)
                for (int i = 0; i < balls.Count; i++)
                    for (int j = i + 1; j < balls.Count; j++)
                    {
                        if (balls[i].IsColliding(balls[j]))
                            PlaySound();

                        balls[i].Bounce(balls[j]);
                    }

            // Bounces balls off the walls and removes them if they fall in a pocket
            poolTable.Bounce(balls);
            poolTable.RemoveBall(balls, inPlayZone);

            this.Invalidate();
        }

        /// <summary>
        /// Checks if all the balls are not moving
        /// </summary>
        /// <returns></returns>
        private bool IsBallsStopped()
        {
            foreach (Ball2D ball in balls)
                if (ball.Velocity.Magnitude != 0)
                    return false;
            return true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the pool table image
            e.Graphics.DrawImage(background, 0, 0);

            // Draw all the balls. draws the balls different colours based on where they are in the list.
            for (int i = 0; i < balls.Count; i++)
            {
                if (i < 8)
                    balls[i].Draw(e.Graphics, Color.Red);
                if (i > 8)
                    balls[i].Draw(e.Graphics, Color.Blue);
                if (i == 0)
                    if (poolTable.CueBallOut == false)
                        balls[i].Draw(e.Graphics, Color.White);
                    else if (poolTable.CueBallOut == true)
                        balls[i].Draw(e.Graphics, Color.Gray);
                if (i == 8)
                    balls[i].Draw(e.Graphics, Color.Black);

                // Draws the numbers on the balls
                if (i != 0)
                    DrawString(i.ToString(), new PointF((float)balls[i].X, (float)balls[i].Y), e.Graphics);
            }

            if (drawPossibleShots == true)
            {
                FindBestShot();
                foreach (Point2D shot in possibleShots)
                {
                    Line2D shotLine = new Line2D(balls[0], shot);
                    shotLine.Pen.Width = 1;
                    shotLine.Pen.Color = Color.Yellow;
                    shotLine.Draw(e.Graphics);
                }
            }

            // Draw the lines           
            foreach (Line2D line in lines)
            {
                line.Draw(e.Graphics);
                line.Pen.Width = 7;
                line.Pen.Color = Color.Brown;
            }

            // Draw the cue stick if one exists
            if (stick != null)
                stick.Draw(e.Graphics);

            poolTable.PoolTableDraw(e.Graphics); // Draws the pool table
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (poolTable.Player == 1)
            {
                // If the left mouse button is clicked and checks if the cue ball is in play
                if (e.Button == MouseButtons.Left && balls[0].Velocity.Magnitude == 0 && poolTable.CueBallOut == false)
                {
                    // Creates a new stick at where the mouse is clicked down
                    stick = null;
                    stick = new CueStick(new Point2D(e.X, e.Y), new Point2D(e.X, e.Y));
                    clickDown = new Point2D(e.X, e.Y);
                }
                else if (e.Button == MouseButtons.Left && poolTable.CueBallOut == true && IsBallsStopped() == true)
                {
                    // If the cue ball is out of played and checks if the cue ball has been hit once. If it has been it has to be placed on the green
                    if (gameStarted == true && inPlayZone.Left + radius < e.X && inPlayZone.Right > e.X && inPlayZone.Top + radius < e.Y && inPlayZone.Bottom - radius > e.Y)
                    {
                        PlaceCueBall(new Point2D(e.X, e.Y));
                    }
                    // If the game hasnt started the cue ball can only be placed behind the line
                    else if (gameStarted == false && startingZone.Left + radius < e.X && startingZone.Right > e.X && startingZone.Top + radius < e.Y && startingZone.Bottom - radius > e.Y)
                    {
                        PlaceCueBall(new Point2D(e.X, e.Y));
                        gameStarted = true;
                    }
                    cueBallHit = false;
                }
            }
        }

        /// <summary>
        /// Puts the cue ball down where the mouse is if there isn't a ball there
        /// </summary>
        /// <param name="e">Point of the mouse</param>
        private void PlaceCueBall(Point2D e)
        {
            for (int i = 1; i < 16; i++)
            {
                if (balls[i].IsColliding(balls[0]))
                    return;
            }
            balls[0].Center = new Point2D(e.X, e.Y);
            balls[0].Velocity = new Point2D(0, 0);
            poolTable.CueBallOut = false;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // Makes the cue stick move when the mouse button is let go of
            if (stick != null && e.Button == MouseButtons.Left)
                stick.Velocity(new Point2D(e.X, e.Y));
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // Puts the end of the stick where the mouse is if the ball is in play. If it is not puts the cue ball on the mouse.
            poolTable.CueHand = new Point2D(e.X, e.Y);
            if (e.Button == MouseButtons.Left && stick != null && poolTable.CueBallOut == false)
                stick.Hand = new Point2D(e.X, e.Y);
        }

        /// <summary>
        /// Draws the numbers on the balls
        /// </summary>
        /// <param name="text">Number to be drawn</param>
        /// <param name="location">Center of the ball to draw the number</param>
        /// <param name="gr">Graphics</param>
        public void DrawString(string text, PointF location, Graphics gr)
        {
            string drawString = text;
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            StringFormat drawFormat = new StringFormat();
            drawFormat.LineAlignment = StringAlignment.Near;
            drawFormat.Alignment = StringAlignment.Center;
            gr.DrawString(drawString, drawFont, drawBrush, (float)location.X, (float)location.Y - 10, drawFormat);
        }

        /// <summary>
        /// Plays a ball bouncing sound
        /// </summary>
        private void PlaySound()
        {
            sound.Play();
        }

        /// <summary>
        /// Checks if the path from a ball to a hole is clear
        /// </summary>
        /// <param name="ball">Ball that is being checked</param>
        /// <param name="target">Hole that is being aimed for</param>
        /// <returns></returns>
        private bool CheckPath(Ball2D ball, Point2D target)
        {
            // Make the exclusion polygon
            Point2D direction = target - ball;
            Point2D normal = direction.Normal;
            normal.Normalize(); // makes normal a unit vector
            Point2D[] path = new Point2D[4];
            path[0] = ball + 2 * radius * normal;
            path[1] = target + 2 * radius * normal;
            path[2] = target - 2 * radius * normal;
            path[3] = ball - 2 * radius * normal;

            foreach (Ball2D otherBall in balls)
            {
                if (otherBall == ball) continue;
                if (otherBall == balls[0]) continue;
                if (IsInPolygon(otherBall, path))
                    return false; // ball in the way
            }
            return true;
        }

        /// <summary>
        /// Checks if the ball is in the polygon/path
        /// </summary>
        /// <param name="otherBall">Ball that it is trying to it</param>
        /// <param name="path">space the ball needs to travel in</param>
        /// <returns></returns>
        private bool IsInPolygon(Ball2D otherBall, Point2D[] path)
        {
            if ((otherBall.Center.Y > path[2].Y && otherBall.Center.Y < path[0].Y) || (otherBall.Center.Y < path[2].Y && otherBall.Center.Y > path[0].Y))
                if ((otherBall.Center.X > path[2].X && otherBall.Center.X < path[0].X) || (otherBall.Center.X < path[2].X && otherBall.Center.X > path[0].X))
                    return true;
            return false;
        }

        /// <summary>
        /// Finds the best ball for the computer to hit with the cueball
        /// </summary>
        /// <returns>Coordinates of where the cueball should hit</returns>
        private Point2D FindBestShot()
        {
            possibleShots.Clear();
            double cosValue = double.MaxValue;
            Point2D bestTarget = null;
            // Goes through each ball and pocket to check if any balls are in the way
            foreach (Ball2D ball in balls)
                foreach (Point2D target in pocketTargets)
                {
                    Point2D direction = target - ball;
                    direction.Normalize();
                    Point2D cueBallTarget = ball - 2 * radius * direction;
                    if (CheckPath(balls[0], cueBallTarget) && cueBallTarget.X > 0)
                        if (CheckPath(ball, target))
                        {
                            for (int i = 0; i < balls.Count; i++)
                            {
                                // Makes the computer hit its colour and not the eightball until it can win
                                if ((poolTable.PlayerTurn == "Red" && i < 8 && balls[i] == ball) ||
                                    (poolTable.PlayerTurn == "Blue" && i > 8 && balls[i] == ball) ||
                                    (poolTable.PlayerTurn == "" && i != 8) ||
                                    (poolTable.PlayerTurn == "Red" && poolTable.ScoreRed == 7 && i == 8) ||
                                    (poolTable.PlayerTurn == "Blue" && poolTable.ScoreBlue == 7 && i == 8))
                                {
                                    possibleShots.Add(cueBallTarget);
                                    Point2D cueDirection = balls[0] - cueBallTarget;
                                    cueDirection.Normalize();
                                    double cos = cueDirection * direction;
                                    // If multiple possible shots are found it picks the straightest one
                                    if (cos < cosValue)
                                    {
                                        cosValue = cos;
                                        bestTarget = cueBallTarget;
                                    }
                                }
                            }
                        }
                }

            if (cosValue != double.MaxValue)
                return bestTarget;
            else
                // If no possible shot is found the closest ball is found
                return FindClosestBall();
        }

        /// <summary>
        /// Finds the ball that is the closest to the cueball
        /// </summary>
        /// <returns>Coordinates of the closest ball</returns>
        private Point2D FindClosestBall()
        {
            double distance = double.MaxValue;
            Point2D closestBall = null;
            foreach (Ball2D ball in balls)
            {
                if (ball != balls[0])
                    if ((ball - balls[0]).Magnitude < distance && ball.X > 0)
                    {
                        for (int i = 0; i < balls.Count; i++)
                        {
                            // Makes the computer hit its colour and not the eightball until it can win
                            if ((poolTable.PlayerTurn == "Red" && i < 8 && balls[i] == ball) ||
                                    (poolTable.PlayerTurn == "Blue" && i > 8 && balls[i] == ball) ||
                                    (poolTable.PlayerTurn == "" && i != 8) ||
                                    (poolTable.PlayerTurn == "Red" && poolTable.ScoreRed == 7 && i == 8) ||
                                    (poolTable.PlayerTurn == "Blue" && poolTable.ScoreBlue == 7 && i == 8))
                            {
                                possibleShots.Add(new Point2D(ball.X, ball.Y));
                                distance = ball.Magnitude - balls[0].Magnitude;
                                closestBall = ball;
                            }
                        }
                    }
            }
            possibleShots.Add(closestBall);
            return closestBall;
        }

        /// <summary>
        /// Shoots the cue ball to the best target found. Velocity is picked based on how far away the cue ball is from the target
        /// </summary>
        private void ComputerShoot()
        {
            Point2D direction = FindBestShot() - balls[0];
            if (direction.Magnitude > 300)
                //20 Steps
                balls[0].Velocity = -1 * direction / ((1 - Math.Pow(balls[0].Friction, 20) / (1 - balls[0].Friction)));
            else if (direction.Magnitude > 200)
                //10 Steps
                balls[0].Velocity = -1 * direction / ((1 - Math.Pow(balls[0].Friction, 10) / (1 - balls[0].Friction)));
            else
                //5 Steps
                balls[0].Velocity = -1 * direction / ((1 - Math.Pow(balls[0].Friction, 5) / (1 - balls[0].Friction)));
        }

        #region menustrips
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void restartToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutMyProgram = new AboutBox();
            aboutMyProgram.ShowDialog();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox aboutMyProgram = new AboutBox();
            aboutMyProgram.ShowDialog();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion
    }
}