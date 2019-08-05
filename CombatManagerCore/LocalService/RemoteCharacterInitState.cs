using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class RemoteCharacterInitState
    {
        public string Name {get; set;}
        public Guid ID {get; set;}
        public RemoteInitiativeCount InitiativeCount {get; set;}
    }
}
