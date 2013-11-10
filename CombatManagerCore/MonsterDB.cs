/*
 *  MonsterDB.cs
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
using ScottsUtils;
using System.IO;

namespace CombatManager
{
    public class MonsterDB : IDisposable
    {
        
        private DBLoader<Monster> _DBLoader;

        private static MonsterDB db;

        private bool _MythicCorrected;


        public MonsterDB()
        {
            InitDB();
        }

        public void InitDB()
        {
            if (_DBLoader == null)
            {
                _DBLoader = new DBLoader<Monster>("bestiary.db");
            }

        }

        public IEnumerable<Monster> Monsters
        {
            get
            {
                var v = _DBLoader.Items;

                if (!_MythicCorrected)
                {
                    foreach (var monster in v)
                    {
                        if (monster.Mythic != "0" || monster.MR != null)
                        {
                            monster.Mythic = "0";
                            monster.MR = null;
                            UpdateMonster(monster);
                        }
                    }
                    _MythicCorrected = true;
                }

                return v;
            }
        }


        public void AddMonster(Monster monster)
        {
            //clear conditions before adding to DB
            List<ActiveCondition> list = new List<ActiveCondition>();
            foreach (ActiveCondition ac in monster.ActiveConditions)
            {
                list.Add(ac);
            }
            foreach (ActiveCondition ac in list)
            {
                monster.RemoveCondition(ac);
            }

            _DBLoader.AddItem(monster);
        }

        public void UpdateMonster(Monster monster)
        {
            _DBLoader.UpdateItem(monster);
        }

        public void DeleteMonster(Monster monster)
        {
            _DBLoader.DeleteItem(monster);
        }

        public void Dispose()
        {
            _DBLoader.Dispose();
            _DBLoader = null;
        }

        public static void ReleaseDB()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
            
        }

        public static MonsterDB DB
        {
            get
            {
                if (db == null)
                {
                    db = new MonsterDB();
                }

                return db;
            }
        }
    }
}
