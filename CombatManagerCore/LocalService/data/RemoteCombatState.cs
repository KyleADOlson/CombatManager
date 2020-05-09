using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Data
{
    public class RemoteCombatState
    {
        public int? Round { get; set; }
        public String CR { get; set; }
        public long? XP { get; set; }
        public int RulesSystem { get; set; }

        public List<RemoteCharacterInitState> CombatList { get; set; }

        public Guid CurrentCharacterID { get; set; }
        public RemoteInitiativeCount CurrentInitiativeCount { get; set; } 

    }
}
