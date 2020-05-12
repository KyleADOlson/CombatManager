using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Request
{
    public class FeatListRequest
    {
        public string Name { get; set; }
        public bool? IsCustom { get; set; }
        public string Type { get; set; }

    }
}
