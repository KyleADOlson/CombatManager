using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Request
{
    public class AddConditionRequest : CharacterRequest
    {
        public string Name { get; set; }
        public int? Turns { get; set; }
    }
}
