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
using Android.Webkit;

namespace CombatManagerDroid
{
    class ConditionDialog : Dialog
    {
        Character _Character;
        CombatState _State;

        ConditionsAdapter _Adapter;

        int _SelectedTab = 0;

        int _SelectedIndex = 0;

        List<Condition> _FilteredList;

        public ConditionDialog(Context context, CombatState state, Character character) : base (context)
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.ConditionDialog);
            SetCanceledOnTouchOutside(true);

            _State = state;
            _Character = character;

            Button close = (Button)FindViewById(Resource.Id.cancelButton);
            close.Click += delegate
            {
                Dismiss ();
            };

            Button okButton = (Button)FindViewById(Resource.Id.okButton);
            okButton.Click += delegate
            {
                if (SelectedCondition != null)
                {
                    ActiveCondition ac = new ActiveCondition();
                    ac.Condition = SelectedCondition;
                    ac.InitiativeCount = _State.CurrentInitiativeCount;
                    _Character.Monster.ActiveConditions.Add(ac);
                    Condition.PushRecentCondition(ac.Condition);
                    Dismiss();
                }
            };

            Button b;
            b = FindViewById<Button>(Resource.Id.conditionsButton);
            b.Click += delegate 
            {
                SetSelectedTab(0);
            };
            b = FindViewById<Button>(Resource.Id.spellsButton);
            b.Click += delegate 
            {
                SetSelectedTab(1);
            };
            b = FindViewById<Button>(Resource.Id.afflictionsButton);
            b.Click += delegate 
            {
                if (!Condition.MonsterConditionsLoaded)
                {
                    Condition.LoadMonsterConditions();
                }
                SetSelectedTab(2);
            };

            EditText et = FindViewById<EditText>(Resource.Id.filterText);
            et.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => 
            {
                RefreshList();
            };

            PrepareList();
        }

        void SetSelectedTab(int val)
        {
            if (_SelectedTab != val)
            {
                _SelectedTab = val;
                RefreshList();
            }
        }

        void RefreshList()
        {
            FilterList();
            _Adapter.NotifyDataSetChanged();
            ShowSelection();
        }

        void PrepareList()
        {
            FilterList();
            _Adapter = new ConditionsAdapter(this);
            ListView lv = FindViewById<ListView>(Resource.Id.itemList);
            lv.SetAdapter(_Adapter);
            lv.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
            {
                e.View.Selected = true;
                _SelectedIndex = e.Position;
                _Adapter.NotifyDataSetChanged();
                ShowSelection();
            };
            ShowSelection();


        }


        void FilterList()
        {
            ConditionType t = ConditionType.Condition;
            switch (_SelectedTab)
            {
            case 0:
                t = ConditionType.Condition;
                break;
            case 1:
                t = ConditionType.Spell;
                break;
            case 2:
                t = ConditionType.Afflicition;
                break;
            }

            string filterText = FindViewById<EditText>(Resource.Id.filterText).Text;


            _FilteredList = new List<Condition>(from x in Condition.Conditions where x.Type == t && FilterText(filterText, x) select x);

            if (_SelectedIndex > _FilteredList.Count)
            {
                _SelectedIndex = 0;
            }
        }

        bool FilterText(string filterText, Condition c)
        {
            return filterText == null || c.Name.ToUpper().Contains(filterText.ToUpper());
        }




        Drawable ItemImage(object item)
        {
            if (item is Condition)
            {
                return PopupUtils.NamedImage(Context, ((Condition)item).Image);
            }
            return null;
        }

        string ItemText(object item)
        {
            if (item is Condition)
            {
                return ((Condition)item).Name;
            }
            return null;
        }

        Condition SelectedCondition
        {
            get
            {
                if (_SelectedIndex < _FilteredList.Count)
                {
                    return _FilteredList[_SelectedIndex];
                }
                return null;
            }
        }

        void ShowSelection()
        {
            WebView wv = FindViewById<WebView>(Resource.Id.conditionView);
            if (SelectedCondition == null)
            {
                wv.LoadUrl("about:blank");
            }
            else
            {
  
                wv.LoadDataWithBaseURL(null, ConditionHtmlCreator.CreateHtml(SelectedCondition), "text/html", "utf-8", null);
 
            }
        }

        class ConditionsAdapter : BaseAdapter
        {
            ConditionDialog _Dialog;
            Context _Context;


            public ConditionsAdapter(ConditionDialog dialog)
            {
                _Dialog = dialog;
                _Context = dialog.Context;
            }

            public override int Count
            {
                get
                {
                    return _Dialog._FilteredList.Count;
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
                object item = _Dialog._FilteredList[position];
                t.Text = _Dialog.ItemText(item);
                t.SetCompoundDrawablesWithIntrinsicBounds(_Dialog.ItemImage(item), null, null, null);

                if (position == _Dialog._SelectedIndex)
                {
                    t.SetTextColor(new Android.Graphics.Color(0, 0, 0));
                    t.SetBackgroundColor(new Android.Graphics.Color(0xff, 0xff, 0xff));

                }
                else
                {
                    t.SetBackgroundColor(new Android.Graphics.Color(0, 0, 0));
                    t.SetTextColor(new Android.Graphics.Color(0xff, 0xff, 0xff));
                }

                return t;

            }
        }
    }
}

