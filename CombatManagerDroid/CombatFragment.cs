
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using CombatManager;
using Android.Webkit;

namespace CombatManagerDroid
{
    public class CombatFragment : Fragment
    {
        static CombatState _CombatState;
        static Character _ViewCharacter;

        ListView _MonsterList;
        ListView _PlayerList;

        public override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            if (_CombatState == null)
            {
                LoadCombatState();
                /*_CombatState = new CombatState();

                for (int i=0; i<6; i++)
                {
                    Character c = new Character(Monster.Monsters[i], true);
                    c.IsMonster = (i%2==0)?true:false;
                    _CombatState.AddCharacter(c);
                }*/
            }
            _CombatState.PropertyChanged += HandleCombatStatePropertyChanged;
            _CombatState.Characters.CollectionChanged += HandledCombatStateCharactersChanged;


        }

        void HandledCombatStateCharactersChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_MonsterList != null)
            {
                _MonsterList.SetAdapter(new CharacterListAdapter(_CombatState, true));
            }
            if (_PlayerList != null)
            {
                _PlayerList.SetAdapter(new CharacterListAdapter(_CombatState, false));
            }
            
            SaveCombatState();
        }

        void HandleCombatStatePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCharacter")
            {
                UpdateCurrentCharacter(View);
                ((BaseAdapter)View.FindViewById<ListView>
                    (Resource.Id.initiativeList).Adapter).NotifyDataSetChanged();

                SaveCombatState();
            }
        }

        public override Android.Views.View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.Combat, container, false);

            v.FindViewById<Button>(Resource.Id.nextButton).Click += 
                delegate {NextClicked();};
            v.FindViewById<Button>(Resource.Id.prevButton).Click += 
                delegate {PrevClicked();};
            v.FindViewById<Button>(Resource.Id.upButton).Click += 
                delegate {UpClicked();};
            v.FindViewById<Button>(Resource.Id.downButton).Click += 
                delegate {DownClicked();};
            v.FindViewById<Button>(Resource.Id.rollInitiativeButton).Click += 
            delegate {RollInitiativeClicked();};

            
            UpdateCurrentCharacter(v);

            ListView lv = v.FindViewById<ListView>(Resource.Id.initiativeList);
            lv.SetAdapter (
                new InitiativeListAdapter(_CombatState));
            
            lv.ItemClick +=  (sender, e) => {
                Character c = ((BaseAdapter<Character>)lv.Adapter)[e.Position];
                ShowCharacter(v, c);
            };

            AddCharacterList(inflater, container, v, Resource.Id.playerListLayout, true);
            AddCharacterList(inflater, container, v, Resource.Id.monsterListLayout, false);

            ShowCharacter(v, _ViewCharacter);

            return v;
        }

        private void AddCharacterList(LayoutInflater inflater, ViewGroup container, View v, int id, bool monsters)
        {
            LinearLayout cl = (LinearLayout)inflater.Inflate(Resource.Layout.CharacterList, container, false);

            cl.LayoutParameters = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent, 1f);

            ListView lv = cl.FindViewById<ListView>(Resource.Id.characterList);

            lv.SetAdapter(new CharacterListAdapter(_CombatState, monsters));
            lv.ItemSelected += (sender, e) => {
                Character c = ((BaseAdapter<Character>)lv.Adapter)[e.Position];
                ShowCharacter(v, c);
            };
            lv.ItemClick +=  (sender, e) => {
                Character c = ((BaseAdapter<Character>)lv.Adapter)[e.Position];
                ShowCharacter(v, c);
            };
            if (!monsters)
            {
                _PlayerList = lv;
            }
            else
            {
                _MonsterList = lv;
            }

            cl.FindViewById<ImageButton>(Resource.Id.blankButton).Click += 
                (object sender, EventArgs e) => 
                {

                    _CombatState.AddBlank(monsters);

                };


            cl.FindViewById<ImageButton>(Resource.Id.monsterButton).Click += 
                (object sender, EventArgs e) => 
                {
                MonsterPickerDialog dl = new MonsterPickerDialog(v.Context, monsters, _CombatState);
                dl.Show();

                };

            cl.FindViewById<Button>(Resource.Id.clearButton).Click += 
                (object sender, EventArgs e) => 
            {
                AlertDialog.Builder bui = new AlertDialog.Builder(v.Context);
                bui.SetMessage("Clear " + (monsters?"Monsters":"Players") + " List?");
                bui.SetPositiveButton("OK", (a, x) => {
                    List<Character> removeList = new List<Character>(from c in _CombatState.Characters where c.IsMonster == monsters select c);
                        foreach (Character c in removeList)
                    {
                        _CombatState.RemoveCharacter(c);
                    }
                    });
                bui.SetNegativeButton("Cancel", (a, x) => {});
                bui.Show();                
            };


            v.FindViewById<LinearLayout>(id).AddView(cl);

        }

        private void ShowCharacter(View v, Character c)
        {
            if (c != null)
            {
                WebView wv = v.FindViewById<WebView>(Resource.Id.characterView);
                wv.LoadUrl("about:blank");
                wv.LoadData(MonsterHtmlCreator.CreateHtml(c.Monster), "text/html", null);
            }
            _ViewCharacter = c;
        }


        private void NextClicked()
        {
            _CombatState.MoveNext();
        }
        private void PrevClicked()
        {
            _CombatState.MovePrevious();
        }
        private void UpClicked()
        {
            
        }
        private void DownClicked()
        {
            
        }
        private void RollInitiativeClicked()
        {
            _CombatState.RollInitiative();
            _CombatState.SortInitiative();
        }
        private void UpdateCurrentCharacter(View v)
        {
           
            v.FindViewById<TextView>(Resource.Id.characterText).Text 
                = _CombatState.CurrentCharacter == null?"":_CombatState.CurrentCharacter.Name;

            v.FindViewById<TextView>(Resource.Id.roundText).Text =
                "Round " + (_CombatState.Round==null?"":_CombatState.Round.ToString());
        }


        public override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);
        }



        public override void OnDestroy ()
        {
            base.OnDestroy();

            _CombatState.PropertyChanged -= HandleCombatStatePropertyChanged;
            _CombatState.Characters.CollectionChanged -= HandledCombatStateCharactersChanged;
        }

        public static void SaveCombatState()
        {
            XmlLoader<CombatState>.Save(_CombatState, "CombatState.xml", true);
        }

        private void LoadCombatState()
        {
            CombatState state = XmlLoader<CombatState>.Load("CombatState.xml", true);
            if (state == null)
            {
                _CombatState = new CombatState();
            }
            else
            {
                _CombatState = state;
            }
        }
    }
}

