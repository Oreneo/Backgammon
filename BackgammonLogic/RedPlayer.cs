using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public class RedPlayer : Player
    {
        public RedPlayer(string name, CheckerColor playerColor) : base(name, playerColor) 
        {

        }

        public override IEnumerable<KeyValuePair<int, int>> GetAvailableMoves(Board gameBoard, Dice gameDice)
        {
            if (gameBoard.GameBar.NumOfRedCheckers == 0)  // if he's stuck on the bar, no point counting his normal moves
            {
                List<KeyValuePair<int, int>> AvailableMoves = new List<KeyValuePair<int, int>>();

                for (int i = 0; i < gameBoard.Triangles.Count; i++)
                {
                    if (!gameDice.RolledDouble)  // not double. if double - only check once (next if)
                    {
                        if (i + gameDice.FirstCube <= 23 && gameDice.FirstCube != 0)  // don't step out of array boundries, check if cube was not 'reset' to 0
                        {
                            if (IsLegalPlayerInitialMove(gameBoard, i) && IsLegalPlayerFinalMove(gameBoard, i, i + gameDice.FirstCube, gameDice.FirstCube))
                            {
                                AvailableMoves.Add(new KeyValuePair<int, int>(i, i + gameDice.FirstCube));
                            }
                        }
                    }

                    if (i + gameDice.SecondCube <= 23 && gameDice.SecondCube != 0)  // don't step out of array boundries, check if cube was not 'reset' to 0)
                    {
                        // 2nd cube can also be an available 'move' (if he hasn't rolled double)
                        if (IsLegalPlayerInitialMove(gameBoard, i) && IsLegalPlayerFinalMove(gameBoard, i, i + gameDice.SecondCube, gameDice.SecondCube))
                        {
                            AvailableMoves.Add(new KeyValuePair<int, int>(i, i + gameDice.SecondCube));
                        }
                    }
                }

                return AvailableMoves;
            }
            else
            {
                return GetAvailableMovesFromBar(gameBoard, gameDice);
            }
        }

        public override IEnumerable<KeyValuePair<int, int>> GetAvailableMovesFromBar(Board gameBoard, Dice gameDice)
        {
            List<KeyValuePair<int, int>> AvailableMoves = new List<KeyValuePair<int, int>>();

            if (gameDice.FirstCube != 0)  // check if that cube wasn't reset
            {
                if (IsLegalPlayerFinalMove(gameBoard, -1, -1 + gameDice.FirstCube, gameDice.FirstCube))
                {
                    AvailableMoves.Add(new KeyValuePair<int, int>(-1, -1 + gameDice.FirstCube));
                }
            }

            if (gameDice.SecondCube != 0)
            {
                if (IsLegalPlayerFinalMove(gameBoard, -1, -1 + gameDice.SecondCube, gameDice.SecondCube))
                {
                    AvailableMoves.Add(new KeyValuePair<int, int>(-1, -1 + gameDice.SecondCube));
                }
            }

            return AvailableMoves;
        }

        public override IEnumerable<KeyValuePair<int, int>> GetAvailableMovesEat(Board gameBoard, Dice gameDice)
        {
            if (gameBoard.GameBar.NumOfRedCheckers == 0)  // if he's stuck on the bar, no point counting his normal moves
            {
                List<KeyValuePair<int, int>> AvailableMovesEat = new List<KeyValuePair<int, int>>();

                for (int i = 0; i < gameBoard.Triangles.Count; i++)
                {
                    if (!gameDice.RolledDouble)  // not double. if double - only check once (next if)
                    {
                        if (i + gameDice.FirstCube <= 23 && gameDice.FirstCube != 0)  // don't step out of array boundries, check if cube was not 'reset' to 0
                        {
                            if (IsLegalPlayerInitialMove(gameBoard, i) && IsLegalPlayerFinalMoveEat(gameBoard, i, i + gameDice.FirstCube, gameDice.FirstCube))
                            {
                                AvailableMovesEat.Add(new KeyValuePair<int, int>(i, i + gameDice.FirstCube));
                            }
                        }
                    }

                    if (i + gameDice.SecondCube <= 23 && gameDice.SecondCube != 0)  // don't step out of array boundries, check if cube was not 'reset' to 0
                    {
                        // 2nd cube can also be an available 'eat move'
                        if (IsLegalPlayerInitialMove(gameBoard, i) && IsLegalPlayerFinalMoveEat(gameBoard, i, i + gameDice.SecondCube, gameDice.SecondCube))
                        {
                            AvailableMovesEat.Add(new KeyValuePair<int, int>(i, i + gameDice.SecondCube));
                        }
                    }
                }

                return AvailableMovesEat;
            }
            else
            {
                return GetAvailableMovesEatFromBar(gameBoard, gameDice);
            }
        }

        public override IEnumerable<KeyValuePair<int, int>> GetAvailableMovesEatFromBar(Board gameBoard, Dice gameDice)
        {
            List<KeyValuePair<int, int>> AvailableMoves = new List<KeyValuePair<int, int>>();

            if (gameDice.FirstCube != 0)  // check if that cube wasn't reset
            {
                if (IsLegalPlayerFinalMoveEat(gameBoard, -1, -1 + gameDice.FirstCube, gameDice.FirstCube))
                {
                    AvailableMoves.Add(new KeyValuePair<int, int>(-1, -1 + gameDice.FirstCube));
                }
            }
            if (gameDice.SecondCube != 0)  // check if that cube wasn't reset
            {
                if (IsLegalPlayerFinalMoveEat(gameBoard, -1, -1 + gameDice.SecondCube, gameDice.SecondCube))
                {
                    AvailableMoves.Add(new KeyValuePair<int, int>(-1, -1 + gameDice.SecondCube));
                }
            }

            return AvailableMoves;
        }

        public override IEnumerable<KeyValuePair<int, int>> GetAvailableBearOffMoves(Board gameBoard, Dice gameDice)
        {
            List<KeyValuePair<int, int>> AvailableMoves = new List<KeyValuePair<int, int>>();

            for (int i = 18; i <= 23; i++)
            {
                if (!gameDice.RolledDouble)  // not double. if double - only check once (next if)
                {
                    if (gameDice.FirstCube != 0)  // check if cube was not 'reset' to 0
                    {
                        if (IsLegalPlayerInitialMove(gameBoard, i) && IsLegalPlayerBearOffMove(i, gameDice.FirstCube))
                        {
                            AvailableMoves.Add(new KeyValuePair<int, int>(i, gameDice.FirstCube));
                        }
                    }
                }

                if (gameDice.SecondCube != 0)  // check if cube was not 'reset' to 0
                {
                    // 2nd cube can also be an available 'move' (if he hasn't rolled double)
                    if (IsLegalPlayerInitialMove(gameBoard, i) && IsLegalPlayerBearOffMove(i, gameDice.SecondCube))
                    {
                        AvailableMoves.Add(new KeyValuePair<int, int>(i, gameDice.SecondCube));
                    }
                }
            }

            return AvailableMoves;
        }

        public override bool IsLegalPlayerInitialMove(Board gameBoard, int index)
        {
            if (gameBoard.Triangles[index].CheckersColor == CheckerColor.Red && gameBoard.GameBar.NumOfRedCheckers == 0)
            {
                return true;
            }

            return false;
        }

        public override bool IsLegalPlayerFinalMove(Board gameBoard, int fromIndex, int toIndex, int cubeNumber) // check if came from bar? -1...
        {
            if (toIndex - fromIndex == cubeNumber)
            {
                if (gameBoard.Triangles[toIndex].CheckersColor == null || gameBoard.Triangles[toIndex].CheckersColor == CheckerColor.Red)  // 2nd condition ?
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsLegalPlayerFinalMoveEat(Board gameBoard, int fromIndex, int toIndex, int cubeNumber) // check if came from bar? -1...
        {
            if (toIndex - fromIndex == cubeNumber)
            {
                if (gameBoard.Triangles[toIndex].CheckersColor == CheckerColor.Black && gameBoard.Triangles[toIndex].NumOfCheckers == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsLegalPlayerBearOffMove(int fromIndex, int cubeNumber)
        {
            if(fromIndex + cubeNumber >= 24)
            {
                return true;
            }

            return false;
        }

        public override bool CanBearOffCheckers(Board gameBoard)
        {
            int NumOfCheckersOutsideHome = gameBoard.GameBar.NumOfRedCheckers;

            for (int i = 0; i <= 17; i++)
            {
                if(gameBoard.Triangles[i].CheckersColor == CheckerColor.Red)
                {
                    NumOfCheckersOutsideHome += gameBoard.Triangles[i].NumOfCheckers;
                }
            }

            if(NumOfCheckersOutsideHome > 0)
            {
                return false;
            }

            return true;
        }

        public override void UpdateCheckersAtHome(Board gameBoard)
        {
            CheckersAtHome = 0;

            for (int i = 18; i <= 23; i++)
            {
                if(gameBoard.Triangles[i].CheckersColor == CheckerColor.Red)
                {
                    CheckersAtHome += gameBoard.Triangles[i].NumOfCheckers;
                }
            }
        }
    }
}
