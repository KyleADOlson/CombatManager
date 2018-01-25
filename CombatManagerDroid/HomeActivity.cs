using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using CombatManager;


using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using System.IO;

namespace CombatManagerDroid
{
    [Activity (Label = "Combat Manager", Theme = "@android:style/Theme.Light.NoTitleBar")]

    public class HomeActivity : Activity, ActionBar.ITabListener
    {
        enum HomePage
        {
            Combat = 0 ,
            Monsters = 1,
            Feats = 2,
            Spells = 3,
            Rules = 4,
            Treasure = 5
        }
        
        
        MonsterFragment _MonsterFragment;
        FeatFragment _FeatFragment;
        SpellFragment _SpellFragment;
        RuleFragment _RuleFragment;
        CombatFragment _CombatFragment;
        TreasureFragment _TreasureFragment;
        

        
        ProgressDialog _ProgressDialog;

        List<int> _ButtonId = new List<int>
        {
            Resource.Id.combatButton,
            Resource.Id.monstersButton,
            Resource.Id.featsButton,
            Resource.Id.spellsButton,
            Resource.Id.rulesButton,
            Resource.Id.treasureButton
        };

        List<int> _ImageButtonId = new List<int>
        {
            Resource.Id.combatImageButton,
            Resource.Id.monstersImageButton,
            Resource.Id.featsImageButton,
            Resource.Id.spellsImageButton,
            Resource.Id.rulesImageButton,
            Resource.Id.treasureImageButton
        };

        protected override void OnCreate (Bundle bundle)
        {

            base.OnCreate (bundle);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Button monstersButton = FindViewById<Button>(Resource.Id.monstersButton);
            if (monstersButton != null)
            {

                FindViewById<Button>(Resource.Id.monstersButton).Click += delegate
                {
                    ShowMonsterFragment();
                };
                FindViewById<Button>(Resource.Id.featsButton).Click += delegate
                {
                    ShowFeatsFragment();
                };
                FindViewById<Button>(Resource.Id.spellsButton).Click += delegate
                {
                    ShowSpellsFragment();
                };
                FindViewById<Button>(Resource.Id.combatButton).Click += delegate
                {
                    ShowCombatFragment();
                };
                FindViewById<Button>(Resource.Id.rulesButton).Click += delegate
                {
                    ShowRulesFragment();
                };
                FindViewById<Button>(Resource.Id.treasureButton).Click += delegate
                {
                    ShowTreasureFragment();
                };
            }
            ImageButton b = FindViewById<ImageButton>(Resource.Id.monstersImageButton);
            if (b != null)
            {
                FindViewById<ImageButton>(Resource.Id.monstersImageButton).Click += delegate
                {
                    ShowMonsterFragment();
                };
                FindViewById<ImageButton>(Resource.Id.featsImageButton).Click += delegate
                {
                    ShowFeatsFragment();
                };
                FindViewById<ImageButton>(Resource.Id.spellsImageButton).Click += delegate
                {
                    ShowSpellsFragment();
                };
                FindViewById<ImageButton>(Resource.Id.combatImageButton).Click += delegate
                {
                    ShowCombatFragment();
                };
                FindViewById<ImageButton>(Resource.Id.rulesImageButton).Click += delegate
                {
                    ShowRulesFragment();
                };
                FindViewById<ImageButton>(Resource.Id.treasureImageButton).Click += delegate
                {
                    ShowTreasureFragment();
                };
            }
            b = FindViewById<ImageButton>(Resource.Id.helpButton);
            b.Click += delegate
            {
                ShowHelp();
            };
            b = FindViewById<ImageButton>(Resource.Id.settingsButton);
            b.Click += delegate
            {

                UIUtils.ShowListPopover(b, "Options", new List<string> {
                "Import",
                "Export",
                "Settings",
                },
                (option) =>
                {
                    switch (option)
                    {
                        case 0:
                            ShowImport();
                            break;
                        case 1:
                            ShowExport();
                            break;
                        case 2:
                            ShowSettings();
                            break;
                    }
                });
            };
                
            

            ShowLastFragment();


            //UpdateText();
        }

        void ShowLastFragment()
        {
            switch (LastFragment)
            {
            case HomePage.Combat:
                ShowCombatFragment();
                break;
            case HomePage.Monsters:
                ShowMonsterFragment();
                break;
            case HomePage.Feats:
                ShowFeatsFragment();
                break;
            case HomePage.Spells:
                ShowSpellsFragment();
                break;
            case HomePage.Rules:
                ShowRulesFragment();
                break;
            case HomePage.Treasure:
                ShowTreasureFragment();
                break;
            }
                
        }

        private void SetTabState(HomePage selectedPage)
        {
            for (int i=0; i<6; i++)
            {
                Button b = FindViewById<Button>(_ButtonId[i]);
                if (b != null)
                {
                    b.Selected = i == (int)selectedPage;
                }
                ImageButton ib = FindViewById<ImageButton>(_ImageButtonId[i]);
                if (ib != null)
                {
                    ib.Selected = i == (int)selectedPage;
                }

            }
        }

        private HomePage LastFragment
        {
            get
            {
                return (HomePage)CMPreferences.GetLastMainTab(this);
            }
            set
            {
                CMPreferences.SetLastMainTab(this, (int)value);
            }
        }
        
        private void ShowCombatFragment()
        {
            
            //if (_CombatFragment == null)
            //{
                _CombatFragment = new CombatFragment();
            //}
            LastFragment = HomePage.Combat;
            TransitionBodyFragment(_CombatFragment);
            SetTabState(LastFragment);
        }
        
        private void ShowMonsterFragment()
        {
            
            //if (_MonsterFragment == null)
            //{
                _MonsterFragment = new MonsterFragment();
            //}
            LastFragment = HomePage.Monsters;
            
            TransitionBodyFragment(_MonsterFragment);
            SetTabState(LastFragment);
        }

        static bool _LoadingFragment;
        private void ShowFeatsFragment()
        {
            if (!Feat.FeatsLoaded)
            {
                if (!_LoadingFragment)
                {
                    _LoadingFragment = true;
                    _ProgressDialog = new ProgressDialog(this, (int)ProgressDialogStyle.Spinner);
                    _ProgressDialog.SetCanceledOnTouchOutside(false); 
                    _ProgressDialog.SetMessage("Loading");
                    _ProgressDialog.Show();

                    Thread t = new Thread(() =>
                    {
                        Feat.LoadFeats();
                        RunOnUiThread(() =>
                        {
                            _LoadingFragment = false;
                            _ProgressDialog.Dismiss();
                            FinishShowFeatsFragment();
                        });
                    });
                    t.Start();
                }
            }
            else
            {
                FinishShowFeatsFragment();
            }
        }

        private void FinishShowFeatsFragment()
        {
            
            //if (_FeatFragment == null)
            //{
                _FeatFragment = new FeatFragment();
            //}
            LastFragment = HomePage.Feats;
            TransitionBodyFragment(_FeatFragment);
            SetTabState(LastFragment);
        }

        private void ShowSpellsFragment()
        {

            //if (_SpellFragment == null)
            //{
                _SpellFragment = new SpellFragment();
            //}
            LastFragment = HomePage.Spells;
            
            TransitionBodyFragment(_SpellFragment);
            SetTabState(LastFragment);
        }

        private void ShowRulesFragment()
        {
            if (!Rule.RulesLoaded)
            {
                if (!_LoadingFragment)
                {
                    _LoadingFragment = true;
                    _ProgressDialog = new ProgressDialog(this, (int)ProgressDialogStyle.Spinner); 
                    _ProgressDialog.SetMessage("Loading");
                    _ProgressDialog.SetCanceledOnTouchOutside(false);
                    _ProgressDialog.Show();

                    Thread t = new Thread(() =>
                    {
                        Rule.LoadRules();
                        RunOnUiThread(() =>
                        {
                            _ProgressDialog.Dismiss();
                            FinishLoadRulesFragment();
                            _LoadingFragment = false;
                        });
                    });
                    t.Start();
                }
            }
            else
            {
                FinishLoadRulesFragment();
            }
        }

        private void FinishLoadRulesFragment()
        {
            
            //if (_RuleFragment == null)
            //{
                _RuleFragment = new RuleFragment();
            //}
            LastFragment = HomePage.Rules;

            TransitionBodyFragment(_RuleFragment);
            SetTabState(LastFragment);
        }


        private void ShowTreasureFragment()
        {
            if (!MagicItem.MagicItemsLoaded)
            {
                _ProgressDialog = new ProgressDialog(this, (int)ProgressDialogStyle.Spinner); 
                _ProgressDialog.SetMessage("Loading");
                _ProgressDialog.SetCanceledOnTouchOutside(false);
                _ProgressDialog.Show();

                Thread t = new Thread(() =>
                {
                    MagicItem.LoadMagicItems();
                    RunOnUiThread(() =>
                    {
                        _ProgressDialog.Dismiss();
                        FinishLoadTreasureFragment();
                    });
                });
                t.Start();

            }
            else
            {
                FinishLoadTreasureFragment();
            }

        }

        private void FinishLoadTreasureFragment()
        {
            
            //if (_TreasureFragment == null)
            //{
                _TreasureFragment = new TreasureFragment();
            //}
            LastFragment = HomePage.Treasure;
            TransitionBodyFragment(_TreasureFragment);
            SetTabState(LastFragment);
        }

        private void TransitionBodyFragment(Fragment fragment)
        {
            
            FragmentTransaction ft = FragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.bodyLayout, fragment);
            ft.SetTransition(FragmentTransit.None);
            ft.Commit();
        }


        protected override void OnPause ()
        {
            base.OnPause ();
        }

        void ShowHelp()
        {
          

            AboutDialog dlg = new AboutDialog(this);
            dlg.Show();
        }

        void ShowSettings()
        {
            SettingsDialog dlg = new SettingsDialog(this);
            dlg.Show();
        }

        void ShowImport()
        {
            FileDialog fd = new FileDialog(this, new List<string>() { ".cmx" }, true);
            fd.Show();

            fd.DialogComplete += (object s, FileDialog.FileDialogEventArgs ea) =>
            {

                string name = ea.Filename;
                string fullname = Path.Combine(fd.Folder, name);

                FileInfo file = new FileInfo(fullname);
                ShowImportDialog(fullname);
            };
        }

        void ShowImportDialog(String filename)
        {
            try
            {
                ExportData data = XmlLoader<ExportData>.Load(filename);


                ImportExportDialog ied = new ImportExportDialog(this, data, true);
                ied.Show();
                ied.DialogComplete += (sender, e) =>
                {
                    foreach (Monster m in e.Data.Monsters)
                    {
                        m.DBLoaderID = 0;
                        MonsterDB.DB.AddMonster(m);
                        Monster.Monsters.Add(m);
                    }
                    foreach (Spell s in e.Data.Spells)
                    {
                        s.DBLoaderID = 0;
                        Spell.AddCustomSpell(s);
                    }
                    foreach (Feat s in e.Data.Feats)
                    {
                        s.DBLoaderID = 0;
                        Feat.AddCustomFeat(s);
                    }
                    foreach (Condition s in e.Data.Conditions)
                    {
                        Condition.CustomConditions.Add(s);
                    }
                    if (e.Data.Conditions.Count > 0)
                    {
                        Condition.SaveCustomConditions();
                    }

                    RefreshForImport(e.Data);

                };

            }
            catch (Exception ex)
            {
                DebugLogger.WriteLine(ex.ToString());
            }
        }

        void RefreshForImport(ExportData data)
        {
            if (LastFragment == HomePage.Monsters && data.Monsters.Count > 0)
            {
                _MonsterFragment.RefreshPage();
            }
            if (LastFragment == HomePage.Spells && data.Spells.Count > 0)
            {
                _SpellFragment.RefreshPage();
            }
            if (LastFragment == HomePage.Feats && data.Feats.Count > 0)
            {
                _FeatFragment.RefreshPage();
            }
        }


        void ShowExport()
        {

            ExportData data = ExportData.DataFromDBs();

            ImportExportDialog ied = new ImportExportDialog(this, data, true);
            ied.Show();
            ied.DialogComplete += (sender, e) =>
            {
                FileDialog fd = new FileDialog(this, new List<string>() { ".cmx" }, false);
                fd.Text = "export.cmx";
                fd.Show();

                fd.DialogComplete += (object s, FileDialog.FileDialogEventArgs ea) =>
                {

                    string name = ea.Filename;
                    string fullname = Path.Combine(fd.Folder, name);

                    FileInfo file = new FileInfo(fullname);

                    fullname = fullname.TrimEnd('.') + ".cmx";

                    XmlLoader<ExportData>.Save(e.Data, fullname);

                };
            };
        }


        /*private void CreateLibraryAdapter()
        {
            new Thread(new ThreadStart(delegate {
               
                IList<String> str = null;

                if (MonsterText.Text.Trim().Length == 0)
                {
                    str = new List<String>(
                    from x in Monster.Monsters orderby x.Name ascending select x.Name );
                }
                else
                {
                    
                    str = new List<String>(
                        from x in Monster.Monsters where x.Name.ToUpper().Contains(MonsterText.Text.ToUpper())
                            orderby x.Name ascending select x.Name );
                }
                RunOnUiThread(delegate {
                       
                    //LibraryListView.Adapter = new LibraryListAdapter(CoreContext.Context, str);

                    FinishSelection();
                });
            })).Start();

        }*/

        private void FinishSelection()
        {

        }


        public void OnTabReselected (Android.App.ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public void OnTabSelected (Android.App.ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public void OnTabUnselected (Android.App.ActionBar.Tab tab, FragmentTransaction ft)
        {
        }


        /*private void UpdateText()
        {
            WebView view = FindViewById<WebView>(Resource.Id.webView1);
            
            EditText monsterText = FindViewById<EditText>(Resource.Id.editText1);

            if (monsterText.Text.Trim().Length >0)
            {
                var v = from x in CombatManager.Monster.Monsters where x.Name.ToUpper().Contains(monsterText.Text.ToUpper()) select x;
                var enumer = v.GetEnumerator();
                if (enumer.MoveNext())
                {
                    Monster vo = enumer.Current;
                    view.LoadData(MonsterHtmlCreator.CreateHtml(vo), "text/html", null);
                    
                }
                else
                {
                    
                    Monster m = CombatManager.Monster.Monsters[0];
                    view.LoadData(MonsterHtmlCreator.CreateHtml(m), "text/html", null);
                }
            }
        }*/
    }

   
}


