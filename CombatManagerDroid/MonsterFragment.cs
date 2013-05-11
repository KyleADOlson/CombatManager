
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using CombatManager;

namespace CombatManagerDroid
{
    class MonsterFragment : LookupFragment<Monster>
    {
        protected override List<Monster> GetItems ()
        {
            return new List<Monster>(Monster.Monsters);
        }

        protected override string ItemHtml (Monster item)
        {
            return MonsterHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Monster item)
        {
            return item.Name;
        }
    }
}

