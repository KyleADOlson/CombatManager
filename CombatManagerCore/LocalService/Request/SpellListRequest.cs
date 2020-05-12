using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Request
{
    public class SpellListRequest
    {
        public string Name { get; set; }
        public bool? IsCustom { get; set; }
        public string School { get; set; }
        public string Subschool { get; set; }

    }
}
