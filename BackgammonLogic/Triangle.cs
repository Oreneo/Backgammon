using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public class Triangle
    {
        public int NumOfCheckers { get; set; }

        public CheckerColor? CheckersColor { get; set; }

        public Triangle()
        {
            
        }

        public Triangle(int numOfCheckers, CheckerColor color)
        {
            NumOfCheckers = numOfCheckers;
            CheckersColor = color;
        }
    }
}
