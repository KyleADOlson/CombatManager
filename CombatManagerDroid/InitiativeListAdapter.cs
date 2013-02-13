
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
    class InitiativeListAdapter : BaseAdapter<Character>
    {  
        CombatState _State;
        public InitiativeListAdapter(CombatState state)
        {
            _State = state;
        }
        public override int Count
        {

            get
            {
                return _State.CombatList.Count;
            }
        }
        public override Character this[int position]
        {
            get
            {
                return _State.CombatList[position];
            }
        }
        
        public override Java.Lang.Object GetItem(int position)
        {
            return _State.CombatList[position].Name;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LinearLayout layout = (LinearLayout)convertView;
            if (layout == null)
            {
                layout = new LinearLayout(Application.Context);
            }
            layout.RemoveAllViews();

            TextView t = new TextView(Application.Context);
            if (t == null)
            {
                t = new TextView(Application.Context);
            }
            t.Text = _State.CombatList[position].Name;
            t.SetTextColor (Android.Graphics.Color.Black);

            if (_State.CurrentCharacter == _State.CombatList[position])
            {
                TextView tv = new TextView(Application.Context);
                tv.Text = ">> ";
                layout.AddView(tv);
            }

          

            layout.AddView(t);


            return layout;
        }


    }
}

