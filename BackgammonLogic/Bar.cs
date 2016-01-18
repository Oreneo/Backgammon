using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public class Bar
    {
        public int NumOfRedCheckers { get; private set; }

        public int NumOfBlackCheckers { get; private set; }

        public void AddRedCheckerToBar()
        {
            NumOfRedCheckers++;
        }

        public void AddBlackCheckerToBar()
        {
            NumOfBlackCheckers++;
        }

        public void RemoveRedCheckerFromBar()
        {
            NumOfRedCheckers--;
        }

        public void RemoveBlackCheckerFromBar()
        {
            NumOfBlackCheckers--;
        }
    }
}
