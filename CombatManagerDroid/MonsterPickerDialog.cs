
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
    class MonsterPickerDialog : Dialog
    {
        List<Monster> _Monsters = null;
        List<Monster> _VisibleMonsters;

        Monster _SelectedMonster = null;

        MonsterPickerAdapter _Adapter;
        CombatState _State;
        bool _IsMonsters;

        public MonsterPickerDialog(Context context, bool isMonsters, CombatState state) : base (context)
        {
            _State = state;
            _IsMonsters = isMonsters;

            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetCanceledOnTouchOutside(true);

            SetContentView(Resource.Layout.MonsterPickerDialog);
            Window.SetSoftInputMode(SoftInput.AdjustResize);

            ((Button)FindViewById(Resource.Id.closeButton)).Click += 
            (object sender, EventArgs e) => {Dismiss();};
            
            ((Button)FindViewById(Resource.Id.addButton)).Click += 
            (object sender, EventArgs e) => {AddSelectedMonster();};



            _Monsters = new List<Monster>(from x in Monster.Monsters orderby x.Name select x);

            _VisibleMonsters = _Monsters;
            _SelectedMonster = _Monsters[0];

            _Adapter = new MonsterPickerAdapter(_Monsters);

            _Adapter.SelectedMonster = _SelectedMonster;

            ListView lv = ((ListView)FindViewById(Resource.Id.monsterListView)); 

            lv.SetAdapter(
               _Adapter);
            lv.ItemClick += 
            (object sender, AdapterView.ItemClickEventArgs e) => { 
                if (_SelectedMonster != _VisibleMonsters[e.Position])
                {
                    Monster old = _SelectedMonster;
                    _SelectedMonster = _VisibleMonsters[e.Position];
                    _Adapter.SelectedMonster = _SelectedMonster;
                    UpdateMonster(old);
                    UpdateMonster(_SelectedMonster);
                }

            };

            ((EditText)FindViewById(Resource.Id.monsterEditText)).TextChanged += 
            (object sender, Android.Text.TextChangedEventArgs e) => {FilterMonsters();};

        
        }

        private void UpdateMonster(Monster m)
        {
            ListView lv = ((ListView)FindViewById(Resource.Id.monsterListView)); 
            int index = _VisibleMonsters.IndexOf(m);
            if (index != null)
            {
                TextView tv = (TextView)lv.GetItemViewAt(index);
                if (tv != null)
                {
                    _Adapter.UpdateMonsterView(_VisibleMonsters[index], tv);
                }
            }
        }


        private void FilterMonsters()
        {
            ListView lv = (ListView)FindViewById(Resource.Id.monsterListView);

            MonsterPickerAdapter ad = (MonsterPickerAdapter) 
                lv.Adapter;

            EditText monsterEditText = (EditText)FindViewById(Resource.Id.monsterEditText);

            List<Monster> newMonsters = null;

            string text = monsterEditText.Text.Trim();

            if (text.Length == 0)
            {
                newMonsters = new List<Monster>(_Monsters);
            }
            else
            {

                newMonsters = new List<Monster>(from x in _Monsters where
                                              x.Name.ToUpper().Contains(text.ToUpper()) orderby x.Name select x);
            }
            _VisibleMonsters = newMonsters;

            if (newMonsters.Count == 0)
            {
                _SelectedMonster = null;
            }
            else if (!newMonsters.Contains(_SelectedMonster))
            {
                _SelectedMonster = newMonsters[0];
            }

            ad.SelectedMonster = _SelectedMonster;
            ad.Monsters = newMonsters;


        }

        public void AddSelectedMonster()
        {
            if (_SelectedMonster != null)
            {
                _State.AddMonster(_SelectedMonster, Context.GetCMPrefs().GetRollHP(), _IsMonsters); 
            }
        }


        class MonsterPickerAdapter : BaseAdapter<Monster>
        {


            List<Monster> _Monsters;
            
            public MonsterPickerAdapter(List<Monster> monsters)
            {
                _Monsters = monsters;
            }
            public override int Count
            {
                
                get
                {
                    return _Monsters.Count;
                }
            }
            public override Monster this[int position]
            {
                get
                {
                    return _Monsters[position];
                }
            }
            
            public override Java.Lang.Object GetItem(int position)
            {
                return _Monsters[position].Name;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            
            public override View GetView(int position, View convertView, ViewGroup parent)
            {

                
                TextView t = new TextView(Application.Context);
                if (t == null)
                {
                    t = new TextView(Application.Context);
                }
                UpdateMonsterView(_Monsters[position], t);

                return t;
            }

            public List<Monster> Monsters
            {
                get
                {
                    return _Monsters;
                }
                set
                {
                    _Monsters = value;
                    NotifyDataSetChanged();
                }
            }

            public Monster SelectedMonster
            {
                get;
                set;
            }

            public void UpdateMonsterView(Monster monster, TextView t)
            {
                t.Text = monster.Name;
                t.SetTextSize(Android.Util.ComplexUnitType.Dip, 18);
                if (monster == SelectedMonster)
                {
                    t.SetTextColor (new Android.Graphics.Color(0,0,0));
                    t.SetBackgroundColor(new Android.Graphics.Color(0xff, 0xff, 0xff));
                }
                else
                {
                    t.SetTextColor(t.Context.Resources.GetColor(Android.Resource.Color.PrimaryTextDark));
                    t.SetBackgroundColor(new Android.Graphics.Color(0, 0, 0));
                }
            }

            
        }


        

    }
}

