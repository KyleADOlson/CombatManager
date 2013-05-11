
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
    class CharacterListAdapter : BaseAdapter<Character>
    {
        CombatState _State;
        bool _Monsters;
        List<Character> _Characters;


        public CharacterListAdapter(CombatState state, bool monsters)
        {
            _State = state;
            _Monsters = monsters;
            FilterList();
        }
        public override int Count
        {
            
            get
            {
                return _Characters.Count;
            }
        }
        public override Character this[int position]
        {
            get
            {
                return _Characters[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if (v == null)
            {
                LayoutInflater vi = (LayoutInflater)Application.Context.GetSystemService(Context.LayoutInflaterService);
                v = vi.Inflate(Resource.Layout.CharacterListItem, parent, false);

                v.FindViewById<Button>(Resource.Id.nameEditText).Click += delegate
                {
                    UIUtils.ShowTextDialog("Name", _Characters[(int)v.Tag], parent.Context);
                };

                v.FindViewById<Button>(Resource.Id.hpButton).Click += delegate
                {
                    new NumberDialog("HP", _Characters[(int)v.Tag], parent.Context).Show();
                };
                
                v.FindViewById<Button>(Resource.Id.hpMaxButton).Click += delegate
                {
                    new NumberDialog("MaxHP", _Characters[(int)v.Tag], parent.Context).Show();
                };
                
                v.FindViewById<Button>(Resource.Id.initButton).Click += delegate
                {
                    new NumberDialog("Init", _Characters[(int)v.Tag].Monster, parent.Context).Show();
                };

                v.FindViewById<ImageButton>(Resource.Id.actionButton).Click += delegate
                {
                    Character c = _Characters[(int)v.Tag];
                    ActionDialog d = new ActionDialog(parent.Context, _State);
                    d.Character = c;
                    d.Show();
                };



            }
            v.Tag = position;

            UpdateViewText (v, _Characters[position]);

            return v;
        }

        private void UpdateViewText(View v, Character c)
        {
            v.FindViewById<Button>(Resource.Id.nameEditText).Text = c.Name;
            v.FindViewById<Button>(Resource.Id.hpButton).Text = c.HP.ToString();
            v.FindViewById<Button>(Resource.Id.hpMaxButton).Text = c.MaxHP.ToString();
            v.FindViewById<Button>(Resource.Id.initButton).Text = c.Monster.Init.PlusFormat();

        }

        private void FilterList()
        {
            _Characters = new List<Character>(
                from x in _State.Characters where x.IsMonster == _Monsters select x);

        }

    }
}

