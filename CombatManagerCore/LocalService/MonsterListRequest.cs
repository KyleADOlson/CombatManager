using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class MonsterListRequest
    {
        public String Name { get; set; }
        public bool? IsCustom { get; set; }
        public bool? IsNPC { get; set; }
        public String MinCR { get; set; }
        public String MaxCR { get; set; }

    }
}
