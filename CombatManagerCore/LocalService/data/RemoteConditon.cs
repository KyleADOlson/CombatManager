using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Data
{
    public class RemoteConditon
    {

        public String Name {get; set;}
        public String Text {get; set;}
        public String Image {get; set;}
        public RemoteSpell Spell {get; set;}
        public RemoteAffliction Affliction {get; set;}
        public RemoteBonus Bonus {get; set;}
        public bool Custom {get; set;}
    }
}
