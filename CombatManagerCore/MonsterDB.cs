using System;
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
              
                return _DBLoader.Items;
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
