using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgammonLogic
{
    public class Dice
    {
        public int FirstCube { get; private set; }

        public int SecondCube { get; private set; }

        public bool RolledDouble { get; private set; }
        
        private static Random rand = new Random();

        public void RollDice()
        {
            FirstCube = rand.Next(1, 7);
            SecondCube = rand.Next(1, 7);

            if(FirstCube == SecondCube)
            {
                RolledDouble = true;
            }
            else
            {
                RolledDouble = false;
            }
        }

        public void ResetFirstCube()
        {
            FirstCube = 0;
        }

        public void ResetSecondCube()
        {
            SecondCube = 0;
        }
    }
}
