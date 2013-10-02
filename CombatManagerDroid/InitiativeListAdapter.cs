
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
        Character _Character;
        public Character Character
        {
            get
            {
                return _Character;
            }
            set
            {
                _Character = value;
            }
        }

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
            layout.LayoutParameters = new AbsListView.LayoutParams(
                new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent));


            TextView t = new TextView(Application.Context);
            if (t == null)
            {
                t = new TextView(Application.Context);
            }
            t.Text = _State.CombatList[position].Name;
            t.SetTextColor (Android.Graphics.Color.Black);
            
            layout.SetBackgroundColor(new Android.Graphics.Color(0xee, 0xee, 0xee));
            if (_State.CurrentCharacter == _State.CombatList[position])
            {
                ImageView iv = new ImageView(Application.Context);
                iv.SetImageDrawable(Application.Context.Resources.GetDrawable(Resource.Drawable.next16));

                layout.AddView(iv);
            }

            if (_State.CombatList[position] == _Character)
            {
                layout.SetBackgroundColor(new Android.Graphics.Color(0, 0, 0));
                
                t.SetTextColor (Android.Graphics.Color.White);
            }
          

            layout.AddView(t);


            return layout;
        }


    }
}

