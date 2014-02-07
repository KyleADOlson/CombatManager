
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
using Android.Graphics;

namespace CombatManagerDroid
{

    class CharacterActionsAdapter : BaseAdapter
    {

        Context _Context;
        Character _Character;
        List<CharacterActionItem> _ActionItems;
        Stack<List<CharacterActionItem>> _ParentActionItems = new Stack<List<CharacterActionItem>>();

        
        public CharacterActionsAdapter(Context context, Character character, CombatState state)
        {
            _Context = context;
            _Character = character;
            _ActionItems = CharacterActions.GetActions(_Character, _Character, new List<Character>(
                from x in state.Characters where x.IsMonster == _Character.IsMonster select x));
        }

        public override int Count
        {
            get
            {
                return _ActionItems.Count;
            }
        }

        public List<CharacterActionItem> ActionItems
        {
            get
            {
                return _ActionItems;
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
            CharacterActionItem ai = _ActionItems[position];
            TextView t = (TextView)convertView;
            if (t == null)
            {
                t = new TextView(_Context);
            }
            t.Text = ai.Name;
            t.SetTextColor(new Color(0xee, 0xee, 0xee));
            t.SetTextSize(Android.Util.ComplexUnitType.Dip, 20f);
            if (ai.Name != null && ai.Name.Length > 0)
            {
                t.SetPadding(8, 10, 5, 10);
            }
            else
            {
                t.SetPadding(0, -2, 0, -2);
                t.Enabled = false;
            }
            t.SetCompoundDrawablesWithIntrinsicBounds(PopupUtils.NamedImage(_Context, ai.Icon), null, null, null);
            return t;
        }

        public void MoveToSubItems(CharacterActionItem ai)
        {
            if (_ActionItems != null)
            {
                _ParentActionItems.Push(_ActionItems);
            }
            _ActionItems = ai.SubItems;
        }

        public void MoveBack()
        {
            if (_ParentActionItems.Count > 0)
            {
                _ActionItems = _ParentActionItems.Pop();
            }
        }
    }
}

