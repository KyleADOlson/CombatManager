using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService.Data
{
    public class RemoteRollResult
    {
        public int Total { get; set; }

        public List<RemoteDieResult> Rolls { get; set; }

        public int Mod { get; set; }

    }
}
