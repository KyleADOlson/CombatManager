using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class RemoteActiveCondition
    {
        public RemoteConditon Conditon { get; set; }
        public int? Turns { get; set; }
        public RemoteInitiativeCount InitiativeCount { get; set; }
        public string Details { get; set; }
    }
}
