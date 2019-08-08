using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class MonsterAddRequest
    {
        public string Name { get; set; }
        public bool IsMonster { get; set; }
        public MonsterRequest Source { get; set; }

    }
}
