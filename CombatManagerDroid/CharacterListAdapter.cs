
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
using Android.Graphics.Drawables;

namespace CombatManagerDroid
{
    class CharacterListAdapter : BaseAdapter<Character>
    {
        CombatState _State;
        bool _Monsters;
        List<Character> _Characters;
        List<ConditionChangedHandler> _Handlers = new List<ConditionChangedHandler>();
        List<ConditionButtonChangedHandler> _ButtonHandlers = new List<ConditionButtonChangedHandler>();

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
                    ShowActionDialog(c, parent.Context);
                };


            }
            v.Tag = position;


            Character cp = _Characters[position];

            UpdateViewText(v, cp);
            UpdateConditions(v, cp, parent);
            ConditionChangedHandler cch = new ConditionChangedHandler(v, cp, this, parent);
            _Handlers.Add(cch);

            return v;
        }

        ActionDialog _ActionDialog = null;
        private void ShowActionDialog(Character c, Context context)
        {
            if (_ActionDialog == null)
            {
                _ActionDialog = new ActionDialog(context, _State);
                _ActionDialog.DismissEvent += (object sender, EventArgs e) => 
                {
                    _ActionDialog = null;
                };
                _ActionDialog.Character = c;
                _ActionDialog.Show();
            }
        }

        private class ConditionButtonChangedHandler
        {
            private View _View;
            private Character _Character;
            private ActiveCondition _ActiveCondition;
            View _Button;
            CharacterListAdapter _Adp;
            ViewGroup _Parent;

            public ConditionButtonChangedHandler(View view, View b, Character character, ActiveCondition ac, CharacterListAdapter adp, ViewGroup parent)
            {
                _View = view;
                _Character = character;
                _Button = b;
                _Adp = adp;
                _Parent = parent;
                _ActiveCondition = ac;

                _ActiveCondition.PropertyChanged += HandlePropertyChanged;
                _View.ViewDetachedFromWindow += (object sender, View.ViewDetachedFromWindowEventArgs e) => 
                {
                    _ActiveCondition.PropertyChanged -= HandlePropertyChanged;
                    _Adp._ButtonHandlers.Remove(this);
                };
                b.ViewDetachedFromWindow += (object sender, View.ViewDetachedFromWindowEventArgs e) => 
                {
                    _ActiveCondition.PropertyChanged -= HandlePropertyChanged;
                    _Adp._ButtonHandlers.Remove(this);
                };
            }

            void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Turns")
                {
                    _Adp.UpdateConditions(_View, _Character, _Parent);
                }
            }
        }

        private class ConditionChangedHandler 
        {
            private View _View;
            private Character _Character;
            CharacterListAdapter _Adp;
            ViewGroup _Parent;

            public ConditionChangedHandler(View view, Character character, CharacterListAdapter adp, ViewGroup parent)
            {
                _View = view;
                _Character = character;
                _Adp = adp;
                _Parent = parent;
                
                _Character.Monster.ActiveConditions.CollectionChanged += HandleConditionsChanged;

                _Character.Monster.PropertyChanged += HandlePropertyChanged;
                _Character.PropertyChanged += HandlePropertyChanged;

                _View.ViewDetachedFromWindow += (object sender, View.ViewDetachedFromWindowEventArgs e) => 
                {
                    _Character.Monster.ActiveConditions.CollectionChanged -= HandleConditionsChanged;
                    _Character.Monster.PropertyChanged -= HandlePropertyChanged;
                    _Character.PropertyChanged -= HandlePropertyChanged;
                    _Adp._Handlers.Remove(this);
                };

            }

            void HandlePropertyChanged1 (object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                
            }

            void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                _Adp.UpdateViewText(_View, _Character);
            }
            void HandleConditionsChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {

                _Adp.UpdateConditions(_View, _Character, _Parent);
            }
        }


        private void UpdateViewText(View v, Character c)
        {
            v.FindViewById<Button>(Resource.Id.nameEditText).Text = c.Name;
            v.FindViewById<Button>(Resource.Id.hpButton).Text = c.HP.ToString();
            v.FindViewById<Button>(Resource.Id.hpMaxButton).Text = c.MaxHP.ToString();
            v.FindViewById<Button>(Resource.Id.initButton).Text = c.Monster.Init.PlusFormat();

        }

        private void UpdateConditions(View v, Character c, ViewGroup parent)
        {
            LinearLayout conditionsLayout = v.FindViewById<LinearLayout>(Resource.Id.conditionsLayout);
            conditionsLayout.RemoveAllViews();


            List<string> options = new List<string>()
            {
                "Add 5 Turns",
                "Add Turn",
                "Remove Turn",
                "Remove 5 Turns",
                "Delete",
                "Delete From All Characters"
            };

            foreach (ActiveCondition ac in c.Monster.ActiveConditions)
            {

                int resID = v.Context.Resources.GetIdentifier(ac.Condition.Image.Replace("-", "").ToLower()   + "16", "drawable", v.Context.PackageName);
                Drawable d = v.Context.Resources.GetDrawable(resID);

                View button;
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, 60);
                if (ac.Turns == null)
                {
                
                    ImageButton b = new ImageButton(v.Context);
                    b.SetImageDrawable(d);
                    b.LayoutParameters = lp;
                    conditionsLayout.AddView(b);
                    button = b;
                }
                else
                {
                    Button b = new Button(v.Context);
                    b.Text = ac.Turns.ToString();
                    b.SetCompoundDrawablesWithIntrinsicBounds(d, null, null, null);;
                    b.LayoutParameters = lp;
                    conditionsLayout.AddView(b);
                    button = b;
                }


                ConditionButtonChangedHandler cbch = new ConditionButtonChangedHandler(v, button, c, ac, this, parent);
                _ButtonHandlers.Add(cbch);

                button.Click += (object sender, EventArgs e) => {
                    AlertDialog.Builder builderSingle = new AlertDialog.Builder(parent.Context);

                    builderSingle.SetTitle("Title");
                    ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(parent.Context,
                                                                                 Android.Resource.Layout.SelectDialogItem);
                    arrayAdapter.AddAll(options);


                    builderSingle.SetAdapter (arrayAdapter, (se, ev)=> {
                        switch (ev.Which)
                        {
                        case 0:
                            _State.AddConditionTurns(c, ac, 5);
                            break;
                            case 1:
                            _State.AddConditionTurns(c, ac, 1);
                            break;
                            case 2:
                            _State.RemoveConditionTurns(c, ac, 1);
                            break;
                            case 3:
                            _State.RemoveConditionTurns(c, ac, 5);
                            break;
                        case 4:
                            c.Stats.RemoveCondition(ac);
                            break;
                        case 5:
                            foreach (Character chr in _State.Characters)
                            {
                                c.RemoveConditionByName(ac.Condition.Name);
                            }
                            break;
                        }


                    });


                    builderSingle.Show();
                };

            }

        }

        private void FilterList()
        {
            _Characters = new List<Character>(
                from x in _State.Characters where x.IsMonster == _Monsters select x);

        }

    }
}

