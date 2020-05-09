using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Data
{
    public class RemoteDieRoll
    {
        public int Mod {get; set;}
        public int Fraction {get; set;}

        public List<RemoteDie> Dice {get; set;}
    }
}
