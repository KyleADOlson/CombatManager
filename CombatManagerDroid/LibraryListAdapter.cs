
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
    class LibraryListAdapter : BaseAdapter
    {
        IList<string> _Strings;
        Context _Context;


        public LibraryListAdapter(Context context, IList<string> strings)
        {
            _Context = context;
            _Strings = strings;
        }
        public override int Count
        {
            get
            {
                return _Strings.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            TextView t = (TextView)convertView;
            if (t == null)
            {
                t = new TextView(_Context);
            }
            t.Text = _Strings[position];
            return t;
        }
    }
}

