using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Service
{
    public class MonsterListResponse : ServiceResponse
    {
        public MonsterDetail [] monsters;
    }

    public class MonsterDetail
    {
        public string monsterid;
        public string name;
    }
}
