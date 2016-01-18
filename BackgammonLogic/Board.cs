using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public class Board
    {
        public List<Triangle> Triangles { get; private set; }

        public Bar GameBar { get; private set; }

        public Board()
        {
            Triangles = new List<Triangle>(new Triangle[24]);
            GameBar = new Bar();

            InitializeBoard();
        }

        public void AddCheckerToTriangle(int triangleNumber, CheckerColor checkerColor)
        {
            if (checkerColor != Triangles[triangleNumber].CheckersColor)
            {
                Triangles[triangleNumber].NumOfCheckers++;
            }
        }

        public void RemoveCheckerFromTriangle(int triangleNumber, CheckerColor checkerColor)
        {
            if (checkerColor != Triangles[triangleNumber].CheckersColor)
            {
                Triangles[triangleNumber].NumOfCheckers--;
            }
        }

        private void InitializeBoard()  // Player1 = Red. zero-based index.
        {
            for (int i = 0; i < 24; i++)
            {
                switch(i)
                {
                    case 0: Triangles[i] = new Triangle(2, CheckerColor.Red);
                        break;

                    case 5: Triangles[i] = new Triangle(5, CheckerColor.Black);
                        break;

                    case 7: Triangles[i] = new Triangle(3, CheckerColor.Black);
                        break;

                    case 11: Triangles[i] = new Triangle(5, CheckerColor.Red);
                        break;

                    case 12: Triangles[i] = new Triangle(5, CheckerColor.Black);
                        break;

                    case 16: Triangles[i] = new Triangle(3, CheckerColor.Red);
                        break;

                    case 18: Triangles[i] = new Triangle(5, CheckerColor.Red);
                        break;

                    case 23: Triangles[i] = new Triangle(2, CheckerColor.Black);
                        break;

                    default: Triangles[i] = new Triangle();
                        break;
                }
            }
        }
    }
}
