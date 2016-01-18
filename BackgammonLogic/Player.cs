using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public abstract class Player
    {
        public bool IsMyTurn { get; set; }

        public int CheckersAtHome { get; set; }

        public CheckerColor PlayerColor { get; private set; }

        public string Name { get; private set; }

        public Player(string name, CheckerColor color)
        {
            Name = name;
            PlayerColor = color;
        }

        public bool HasAvailableMoves(Board gameBoard, Dice gameDice)
        {
            return GetAvailableMoves(gameBoard, gameDice).ToList().Count + GetAvailableMovesEat(gameBoard, gameDice).ToList().Count > 0 ? true : false;
        }

        public bool HasAvailableBearOffMoves(Board gameBoard, Dice gameDice)
        {
            return GetAvailableBearOffMoves(gameBoard, gameDice).ToList().Count > 0 ? true : false;
        }

        public abstract IEnumerable<KeyValuePair<int, int>> GetAvailableMoves(Board gameBoard, Dice gameDice);

        public abstract IEnumerable<KeyValuePair<int, int>> GetAvailableMovesFromBar(Board gameBoard, Dice gameDice);

        public abstract IEnumerable<KeyValuePair<int, int>> GetAvailableMovesEat(Board gameBoard, Dice gameDice);

        public abstract IEnumerable<KeyValuePair<int, int>> GetAvailableMovesEatFromBar(Board gameBoard, Dice gameDice);

        public abstract IEnumerable<KeyValuePair<int, int>> GetAvailableBearOffMoves(Board gameBoard, Dice gameDice);
        
        public abstract bool IsLegalPlayerInitialMove(Board gameBoard, int index);

        public abstract bool IsLegalPlayerFinalMove(Board gameBoard, int fromIndex, int toIndex, int cubeNumber);

        public abstract bool IsLegalPlayerFinalMoveEat(Board gameBoard, int fromIndex, int toIndex, int cubeNumber);

        public abstract bool IsLegalPlayerBearOffMove(int fromIndex, int cubeNumber);

        public abstract bool CanBearOffCheckers(Board gameBoard);

        public abstract void UpdateCheckersAtHome(Board gameBoard);
    }
}
