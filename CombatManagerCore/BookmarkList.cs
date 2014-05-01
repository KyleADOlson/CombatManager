using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public class BookmarkList
    {
        static BookmarkList list;

        List<Bookmark> bookmarks = new List<Bookmark>();


        public static BookmarkList List
        {
            get
            {
                if (list == null)
                {
                    //load bookmark list
                }
                return list;
            }
        }

        private void SaveList()
        {

        }

        public void AddFeat(Feat feat)
        {

        }

        public void AddMonster(Monster monster)
        {

        }

        public void AddSpell(Spell spell)
        {

        }

        public void AddRule(Rule rule)
        {

        }

        public void AddTreasure(Treasure treasure)
        {

        }
    }
}
