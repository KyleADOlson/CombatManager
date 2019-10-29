using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager
{
    public class CombatStateNotification
    {
        public enum EventType
        {
            NotStabilized,
            Stabilized,
            DyingDied
        }

        public EventType Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public object Data { get; set; }
    }
}
