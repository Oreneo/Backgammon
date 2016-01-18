using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public class BackgammonController
    {
        public RedPlayer _RedPlayer { get; private set; }

        public BlackPlayer _BlackPlayer { get; private set; }

        public Board GameBoard { get; private set; }

        public Dice GameDice { get; set; }

        public bool RolledDice { get; set; }

        public int MovesLeft { get; private set; }

        public int? PlayerInitialTriangleChoice { get; private set; }

        public BackgammonController()
        {
            _RedPlayer = new RedPlayer("Red Player", CheckerColor.Red);
            _BlackPlayer = new BlackPlayer("Black Player", CheckerColor.Black);
            
            GameBoard = new Board();
            GameDice = new Dice();

            _RedPlayer.IsMyTurn = true;
        }

        public bool PlayerHasAvailableMoves()
        {
            if(_RedPlayer.IsMyTurn == true)
            {
                return _RedPlayer.HasAvailableMoves(GameBoard, GameDice);
            }
            else
            {
                return _BlackPlayer.HasAvailableMoves(GameBoard, GameDice);
            }
        }

        public bool PlayerHasAvailableBearOffMoves()
        {
            if (_RedPlayer.IsMyTurn == true)
            {
                return _RedPlayer.HasAvailableBearOffMoves(GameBoard, GameDice);
            }
            else
            {
                return _BlackPlayer.HasAvailableBearOffMoves(GameBoard, GameDice);
            }
        }

        public bool IsLegalBearOffMove(out int cubeUsed)
        {
            cubeUsed = GameDice.FirstCube < GameDice.SecondCube ? GameDice.FirstCube : GameDice.SecondCube;

            if (_RedPlayer.IsMyTurn == true)
            {
                if (GameDice.RolledDouble == true)
                {
                    return _RedPlayer.IsLegalPlayerBearOffMove(PlayerInitialTriangleChoice.Value, GameDice.FirstCube);
                }
                else
                {
                    bool firstCubeLegalMove = _RedPlayer.IsLegalPlayerBearOffMove(PlayerInitialTriangleChoice.Value, GameDice.FirstCube);
                    bool secondCubeLegalMove = _RedPlayer.IsLegalPlayerBearOffMove(PlayerInitialTriangleChoice.Value, GameDice.SecondCube);
                    
                    if(firstCubeLegalMove == true)
                    {
                        cubeUsed = GameDice.FirstCube;
                    }
                    if(secondCubeLegalMove == true)
                    {
                        cubeUsed = GameDice.SecondCube;
                    }
                    if(firstCubeLegalMove == true && secondCubeLegalMove)
                    {
                        cubeUsed = GameDice.FirstCube < GameDice.SecondCube ? GameDice.FirstCube : GameDice.SecondCube;
                    }

                    return firstCubeLegalMove || secondCubeLegalMove;
                }
            }
            else // black player's turn
            {
                if (GameDice.RolledDouble == true)
                {
                    return _BlackPlayer.IsLegalPlayerBearOffMove(PlayerInitialTriangleChoice.Value, GameDice.FirstCube);
                }
                else
                {
                    bool firstCubeLegalMove = _BlackPlayer.IsLegalPlayerBearOffMove(PlayerInitialTriangleChoice.Value, GameDice.FirstCube);
                    bool secondCubeLegalMove = _BlackPlayer.IsLegalPlayerBearOffMove(PlayerInitialTriangleChoice.Value, GameDice.SecondCube);

                    if (firstCubeLegalMove == true)
                    {
                        cubeUsed = GameDice.FirstCube;
                    }
                    if (secondCubeLegalMove == true)
                    {
                        cubeUsed = GameDice.SecondCube;
                    }
                    if (firstCubeLegalMove == true && secondCubeLegalMove)
                    {
                        cubeUsed = GameDice.FirstCube < GameDice.SecondCube ? GameDice.FirstCube : GameDice.SecondCube;
                    }

                    return firstCubeLegalMove || secondCubeLegalMove;
                }
            }
        }

        public bool IsLegalFinalMoveEat(int toIndex)
        {
            if (_RedPlayer.IsMyTurn == true)
            {
                if (GameDice.RolledDouble == true)
                {
                    return _RedPlayer.IsLegalPlayerFinalMoveEat(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube);
                }
                else
                {
                    return _RedPlayer.IsLegalPlayerFinalMoveEat(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube) ||
                           _RedPlayer.IsLegalPlayerFinalMoveEat(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.SecondCube);
                }
            }
            else // black player's turn
            {
                if (GameDice.RolledDouble == true)
                {
                    return _BlackPlayer.IsLegalPlayerFinalMoveEat(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube);
                }
                else
                {
                    return _BlackPlayer.IsLegalPlayerFinalMoveEat(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube) ||
                           _BlackPlayer.IsLegalPlayerFinalMoveEat(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.SecondCube);
                }
            }
        }

        // Dude wants to move 'to'
        public bool IsLegalFinalMove(int toIndex)
        {
            if (_RedPlayer.IsMyTurn == true)
            {
                if (GameDice.RolledDouble == true)
                {
                    return _RedPlayer.IsLegalPlayerFinalMove(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube);
                }
                else
                {
                    return _RedPlayer.IsLegalPlayerFinalMove(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube)  ||
                           _RedPlayer.IsLegalPlayerFinalMove(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.SecondCube);
                }
            }
            else // black player's turn
            {
                if (GameDice.RolledDouble == true)
                {
                    return _BlackPlayer.IsLegalPlayerFinalMove(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube);
                }
                else
                {
                    return _BlackPlayer.IsLegalPlayerFinalMove(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.FirstCube) ||
                           _BlackPlayer.IsLegalPlayerFinalMove(GameBoard, PlayerInitialTriangleChoice.Value, toIndex, GameDice.SecondCube);
                }
            }
        }

        public bool IsLegalInitialMove(int index)
        {
            if (_RedPlayer.IsMyTurn == true)
            {
                return _RedPlayer.IsLegalPlayerInitialMove(GameBoard, index)/* && GameBoard.GameBar.NumOfRedCheckers == 0*/;   // dont let him move if he has checkers on the bar.
            }
            else // black player's turn
            {
                return _BlackPlayer.IsLegalPlayerInitialMove(GameBoard, index);
            }
        }

        public void SetPlayerBearOffMove(int cube)
        {
            ResetAppropriateCubeByNumber(cube);

            GameBoard.Triangles[PlayerInitialTriangleChoice.Value].NumOfCheckers--;

            // update triangle.
            if (GameBoard.Triangles[PlayerInitialTriangleChoice.Value].NumOfCheckers == 0)
            {
                GameBoard.Triangles[PlayerInitialTriangleChoice.Value].CheckersColor = null;
            }

            PlayerInitialTriangleChoice = null;

            MovesLeft--;
        }

        private void ResetAppropriateCubeByNumber(int cube)
        {
            if (GameDice.RolledDouble == false)
            {
                if (cube == GameDice.FirstCube)
                {
                    GameDice.ResetFirstCube();
                }
                if (cube == GameDice.SecondCube)  // don't enter both if clauses if we got double
                {
                    GameDice.ResetSecondCube();
                }
            }
        }

        public void SetPlayerInitialMove(int? index)
        {
            PlayerInitialTriangleChoice = index;
        }

        public void SetPlayerFinalMoveEat(int index)
        {
            if (_RedPlayer.IsMyTurn)  // black was eaten.
            {
                GameBoard.GameBar.AddBlackCheckerToBar();
            }
            else
            {
                GameBoard.GameBar.AddRedCheckerToBar();
            }
            
            SetPlayerFinalMove(index, true);
        }

        public void SetPlayerFinalMove(int index, bool eaten)
        {
            ResetAppropriateCube(index);

            if (eaten == false)   // if one checker eats another, no need to 'add' another checker to the triangle
            {
                GameBoard.Triangles[index].NumOfCheckers++;
            }

            if (PlayerInitialTriangleChoice >= 0 && PlayerInitialTriangleChoice <= 23)
            {
                GameBoard.Triangles[PlayerInitialTriangleChoice.Value].NumOfCheckers--;
            }
            else // Initial move was from the Bar.
            {
                // remove checker from bar
                if(_RedPlayer.IsMyTurn == true)
                {
                    GameBoard.GameBar.RemoveRedCheckerFromBar();
                }
                else
                {
                    GameBoard.GameBar.RemoveBlackCheckerFromBar();
                }
            }

            // update colors for triangles.
            if (PlayerInitialTriangleChoice >= 0 && PlayerInitialTriangleChoice <= 23)
            {
                if (GameBoard.Triangles[PlayerInitialTriangleChoice.Value].NumOfCheckers == 0)
                {
                    GameBoard.Triangles[PlayerInitialTriangleChoice.Value].CheckersColor = null;
                }
            }

            if (GameBoard.Triangles[index].NumOfCheckers == 1) // eaten or
            {
                GameBoard.Triangles[index].CheckersColor = _RedPlayer.IsMyTurn ? _RedPlayer.PlayerColor : _BlackPlayer.PlayerColor;
            }

            PlayerInitialTriangleChoice = null;

            MovesLeft--;
        }

        private void ResetAppropriateCube(int toIndex)
        {
            if (GameDice.RolledDouble == false)
            {
                if (_BlackPlayer.IsMyTurn)
                {
                    if (PlayerInitialTriangleChoice - toIndex == GameDice.FirstCube)
                    {
                        GameDice.ResetFirstCube();
                    }
                    else
                    {
                        GameDice.ResetSecondCube();
                    }
                }
                else
                {
                    if (toIndex - PlayerInitialTriangleChoice == GameDice.FirstCube)
                    {
                        GameDice.ResetFirstCube();
                    }
                    else
                    {
                        GameDice.ResetSecondCube();
                    }
                }
            }
        }

        public void SwapTurns()
        {
            _RedPlayer.IsMyTurn = !_RedPlayer.IsMyTurn;
            _BlackPlayer.IsMyTurn = !_BlackPlayer.IsMyTurn;
        }

        public void GetDiceRolls()
        {
            GameDice.RollDice();

            RolledDice = true;
        }

        public void UpdateMovesLeft()
        {
            if (GameDice.RolledDouble == true)
            {
                MovesLeft = 4;
            }
            else
            {
                MovesLeft = 2;
            }
        }

        public void ResetMovesLeft()
        {
            MovesLeft = 0;
        }
    }
}
