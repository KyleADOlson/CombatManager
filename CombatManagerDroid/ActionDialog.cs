
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
using Android.Webkit;

using CombatManager;

namespace CombatManagerDroid
{
    class ActionDialog : Dialog
    {
        Character _Character;
        CombatState _State;

        public ActionDialog(Context context, CombatState state) : base (context)
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.ActionDialog);

            _State = state;

            Button close = (Button)FindViewById(Resource.Id.closeButton);
            close.Click += delegate
                {
                    Dismiss ();
                };


        }

        public Character Character
        {
            get
            {
                return _Character;
            }
            set
            {
                _Character = value;
                SetTitle(_Character.Name);

                WebView wv = (WebView)this.FindViewById(Resource.Id.webView);
                
                wv.LoadUrl("about:blank");
                wv.LoadData(MonsterHtmlCreator.CreateHtml(_Character.Monster), "text/html", null);

                ListView lv = (ListView)FindViewById(Resource.Id.actionListView);
                lv.ItemClick += ListViewItemClick;

                var ca = new CharacterActionsAdapter(Context, _Character);
                lv.SetAdapter(ca);
            }
        }

        void ListViewItemClick (object sender, AdapterView.ItemClickEventArgs e)
        {
            
            ListView lv = (ListView)FindViewById(Resource.Id.actionListView);
            CharacterActionsAdapter ca = (CharacterActionsAdapter)lv.Adapter;
            CharacterActionItem ai = ca.ActionItems[e.Position];

            if (ai.SubItems != null)
            {
                ca.MoveToSubItems(ai);
                ca.NotifyDataSetChanged();
            }
            else if (ai.Name != null && ai.Name.Length > 0)
            {
                CharacterActions.TakeAction(_State, ai.Action, _Character, new List<CombatManager.Character>() {_Character}, ai.Tag);
                Dismiss();
            }
        }

    }
}

