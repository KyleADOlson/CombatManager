using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace CombatManager
{
    public class BookmarkList
    {
        static BookmarkList list;

        ObservableCollection<Bookmark> bookmarks = new ObservableCollection<Bookmark>();


        public static BookmarkList List
        {
            get
            {
                if (list == null)
                {
                    list = XmlLoader<BookmarkList>.Load("BookmarkList.xml", true);

                }
                if (list == null)
                {
                    list = new BookmarkList();
                }
                return list;
            }
        }

        private static void SaveList()
        {
            XmlLoader<BookmarkList>.Save(list, "BookmarkList.xml", true);
        }

        public bool AddFeat(Feat feat)
        {
            Bookmark b = new Bookmark();
            b.Type = "feat";
            b.Name = feat.Name;
            b.ID = feat.Name;

            return AddBookmark(b);

        }

        public void AddMonster(Monster monster)
        {
            //Bookmark b = new Bo
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

        private bool AddBookmark(Bookmark b)
        {
            if (bookmarks.FirstOrDefault((a) => (a.Name == b.Name && a.Type == b.Type && a.ID == b.ID  && a.Data == b.Data)) == null)
            {
                bookmarks.Add(b);

                SaveList();
                return true;
            }
            else
            {
                return false;
            }
        }

        public ObservableCollection<Bookmark> Bookmarks
        {
            get
            {
                return list.bookmarks;
            }
        }
    }
}
