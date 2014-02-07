
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
        CharacterActionsAdapter _CharacterActionsAdapter;

        public ActionDialog(Context context, CombatState state) : base (context)
        {
            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.ActionDialog);
            SetCanceledOnTouchOutside(true);

            _State = state;

            Button close = (Button)FindViewById(Resource.Id.closeButton);
            close.Click += delegate
                {
                    Dismiss ();
                };

            Button back  = (Button)FindViewById(Resource.Id.backButton);
            back.Click += delegate
            {
                GoBack ();
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

                wv.LoadDataWithBaseURL(null, MonsterHtmlCreator.CreateHtml(_Character.Monster), "text/html", "utf-8", null);

                ListView lv = (ListView)FindViewById(Resource.Id.actionListView);
                lv.ItemClick += ListViewItemClick;

                var ca = new CharacterActionsAdapter(Context, _Character, _State);
                lv.SetAdapter(ca);
                _CharacterActionsAdapter = ca;
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
                CharacterActionResult res = CharacterActions.TakeAction(_State, ai.Action, _Character, new List<CombatManager.Character>() {_Character}, ai.Tag);
                Dismiss();

                switch(res)
                {
                case CharacterActionResult.NeedAttacksDialog:
                    break;
                case CharacterActionResult.NeedMonsterEditorDialog:
                    ShowMonsterEditor();
                    break;
                case CharacterActionResult.NeedConditionDialog:
                    ShowConditionDialog();
                    break;
                case CharacterActionResult.NeedNotesDialog:
                    ShowNotesDialog();
                    break;
                case CharacterActionResult.RollAttack:
                    _State.Roll(CombatState.RollType.Attack, _Character, (Attack)ai.Tag, null); 
                    break;
                case CharacterActionResult.RollAttackSet:
                    _State.Roll(CombatState.RollType.AttackSet, _Character, (AttackSet)ai.Tag, null); 
                    break;
                case CharacterActionResult.RollSave:

                    _State.Roll(CombatState.RollType.Save, _Character, (Monster.SaveType)ai.Tag, null);
                    break;

                case CharacterActionResult.RollSkill:
                    var sks = (Tuple<string, string>)ai.Tag;
                    _State.Roll(CombatState.RollType.Skill, _Character, sks.Item1, sks.Item2);
                    break;
                }
            }
        }

        void ShowMonsterEditor()
        {
            MonsterEditorActivity.SourceMonster = _Character.Monster;

            Intent intent = new Intent(this.Context, (Java.Lang.Class) new MonsterEditorMainActivity().Class); 
            intent.AddFlags(ActivityFlags.NewTask); 
            Context.StartActivity(intent);
        }

        void ShowConditionDialog()
        {
            ConditionDialog dl = new ConditionDialog(this.Context, _State, _Character);
            dl.Show();
        }
        void ShowNotesDialog()
        {
            UIUtils.ShowTextDialog("Notes", _Character, Context ,true);
        }

        void GoBack()
        {
            if (_CharacterActionsAdapter != null)
            {
                _CharacterActionsAdapter.MoveBack();
                _CharacterActionsAdapter.NotifyDataSetChanged();
            }
        }
    }
}

