/*
 *  MonsterLibrary.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

﻿using System;
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
