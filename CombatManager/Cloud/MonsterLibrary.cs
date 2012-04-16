using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager.Cloud
{
    public class MonsterLibrary
    {
        private CloudConnection _Connection;

        public event MonsterLibraryAddedEvent MonsterAdded;
        public event MonsterLibraryRemovedEvent MonsterRemoved;
        public event MonsterQueryItemEvent MonsterQueryItemRecieved;
        public event MonsterQueryCompleteEvent MonsterQueryComplete;

        public delegate void MonsterLibraryAddedEvent(object sender, int queryId, Monster m);
        public delegate void MonsterLibraryRemovedEvent(object sender, int queryId, int id);        
        public delegate void MonsterQueryItemEvent(object sender, int queryId, Monster m);
        public delegate void MonsterQueryCompleteEvent(object sender, int queryId, bool succeeded);

        internal MonsterLibrary(CloudConnection c)
        {
            _Connection = c;
        }

        
        public int Count
        {
            get
            {
                return 0;
            }
        }

        public int QueryMonster(int id)
        {

            if (MonsterQueryItemRecieved != null)
            {
                MonsterQueryItemRecieved(this, id, null);
            }

            if (MonsterQueryComplete != null)
            {
                MonsterQueryComplete(this, id, true);
            }


            return 0;

        }

        public int QueryAllMonsters()
        {
            return 0;
        }
        
        public int AddMonster(Monster m)
        {
            if (MonsterAdded != null)
            {
                MonsterAdded(this, 0, m);
            }



            return 0;
        }

        public int RemoveMonster(int id)
        {
            if (MonsterRemoved != null)
            {
                MonsterRemoved(this, 0, id);
            }

            return 0;
        }

    }
}
