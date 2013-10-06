
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
        View _View;
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

        public class CharacterEventArgs
        {
            public Character Character{get; set;}
        }

        public EventHandler<CharacterEventArgs> CharacterClicked;

        public InitiativeListAdapter(CombatState state, View view)
        {
            _State = state;
            _View = view;
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

        private class ViewUpdateListener
        {
            Character _Char;
            ImageView _Ready;
            ImageView _DelayView;
            Button _Init;

            public ViewUpdateListener(Character c, LinearLayout layout, ImageView rv, ImageView dv, Button ib)
            {
                _Char = c;
                _Ready = rv;
                _DelayView = dv;
                _Init = ib;

                layout.ViewDetachedFromWindow += (object sender, View.ViewDetachedFromWindowEventArgs e) => 
                {
                    _Char.PropertyChanged -= HandlePropertyChanged;
                };

                _Char.PropertyChanged += HandlePropertyChanged;

                UpdateItems();
            }

            void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                UpdateItems();
            }

            private void UpdateItems()
            {
                
                _Ready.Visibility = _Char.IsReadying?ViewStates.Visible:ViewStates.Gone;
                _DelayView.Visibility = _Char.IsDelaying?ViewStates.Visible:ViewStates.Gone;
                _Init.Text = _Char.CurrentInitiative.ToString();
            }
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
            
            Character c = _State.CombatList[position];

            ImageView rv = new ImageView(_View.Context);
            rv.SetImageDrawable(_View.Context.Resources.GetDrawable(
            Resource.Drawable.target16));
            layout.AddView(rv);
            rv.Visibility = c.IsReadying?ViewStates.Visible:ViewStates.Gone;
        

        
            ImageView dv = new ImageView(_View.Context);
            dv.SetImageDrawable(_View.Context.Resources.GetDrawable(
                Resource.Drawable.hourglass16));
            layout.AddView(dv);
            dv.Visibility =  c.IsDelaying?ViewStates.Visible:ViewStates.Gone;



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

            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            lp.Weight = 1.0f;
            t.LayoutParameters = lp;

            t.Click += (object sender, EventArgs e) => 
            {
                if (CharacterClicked != null)
                {
                    CharacterClicked(this, new CharacterEventArgs() {Character = c});
                }

            };
          

            layout.AddView(t);



            //init button
            Button bu = new Button(_View.Context);

            bu.SetHeight(45);
            bu.SetWidth(45);
            layout.AddView(bu);
            bu.SetBackgroundDrawable(_View.Context.Resources.GetDrawable(Resource.Drawable.blue_button));
            bu.Text = c.CurrentInitiative.ToString();
            bu.SetTextColor(new Android.Graphics.Color(0xff, 0xff, 0xff));
            bu.Click += (object sender, EventArgs e) => {
                
                new NumberDialog("CurrentInitiative", "Initiative", c, _View.Context).Show();
            };

            //action button
            ImageButton b = new ImageButton(_View.Context);
            b.SetImageDrawable(_View.Context.Resources.GetDrawable(Resource.Drawable.lightning16));
            layout.AddView(b);
            b.SetMaxHeight(45);
            b.SetMinimumHeight(45);
            b.SetMaxWidth(45);
            b.SetMinimumWidth(45);
            b.SetBackgroundDrawable(_View.Context.Resources.GetDrawable(Resource.Drawable.blue_button));

           
            var options = new List<string> (){"Move Down", "Move Up", "Ready", "Delay"};
            PopupUtils.AttachSimpleStringPopover(c.Name,
                                                 b,
                                                 options,
                                                 (v, index, val) => 
                                                 {
                switch (index)
                {
                case 0:
                    _State.MoveDownCharacter(c);
                    break;
                case 1:
                    _State.MoveUpCharacter(c);
                    break;
                case 2:
                    
                    c.IsReadying = !c.IsReadying;
                    break;
                case 3:
                    c.IsDelaying = !c.IsDelaying;
                    break;
                }

            });

            new ViewUpdateListener(c, layout, rv, dv, bu);



            return layout;
        }


    }
}

