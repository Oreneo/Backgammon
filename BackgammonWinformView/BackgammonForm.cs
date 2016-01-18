using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackgammonLogic;
using System.Threading;

namespace BackgammonWinformView
{
    public partial class BackgammonForm : Form
    {
        public BackgammonController GameController { get; private set; }

        public bool HighlightTriangleRequired { get; private set; }

        public bool CancelHighlightRequired { get; private set; }

        public bool HighlightRedBarRequired { get; private set; }

        public bool HighlightBlackBarRequired { get; private set; }

        public bool DrawRedBarRequired { get; private set; }

        public bool DrawBlackBarRequired { get; private set; }

        public List<PictureBox> TrianglesPictureBoxes { get; private set; }

        private const int drawCheckerShiftX = 6;
        private const int checkerRadiusSize = 53;

        public BackgammonForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        // All triangles invoke this event when clicked.
        private void TrianglePictureBox_Click(object sender, EventArgs e)
        {
            int clickedTriangle = FindClickedTriangle((PictureBox)sender);

            if (GameController.RolledDice == true)
            {
                if (GameController.PlayerInitialTriangleChoice == null)    // no choice 'from' has been made yet. trying to determine player's initial checker choice.
                {
                    TryGetAndExecuteInitialMove(clickedTriangle);
                }
                else if (clickedTriangle == GameController.PlayerInitialTriangleChoice)  // cancel 'from'
                {
                    CancelInitialMove();
                }
                else   // wants to move 'to' here
                {
                    TryGetAndExecuteFinalMove(clickedTriangle);
                }
            }
            else
            {
                MessageBox.Show(string.Format("{0} you need to roll the dice first !", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name));
            }
        }

        private void TryGetAndExecuteFinalMove(int clickedTriangle)
        {
            if (GameController.IsLegalFinalMove(clickedTriangle))      // trying to move to his color or empty slot
            {
                HandleLegalFinalMove(clickedTriangle);
            }
            else if (GameController.IsLegalFinalMoveEat(clickedTriangle))   // trying to eat.
            {
                HandleLegalFinalMoveEat(clickedTriangle);
            }
            else
            {
                msgLabel.Text = string.Format("You can't move there, {0}. Waiting for your move.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
        }

        private void HandleLegalFinalMoveEat(int clickedTriangle)
        {
            if (GameController._RedPlayer.IsMyTurn)
            {
                DrawBlackBarRequired = true;   // indication for paint event (refresh)
                blackBarHalfPictureBox.Visible = false;
            }
            else
            {
                DrawRedBarRequired = true;
                redBarHalfPictureBox.Visible = false;
            }

            GameController.SetPlayerFinalMoveEat(clickedTriangle);  // update cube.... move to bar.

            if (GameController.PlayerHasAvailableMoves() && GameController.MovesLeft > 0)    // including moves from the bar if he's stuck there.
            {
                msgLabel.Text = string.Format("Choose from where to move your checkers, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
            else if (!GameController.PlayerHasAvailableMoves() && GameController.MovesLeft != 0) // no moves for this guy. 
            {
                GameController.ResetMovesLeft();
                MessageBox.Show(string.Format("You have no available moves, {0}. Your turn is forfeit.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name));
            }
            if (GameController.MovesLeft == 0)
            {
                GameController.SwapTurns();
            }

            RefreshUserInterfaceAfterTurn();
        }

        private void HandleLegalFinalMove(int clickedTriangle)
        {
            GameController.SetPlayerFinalMove(clickedTriangle, false);  // update cube

            if (GameController.PlayerHasAvailableMoves() && GameController.MovesLeft > 0)    // including moves from the bar if he's stuck there.
            {
                msgLabel.Text = string.Format("Choose from where to move your checkers, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
            else if (!GameController.PlayerHasAvailableMoves() && GameController.MovesLeft != 0) // no moves for this guy. 
            {
                GameController.ResetMovesLeft();
                MessageBox.Show(string.Format("You have no available moves, {0}. Your turn is forfeit.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name));
            }

            if (GameController.MovesLeft == 0)
            {
                GameController.SwapTurns();
            }

            RefreshUserInterfaceAfterTurn();
        }

        private void CancelInitialMove()
        {
            msgLabel.Text = string.Format("Choose from where to move your checkers, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            GameController.SetPlayerInitialMove(null);

            CancelHighlightRequired = true;
            boardPictureBox.Refresh();
            CancelHighlightRequired = false;
        }

        private void TryGetAndExecuteInitialMove(int clickedTriangle)
        {
            if (GameController.IsLegalInitialMove(clickedTriangle))
            {
                GameController.SetPlayerInitialMove(clickedTriangle);
                msgLabel.Text = string.Format("Waiting for {0} move...", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);

                HighlightTriangleRequired = true;
                boardPictureBox.Refresh();
                HighlightTriangleRequired = false;
            }
            else if ((GameController._RedPlayer.IsMyTurn == true && GameController.GameBoard.GameBar.NumOfRedCheckers > 0) ||
                     (GameController._BlackPlayer.IsMyTurn == true && GameController.GameBoard.GameBar.NumOfBlackCheckers > 0))
            {
                msgLabel.Text = string.Format("You can't move checkers while you have checkers on the Bar, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
            else if (GameController.GameBoard.Triangles[clickedTriangle].CheckersColor == null)   // empty triangle (initial choice)
            {
                msgLabel.Text = string.Format("You don't have any checkers there, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
            else
            {
                msgLabel.Text = string.Format("Those are the enemy's checkers, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
        }

        private void RefreshUserInterfaceAfterTurn()
        {
            boardPictureBox.Refresh();
            sidePanel.Refresh();

            msgLabel.Text = string.Format("Choose from where to move your checkers, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);

            if (GameController.MovesLeft == 0)
            {
                ClearDiceImages();
                GameController.RolledDice = false;
                msgLabel.Text = string.Format("{0} its your turn. Please roll the dice.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
        }

        private void boardPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (DrawRedBarRequired == true)
            {
                DrawCheckersInRedBar(e, false);
                redBarHalfPictureBox.Visible = true;
            }
            if (DrawBlackBarRequired == true)
            {
                DrawCheckersInBlackBar(e, false);
                blackBarHalfPictureBox.Visible = true;
            }
            if(HighlightRedBarRequired == true)
            {
                DrawCheckersInRedBar(e, true);
                redBarHalfPictureBox.Visible = true;
            }
            if(HighlightBlackBarRequired == true)
            {
                DrawCheckersInBlackBar(e, true);
                blackBarHalfPictureBox.Visible = true;
            }

            DrawTriangles(e);
            DisplayWhosTurn(e);

            if(HighlightTriangleRequired == true)
            {
                if (GameController.PlayerInitialTriangleChoice != null)   // a choice was made 'from' but not 'to' (yet)
                {
                    HighlightChosenTriangle(e, GameController.PlayerInitialTriangleChoice.Value);
                }
            }

            if (CancelHighlightRequired == true)
            {
                if (GameController.PlayerInitialTriangleChoice != null)   // a choice was made 'from' but not 'to' (yet)
                {
                    CancelHighlightedTriangle(e, GameController.PlayerInitialTriangleChoice.Value);
                }
            }
        }

        private void DrawCheckersInRedBar(PaintEventArgs e, bool highlight)
        {
            int drawShiftY = SetDrawShiftY(GameController.GameBoard.GameBar.NumOfRedCheckers);

            for (int i = 0; i < GameController.GameBoard.GameBar.NumOfRedCheckers; i++)
            {
                DrawChecker(e, CheckerColor.Red, redBarHalfPictureBox.Location.X, redBarHalfPictureBox.Location.Y + drawShiftY * i, highlight);
            }
        }

        private void DrawCheckersInBlackBar(PaintEventArgs e, bool highlight)
        {
            int drawShiftY = SetDrawShiftY(GameController.GameBoard.GameBar.NumOfBlackCheckers);

            for (int i = 0; i < GameController.GameBoard.GameBar.NumOfBlackCheckers; i++)
            {
                DrawChecker(e, CheckerColor.Black, blackBarHalfPictureBox.Location.X, blackBarHalfPictureBox.Location.Y + blackBarHalfPictureBox.Size.Height - checkerRadiusSize - drawShiftY * i, highlight);
            }
        }

        private void DrawCheckersInRedBearOffPictureBox(PaintEventArgs e)
        {
            if (GameController._RedPlayer.CanBearOffCheckers(GameController.GameBoard))
            {
                GameController._RedPlayer.UpdateCheckersAtHome(GameController.GameBoard);

                int drawShiftY = SetDrawShiftY(15 - GameController._RedPlayer.CheckersAtHome);

                for (int i = 0; i < 15 - GameController._RedPlayer.CheckersAtHome; i++)
                {
                    DrawChecker(e, CheckerColor.Red, redCheckersBearOff.Location.X, redCheckersBearOff.Location.Y + drawShiftY * i, false);
                }
            }
        }

        private void DrawCheckersInBlackBearOffPictureBox(PaintEventArgs e)
        {
            if (GameController._BlackPlayer.CanBearOffCheckers(GameController.GameBoard))
            {
                GameController._BlackPlayer.UpdateCheckersAtHome(GameController.GameBoard);

                int drawShiftY = SetDrawShiftY(15 - GameController._BlackPlayer.CheckersAtHome);

                for (int i = 0; i < 15 - GameController._BlackPlayer.CheckersAtHome; i++)
                {
                    DrawChecker(e, CheckerColor.Black, blackCheckersBearOff.Location.X, blackCheckersBearOff.Location.Y + drawShiftY * i, false);
                }
            }
        }

        private void DrawTriangles(PaintEventArgs e)
        {
            for (int i = 0; i < GameController.GameBoard.Triangles.Count; i++)
            {
                DrawCheckersInTriangle(e, i, false);
            }
        }

        private void DrawCheckersInTriangle(PaintEventArgs e, int triangleNum, bool highlight)
        {
            int drawShiftY = SetDrawShiftY(GameController.GameBoard.Triangles[triangleNum].NumOfCheckers);

            if (GameController.GameBoard.Triangles[triangleNum].CheckersColor != null)  // empty triangle = nothing to draw.
            {
                if (triangleNum >= 12 && triangleNum <= 23)
                {
                    for (int i = 0; i < GameController.GameBoard.Triangles[triangleNum].NumOfCheckers; i++)
                    {
                        DrawChecker(e, GameController.GameBoard.Triangles[triangleNum].CheckersColor.Value, TrianglesPictureBoxes[triangleNum].Location.X + drawCheckerShiftX, TrianglesPictureBoxes[triangleNum].Location.Y + drawShiftY * i, highlight);
                    }
                }
                else  // bottom triangles.
                {
                    for (int i = 0; i < GameController.GameBoard.Triangles[triangleNum].NumOfCheckers; i++)
                    {
                        DrawChecker(e, GameController.GameBoard.Triangles[triangleNum].CheckersColor.Value, TrianglesPictureBoxes[triangleNum].Location.X + drawCheckerShiftX, TrianglesPictureBoxes[triangleNum].Location.Y + TrianglesPictureBoxes[triangleNum].Size.Height - checkerRadiusSize - drawShiftY * i, highlight);
                    }
                }
            }
        }

        private void HighlightChosenTriangle(PaintEventArgs e, int triangleNum)
        {
            DrawCheckersInTriangle(e, triangleNum, true);
        }

        private void CancelHighlightedTriangle(PaintEventArgs e, int triangleNum)
        {
            DrawCheckersInTriangle(e, triangleNum, false);
        }

        private int SetDrawShiftY(int numOfCheckers)
        {
            int drawShiftY = 0;

            switch (numOfCheckers)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:  drawShiftY = 53;
                         break;
                case 6:  drawShiftY = 48;
                         break;
                case 7:  drawShiftY = 41;
                         break;
                case 8:  drawShiftY = 37;
                         break;
                case 9:  drawShiftY = 33;
                         break;
                case 10: drawShiftY = 29;
                         break;
                case 11: drawShiftY = 26;
                         break;
                case 12: drawShiftY = 24;
                         break;
                case 13: drawShiftY = 21;
                         break;
                case 14: drawShiftY = 20;
                         break;
                case 15: drawShiftY = 19;
                         break;
            }

            return drawShiftY;
        }

        private void InitializeCustomComponents()
        {
            GameController = new BackgammonController();

            TrianglesPictureBoxes = new List<PictureBox>(new PictureBox[24]);
            InitializePictureBoxListReferences();

            for (int i = 0; i < TrianglesPictureBoxes.Count; i++)
            {
                InitializePictureBoxTriangle(i);
            }

            blackBarHalfPictureBox.BackColor = Color.Transparent;
            blackBarHalfPictureBox.Parent = boardPictureBox;

            redBarHalfPictureBox.BackColor = Color.Transparent;
            redBarHalfPictureBox.Parent = boardPictureBox;

            redCheckersBearOff.BackColor = Color.Transparent;
            blackCheckersBearOff.BackColor = Color.Transparent;
            redCheckersBearOff.Visible = false;
            blackCheckersBearOff.Visible = false;
        }

        private void InitializePictureBoxTriangle(int index)
        {             
            TrianglesPictureBoxes[index].Click += new EventHandler(this.TrianglePictureBox_Click);
            TrianglesPictureBoxes[index].BackColor = Color.Transparent;
            TrianglesPictureBoxes[index].Parent = boardPictureBox;
        }

        private int FindClickedTriangle(PictureBox clickedPictureBox)
        {
            for (int i = 0; i <= 23; i++)
            {
                if ((TrianglesPictureBoxes[i].Left <= clickedPictureBox.Left) && (TrianglesPictureBoxes[i].Top <= clickedPictureBox.Top) && (TrianglesPictureBoxes[i].Bottom >= clickedPictureBox.Bottom) && (TrianglesPictureBoxes[i].Right >= clickedPictureBox.Right))
                {
                    return i;
                }
            }
            return -1;
        }

        private void DrawChecker(PaintEventArgs e, CheckerColor checkerColor, int x, int y, bool highlight)
        {
            // Create a path that consists of a single ellipse.
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(x, y, checkerRadiusSize, checkerRadiusSize);

            // Use the path to construct a brush.
            PathGradientBrush pthGrBrush = new PathGradientBrush(path);

            // Set the color at the center of the path.
            pthGrBrush.CenterColor = Color.FromArgb(215, 225, 255, 255);

            // Set the color along the entire boundary of the path. // 2nd argument is 255 - red or 0 - black
            if (highlight == false)
            {
                Color[] colors = { Color.FromArgb(255, checkerColor == CheckerColor.Red ? 255 : 0, 0, 0) };
                pthGrBrush.SurroundColors = colors;
                e.Graphics.FillEllipse(pthGrBrush, x, y, checkerRadiusSize, checkerRadiusSize);
            }
            else
            {
                Color[] colors = { Color.FromArgb(255, checkerColor == CheckerColor.Red ? 255 : 22, 22, 255) };
                pthGrBrush.SurroundColors = colors;
                e.Graphics.FillEllipse(pthGrBrush, x, y, checkerRadiusSize, checkerRadiusSize);
            }
        }

        private void InitializePictureBoxListReferences()
        {
            TrianglesPictureBoxes[0] = pictureBox0;
            TrianglesPictureBoxes[1] = pictureBox1;
            TrianglesPictureBoxes[2] = pictureBox2;
            TrianglesPictureBoxes[3] = pictureBox3;
            TrianglesPictureBoxes[4] = pictureBox4;
            TrianglesPictureBoxes[5] = pictureBox5;
            TrianglesPictureBoxes[6] = pictureBox6;
            TrianglesPictureBoxes[7] = pictureBox7;
            TrianglesPictureBoxes[8] = pictureBox8;
            TrianglesPictureBoxes[9] = pictureBox9;
            TrianglesPictureBoxes[10] = pictureBox10;
            TrianglesPictureBoxes[11] = pictureBox11;
            // Upper
            TrianglesPictureBoxes[12] = pictureBox12;
            TrianglesPictureBoxes[13] = pictureBox13;
            TrianglesPictureBoxes[14] = pictureBox14;
            TrianglesPictureBoxes[15] = pictureBox15;
            TrianglesPictureBoxes[16] = pictureBox16;
            TrianglesPictureBoxes[17] = pictureBox17;
            TrianglesPictureBoxes[18] = pictureBox18;
            TrianglesPictureBoxes[19] = pictureBox19;
            TrianglesPictureBoxes[20] = pictureBox20;
            TrianglesPictureBoxes[21] = pictureBox21;
            TrianglesPictureBoxes[22] = pictureBox22;
            TrianglesPictureBoxes[23] = pictureBox23;
        }

        private void roleDiceButton_Click(object sender, EventArgs e)
        {
            if (GameController.RolledDice == false)
            {
                GameController.GetDiceRolls();

                LoadDieImage(GameController.GameDice.FirstCube, firstDiePictureBox);
                LoadDieImage(GameController.GameDice.SecondCube, secondDiePictureBox);
            }
            else
            {
                MessageBox.Show(string.Format("{0} you have already rolled !", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name));
            }

            EvaluateInitialAvailableMoves();
        }

        private void EvaluateInitialAvailableMoves()
        {
            if (GameController.PlayerHasAvailableMoves() || GameController.PlayerHasAvailableBearOffMoves())    // including moves from the bar if he's stuck there.
            {
                GameController.UpdateMovesLeft();
                msgLabel.Text = string.Format("Choose from where to move your checkers, {0}.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name);
            }
            else  // no moves for this guy. 
            {
                MessageBox.Show(string.Format("You have no available moves, {0}. Your turn is forfeit.", GameController._RedPlayer.IsMyTurn ? GameController._RedPlayer.Name : GameController._BlackPlayer.Name));
                GameController.SwapTurns();
                RefreshUserInterfaceAfterTurn();
            }
        }

        private void LoadDieImage(int firstDieNum, PictureBox diePictureBox)
        {
            switch (firstDieNum)
            {
                case 1: diePictureBox.Image = global::BackgammonWinformView.Properties.Resources.Die1;
                    break;

                case 2: diePictureBox.Image = global::BackgammonWinformView.Properties.Resources.Die2;
                    break;

                case 3: diePictureBox.Image = global::BackgammonWinformView.Properties.Resources.Die3;
                    break;

                case 4: diePictureBox.Image = global::BackgammonWinformView.Properties.Resources.Die4;
                    break;

                case 5: diePictureBox.Image = global::BackgammonWinformView.Properties.Resources.Die5;
                    break;

                case 6: diePictureBox.Image = global::BackgammonWinformView.Properties.Resources.Die6;
                    break;

                default:
                    break;
            }
        }

        private void ClearDiceImages()
        {
            firstDiePictureBox.Image = null;
            secondDiePictureBox.Image = null;
        }

        private void DisplayWhosTurn(PaintEventArgs e)
        {
            DrawChecker(e, GameController._RedPlayer.IsMyTurn ? CheckerColor.Red : CheckerColor.Black, 1118, 3, false);
        }

        private void blackBarHalfPictureBox_Click(object sender, EventArgs e)
        {
            if (GameController.GameBoard.GameBar.NumOfBlackCheckers > 0 && GameController._BlackPlayer.IsMyTurn == true/* && GameController.RolledDice == true*/)
            {
                if (GameController.RolledDice == true)
                {
                    GameController.SetPlayerInitialMove(24);
                    HighlightBlackBarRequired = true;
                    boardPictureBox.Refresh();
                    HighlightBlackBarRequired = false;
                }
                else
                {
                    MessageBox.Show(string.Format("{0} you need to roll the dice first !", GameController._BlackPlayer.Name));
                }
            }
        }

        private void redBarHalfPictureBox_Click(object sender, EventArgs e)
        {
            if(GameController.GameBoard.GameBar.NumOfRedCheckers > 0 && GameController._RedPlayer.IsMyTurn == true/* && GameController.RolledDice == true*/)
            {
                if (GameController.RolledDice == true)
                {
                    GameController.SetPlayerInitialMove(-1);
                    HighlightRedBarRequired = true;
                    boardPictureBox.Refresh();
                    HighlightRedBarRequired = false;
                }
                else
                {
                    MessageBox.Show(string.Format("{0} you need to roll the dice first !", GameController._RedPlayer.Name));
                }
            }
        }

        private void sidePanel_Paint(object sender, PaintEventArgs e)
        {
            DrawCheckersInBlackBearOffPictureBox(e);
            DrawCheckersInRedBearOffPictureBox(e);
        }

        private void sidePanel_Click(object sender, EventArgs e)
        {
            int cubeUsed;

            if (GameController._RedPlayer.IsMyTurn)
            {
                if (GameController._RedPlayer.CanBearOffCheckers(GameController.GameBoard))
                {
                    if (GameController.PlayerInitialTriangleChoice != null)   // made a 'from' choice. nullify at the end... 
                    {
                        if(GameController.IsLegalBearOffMove(out cubeUsed))
                        {
                            GameController.SetPlayerBearOffMove(cubeUsed);  // update cube

                            if (GameController.MovesLeft == 0)
                            {
                                GameController.SwapTurns();
                            }

                            RefreshUserInterfaceAfterTurn();

                            GameController._RedPlayer.UpdateCheckersAtHome(GameController.GameBoard);

                            if (GameController._RedPlayer.CheckersAtHome == 0)
                            {
                                MessageBox.Show("Game over - Red player is the winner !");
                            }
                        }
                    }
                }
                else  // can't bear off.
                {
                    MessageBox.Show(string.Format("You can't bear off any checkers yet, {0}.", GameController._RedPlayer.Name));
                }
            }
            else   // black turn.
            {
                if (GameController._BlackPlayer.CanBearOffCheckers(GameController.GameBoard))
                {
                    if (GameController.PlayerInitialTriangleChoice != null)   // made a 'from' choice. 
                    {
                        if (GameController.IsLegalBearOffMove(out cubeUsed))
                        {
                            GameController.SetPlayerBearOffMove(cubeUsed);  // update cube

                            if (GameController.MovesLeft == 0)
                            {
                                GameController.SwapTurns();
                            }

                            RefreshUserInterfaceAfterTurn();

                            GameController._BlackPlayer.UpdateCheckersAtHome(GameController.GameBoard);

                            if(GameController._BlackPlayer.CheckersAtHome == 0)
                            {
                                MessageBox.Show("Game over - Black player is the winner !");
                            }
                        }
                    }
                }
                else  // can't bear off.
                {
                    MessageBox.Show(string.Format("You can't bear off any checkers yet, {0}.", GameController._RedPlayer.Name));
                }
            }
        }
    }
}