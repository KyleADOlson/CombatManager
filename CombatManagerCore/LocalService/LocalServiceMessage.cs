using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    public class LocalServiceMessage
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public object Data { get; set; }

        private static int nextID = 1;

        public static LocalServiceMessage Create(String name, object data)
        {
            return new LocalServiceMessage() { Name = name, Data = data, ID = nextID++};

        }
    }


}
