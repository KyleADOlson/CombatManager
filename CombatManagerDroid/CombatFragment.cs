
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
using System.IO;

namespace CombatManagerDroid
{
    public class CombatFragment : Fragment
    {
        static CombatState _CombatState;
        static Character _ViewCharacter;

        static Character _SelectedCharacter;
       
        ListView _MonsterList;
        ListView _PlayerList;

        static TextView _XPText;

        InitiativeListAdapter _InitListAdapter;

        //die roller
        static string _DieText = "";
        static List<object> _RollResults = new List<object>();

        static List<string> _Extensions = new List<string>()
        {
            ".cmpt",
            ".por",
            ".rpgrp"
        };


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
            _CombatState.RollRequested += HandleRollRequested;
           
            _CombatState.CombatList.CollectionChanged += HandleCombatListChanged;
            _CombatState.CharacterSortCompleted += HandleCharacterSortCompleted;

        }

        void HandleCharacterSortCompleted (object sender, EventArgs e)
        {
            
            ReloadInitiativeList();
        }

        void HandleCombatListChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!_CombatState.SortingList)
            {
                ReloadInitiativeList();
            }
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

        void HandleRollRequested (object sender, CombatState.RollEventArgs e)
        {
            _RollResults.Add(e.Roll);
            ShowDieRolls(View);
        }

        bool reloadingList;
        void ReloadInitiativeList()
        {
            if (!reloadingList)
            {
                reloadingList = true;
                _InitListAdapter.NotifyDataSetChanged();
                reloadingList = false;
            }

           
        }

        void ReloadXPText()
        {
            if (_CombatState.XP == null)
            {
                _XPText.Text = null;
            }
            else
            {
                _XPText.Text = "CR: " + (_CombatState.XP.Value > 0?Monster.EstimateCR(_CombatState.XP.Value):"0") + " XP: " + _CombatState.XP;
            }

        }

        void HandleCombatStatePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCharacter")
            {
                UpdateCurrentCharacter(View);
                ReloadInitiativeList();
                SaveCombatState();
            }
            if (e.PropertyName == "XP")
            {
                ReloadXPText();
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
            v.FindViewById<Button>(Resource.Id.sortButton).Click += (object sender, EventArgs e) => 
            {
                SortInitiativeClicked();
            };
            v.FindViewById<Button>(Resource.Id.resetButton).Click += (object sender, EventArgs e) => 
            {
                ResetInitiativeClicked();
            };


            UpdateCurrentCharacter(v);

            ListView lv = v.FindViewById<ListView>(Resource.Id.initiativeList);
            _InitListAdapter = new InitiativeListAdapter(_CombatState, v);
            lv.SetAdapter (_InitListAdapter);

            _InitListAdapter.CharacterClicked += (sender, e) =>
            {
                if (_SelectedCharacter != e.Character)
                {
                    _SelectedCharacter = e.Character;
                    _InitListAdapter.Character = e.Character;
                    _InitListAdapter.NotifyDataSetChanged();
                }
                ShowCharacter(v, e.Character);
            };
            
            lv.ItemClick +=  (sender, e) => {
                Character c = ((BaseAdapter<Character>)lv.Adapter)[e.Position];
                if (_SelectedCharacter != c)
                {
                    _SelectedCharacter = c;
                    _InitListAdapter.Character = _SelectedCharacter;
                    _InitListAdapter.NotifyDataSetChanged();
                }
                ShowCharacter(v, c);

            };




            AddCharacterList(inflater, container, v, Resource.Id.playerListLayout, false);
            AddCharacterList(inflater, container, v, Resource.Id.monsterListLayout, true);

            ShowCharacter(v, _ViewCharacter);

            SetupDieRoller(v);

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

            lv.SetOnDragListener(new ListOnDragListener(monsters, v));

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

            cl.FindViewById<ImageButton>(Resource.Id.loadButton).Click += 
                (object sender, EventArgs e) => 
            {

                FileDialog fd = new FileDialog(cl.Context, _Extensions, true);
                fd.Show();
                
                fd.DialogComplete += (object s, FileDialog.FileDialogEventArgs ea) => 
                {
                    
                    string name = ea.Filename;
                    string fullname = Path.Combine(fd.Folder, name);

                    FileInfo file = new FileInfo(fullname);

                    if (String.Compare(".por", file.Extension, true) == 0 || String.Compare(".rpgrp", file.Extension, true) == 0)
                    {
                        List<Monster> importmonsters = Monster.FromFile(fullname);

                        if (importmonsters != null)
                        {
                            foreach (Monster m in importmonsters)
                            {
                                Character ch = new Character(m, false);
                                ch.IsMonster = monsters;
                                _CombatState.AddCharacter(ch);
                            }
                        }
                    }
                    else
                    {

                        List<Character> l = XmlListLoader<Character>.Load(fullname);
                        foreach (var c in l)
                        {
                            c.IsMonster = monsters;
                            _CombatState.AddCharacter(c);
                        }
                    }

                };

            };

            cl.FindViewById<ImageButton>(Resource.Id.saveButton).Click += 
                (object sender, EventArgs e) => 
            {
                FileDialog fd = new FileDialog(v.Context, _Extensions, false);
                fd.DialogComplete += (object s, FileDialog.FileDialogEventArgs ea) => 
                {
                    string name = ea.Filename;
                    if (!name.EndsWith(".cmpt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        name = name+".cmpt";
                    }
                    string fullname = Path.Combine(fd.Folder, name);

                    XmlListLoader<Character>.Save(new List<Character>(_CombatState.Characters.Where((a)=>a.IsMonster == monsters)), fullname);
                };
                fd.Show();
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

            if (monsters)
            {
                _XPText = cl.FindViewById<TextView>(Resource.Id.xpText);
                ReloadXPText();
            }



            v.FindViewById<LinearLayout>(id).AddView(cl);

        }

        class ListOnDragListener : Java.Lang.Object, View.IOnDragListener 
        {

            View _listview;
            bool _monsters;

            public ListOnDragListener(bool monsters, View listview)
            {
                _listview = listview;
                _monsters = monsters;
            }

            public bool OnDrag(View v, DragEvent e)
            {
                switch (e.Action)
                {
                case DragAction.Entered:
                    break;
                case DragAction.Exited:
                    break;
                case DragAction.Ended:
                    break;
                case DragAction.Drop:
                    View dropView = (View)e.LocalState;
                    if (dropView.Parent is ListView)
                    {
                        if (((ListView)dropView.Parent).Adapter is CharacterListAdapter)
                        {
                            CharacterListAdapter dropAd = (CharacterListAdapter)((ListView)dropView.Parent).Adapter;
                            Character dropChar = dropAd[(int)dropView.Tag];

                            CombatFragment.CombatState.MoveDroppedCharacter(dropChar, null, _monsters);

                        }
                    }
                    break;
                }
                return true;
            }
        }


        private void ShowCharacter(View v, Character c)
        {
            if (c != null)
            {
                WebView wv = v.FindViewById<WebView>(Resource.Id.characterView);

                wv.LoadDataWithBaseURL(null, MonsterHtmlCreator.CreateHtml(c.Monster), "text/html", "utf-8", null);
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
            if (_SelectedCharacter != null)
            {
                if (_CombatState.CombatList.Contains(_SelectedCharacter))
                {
                    _CombatState.MoveUpCharacter(_SelectedCharacter);
                }
            }
        }
        private void DownClicked()
        {
            if (_SelectedCharacter != null)
            {
                if (_CombatState.CombatList.Contains(_SelectedCharacter))
                {
                    _CombatState.MoveDownCharacter(_SelectedCharacter);
                }
            }
        }
        private void RollInitiativeClicked()
        {
            Action func = () =>
            {
                _CombatState.RollInitiative();
                _CombatState.SortInitiative();
            };
            if (!Activity.GetCMPrefs().GetConfirmInitiative())
            {
                func();
            }
            else
            {
                UIUtils.ShowOKCancelDialog(Activity, "Roll Initiative?", func);
            }

        }

        private void SortInitiativeClicked()
        {
            Action func = () =>
            {
                _CombatState.SortInitiative(); 
            };
            if (!Activity.GetCMPrefs().GetConfirmInitiative())
            {
                func();
            }
            else
            {
                UIUtils.ShowOKCancelDialog(Activity, "Sort Initiative?", func);
            }
        }

        private void ResetInitiativeClicked()
        {
            
            Action func = () =>
            {
                foreach (Character ch in _CombatState.Characters)
                {
                    ch.CurrentInitiative = 0;
                }
            };
            if (!Activity.GetCMPrefs().GetConfirmInitiative())
            {
                func();
            }
            else
            {
                UIUtils.ShowOKCancelDialog(Activity, "Reset Initiative?", func);
            }
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
            _CombatState.RollRequested -= HandleRollRequested;
            _CombatState.CombatList.CollectionChanged -= HandleCombatListChanged;
            
            _CombatState.CharacterSortCompleted -= HandleCharacterSortCompleted;
        }

        public static void SaveCombatState()
        {
            try
            {
                XmlLoader<CombatState>.Save(_CombatState, "CombatState.xml", true);
            }
            catch (Exception ex)
            {

                DebugLogger.WriteLine(ex.ToString());
            }
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
            _CombatState.SortCombatList(false, false, true);
            _CombatState.FixInitiativeLinksList(new List<Character>(_CombatState.Characters));

            ISharedPreferences sp = Activity.GetSharedPreferences("CombatManager", 0);
            CombatState.use3d6 = sp.GetBoolean("use3d6", false);
        }

        public static CombatState CombatState
        {
            get
            {
                return _CombatState;
            }
        }

        void SetupDieRoller(View v)
        {
            List<int> buttons = new List<int>() { Resource.Id.rollD4Button, Resource.Id.rollD6Button,
                Resource.Id.rollD8Button, Resource.Id.rollD10Button, Resource.Id.rollD12Button,
                Resource.Id.rollD20Button, Resource.Id.rollD100Button
            };
            foreach (int id in buttons)
            {
                ImageButton b = v.FindViewById<ImageButton>(id);
                int die = 0;
                int.TryParse(((String)b.Tag), out die);

              
                b.Click += (object sender, EventArgs e) => {AddDieRoll(die);};
            }

            View cb = v.FindViewById<View>(Resource.Id.clearInitiativeButton);
            cb.Click += (object sender, EventArgs e) => {ClearAllRolls();};

            ImageButton clearButton = v.FindViewById<ImageButton>(Resource.Id.clearRollButton);
            clearButton.Click += (object sender, EventArgs e) => {ClearRoll();};


            ImageButton rollButton = v.FindViewById<ImageButton>(Resource.Id.rollButton);
            rollButton.Click += (object sender, EventArgs e) => {Roll();};

            
            EditText dieText = v.FindViewById<EditText>(Resource.Id.rollText);
            dieText.Text = _DieText;
            dieText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => 
            {
                _DieText = dieText.Text;
                };
            
            WebView wv = v.FindViewById<WebView>(Resource.Id.rollWebView);  
            wv.SetPictureListener(new DieViewClient());
            ShowDieRolls(v);


        }

 

        private class DieViewClient : Java.Lang.Object, WebView.IPictureListener
        {
            public void OnNewPicture(WebView view, Android.Graphics.Picture picture) {
                //view.PageDown(true);
            }

        }

        void AddDieRoll(int die)
        {
            EditText dieText = View.FindViewById<EditText>(Resource.Id.rollText);

            DieRoll roll = DieRoll.FromString(dieText.Text);
            if (roll == null)
            {
                roll = new DieRoll(1, die, 0);
            }
            else
            {
                roll.AddDie(new DieStep(1, die)); 
            }

            dieText.Text = roll.Text;
        }

        void ClearRoll()
        {
            
            EditText dieText = View.FindViewById<EditText>(Resource.Id.rollText);
            dieText.Text = "";
        }

        void Roll()
        {
            
            EditText dieText = View.FindViewById<EditText>(Resource.Id.rollText);

            DieRoll roll = DieRoll.FromString(dieText.Text);

            Roll(roll);
        }

        void Roll (DieRoll roll)
        {
            if (roll != null)
            {
                RollResult res = roll.Roll();

                _RollResults.Add(res);

                while (_RollResults.Count > 50)
                {
                    _RollResults.RemoveAt(0);
                }

                ShowDieRolls(View);
            }
        }

        void ShowDieRolls(View v)
        {
            
            WebView wv = v.FindViewById<WebView>(Resource.Id.rollWebView);  
            //wv.LoadUrl("about:blank");
            //wv.LoadData(, "text/html", null);
            wv.LoadDataWithBaseURL(null, RollResultHtmlCreator.CreateHtml(_RollResults), "text/html", "utf-8", null);
            wv.PageDown(true);
        }

        void ClearAllRolls()
        {
            _RollResults.Clear();
            ShowDieRolls(this.View);
        }

    }
}

