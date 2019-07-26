using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager
{
    public enum RulesSystem
    {
        PF1 = 0,
        PF2 = 1,
        DD5 = 2
    }

    public static class RulesSystemHelper
    {
        public static string SystemName(RulesSystem system)
        {
            String text = "";
            switch (system)
            {
                case RulesSystem.DD5:
                    text = "Dungeons & Dragons 5th";
                    break;
                case RulesSystem.PF2:
                    text = "Pathfinder 2.0";
                    break;
                case RulesSystem.PF1:
                default:
                    text = "Pathfinder 1.0";
                    break;

            }
            return text;
        }
    }
}
