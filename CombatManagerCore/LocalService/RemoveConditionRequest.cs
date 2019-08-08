using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    class RemoveConditionRequest : CharacterRequest
    {
        public string Name { get; set; }
    }
}