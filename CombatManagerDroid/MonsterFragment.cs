
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
    class MonsterFragment : LookupFragment<Monster>
    {
        //filters
        int _group;
        string _type = "All";
        string _cr = "All";

        //advancer
        int _advMuliplier = 1;
        int _sizeChange = 0;
        int _sizeMultiplier = 1;
        int _outsiderChange = 0;
        int _hdChange = 1;
        int _hdCount = 1;
        int _bonusStat = 0;
        bool _sizeChange50HD = false;
        int _selectedTemplate = 0;

        int _zombieType = 0;
        string _dragonColor = "black";


        Button _AdvancerButton;
        Button _AddButton;
        LinearLayout _AdvancerContainer;
        View _AdvancerLayout;

        bool _AdvancerVisible;

        protected override List<Monster> GetItems ()
        {
            return new List<Monster>(Monster.Monsters);
        }

        protected override string ItemHtml (Monster item)
        {
            return MonsterHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (Monster item)
        {
            return item.Name;
        }

        protected override bool CustomFilterItem(Monster item)
        {
            return NPCFilter(item) && TypeFilter(item) && CRFilter(item);
        }

        bool NPCFilter(Monster item)
        {
            return _group == 0 || (_group == 1 && !item.NPC) || (_group == 2 && item.NPC);
        }

        bool TypeFilter(Monster item)
        {
            return _type.StartsWith("All") || String.Compare(item.Type, _type, true) == 0;
        }

        bool CRFilter(Monster item)
        {
            return _cr.StartsWith("All") || item.CR == _cr;
        }


        protected override void BuildFilters()
        {
            Button b = BuildFilterButton("group", 100);
           
            PopupUtils.AttachButtonStringPopover("Group", b, 
                new List<string> { "All Monsters", "Monsters", "NPCs" }, 
                                            _group, (r1, index, val)=>
            {
                _group = index;
                UpdateFilter();

            });
            b = BuildFilterButton("type", 180);

            List<String> cts = new List<String>(from s in Monster.CreatureTypeNames
                                                         select s.Capitalize());

            cts.Insert(0, "All Types");
            PopupUtils.AttachButtonStringPopover("Type", b, 
                                                cts, 
            0, (r1, index, val)=>
            {
                _type = val;
                UpdateFilter();

            });
            b = BuildFilterButton("cr", 80);
            List<string> crs = new List<string>(Monster.CRList.Values);
            crs.Insert(0, "All CRs");
            PopupUtils.AttachButtonStringPopover("CR", b, 
                                                 crs, 
                                                 0, (r1, index, val)=>
                                                 {
                _cr = val;
                UpdateFilter();

            });

        }
        protected override void BuildAdditionalLayouts()
        {
            LinearLayout barLayout = new LinearLayout(_v.Context);
            BottomLayout.AddView(barLayout);
            barLayout.Orientation = Orientation.Horizontal;

            _AdvancerButton = new Button(_v.Context);
            _AdvancerButton.Text = "Monster Advancer";
            _AdvancerButton.SetCompoundDrawablesWithIntrinsicBounds(Resources.GetDrawable(Resource.Drawable.monster16), null, null, null);
            barLayout.AddView(_AdvancerButton);

            _AdvancerButton.Click += (object sender, EventArgs e) => 
            {
                _AdvancerVisible = !_AdvancerVisible;
                _AdvancerContainer.Visibility = _AdvancerVisible?ViewStates.Visible:ViewStates.Gone;
                RefreshItem();
            };

            _AddButton = new Button(_v.Context);
            _AddButton.Text = "Add to Combat";
            _AddButton.SetCompoundDrawablesWithIntrinsicBounds(Resources.GetDrawable(Resource.Drawable.sword16), null, null, null);
            barLayout.AddView(_AddButton);
            _AddButton.Click += (object sender, EventArgs e) => 
            {
                Monster m = CurrentMonster;
                if (m != null)
                {

                    CombatFragment.CombatState.AddMonster(m, Activity.GetCMPrefs().GetRollHP(), true);
                }
            };

            _AdvancerContainer = new LinearLayout(_v.Context);
            BottomLayout.AddView(_AdvancerContainer);

            LayoutInflater vi = (LayoutInflater)_v.Context.GetSystemService(Context.LayoutInflaterService);
            _AdvancerLayout = vi.Inflate(Resource.Layout.MonsterAdvancer, _AdvancerContainer, false);
            _AdvancerContainer.Visibility = ViewStates.Gone;
            _AdvancerContainer.AddView(_AdvancerLayout);

            
            CheckBox cb = _AdvancerLayout.FindViewById<CheckBox>(Resource.Id.advancedBox);
            cb.Click += (object sender, EventArgs e) => { RefreshItem(); };


            Button b;
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.advancedMultiplierButton);
            List<String> advMult = new List<string>() { "x1", "x2", "x3" };
            PopupUtils.AttachButtonStringPopover("Multiplier", b, 
                                                 advMult, 
                                                 0, (r1, index, val)=>
                                                 {
                _advMuliplier = index+1;
                RefreshItem();

            });

            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.sizeButton);
            List<String> sizeOpt = new List<string>() { "Young", "Normal", "Giant" };
            PopupUtils.AttachButtonStringPopover("Size", b, 
                                                 sizeOpt, 
                                                 0, (r1, index, val)=>
                                                 {
                _sizeChange = index -1;
                RefreshItem();

            });

            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.sizeMultiplierButton);
            List<String> sizeMult = new List<string>() { "x1", "x2" };
            PopupUtils.AttachButtonStringPopover("Multiplier", b, 
                                                 sizeMult, 
                                                 1, (r1, index, val)=>
                                                 {
                _sizeMultiplier = index +1;
                RefreshItem();

            });

            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.outsiderButton);
            List<String> outsiderList = new List<string>() { "None", "Celestial", "Entropic", "Fiendish", "Resoloute" };
            PopupUtils.AttachButtonStringPopover("Multiplier", b, 
                                                 outsiderList, 
                                                 0, (r1, index, val)=>
                                                 {
                _outsiderChange = index;
                RefreshItem();

            });

            
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.hdButton);
            List<String> hdList = new List<string>() { "Remove HD", "No HD", "Add HD" };
            PopupUtils.AttachButtonStringPopover("Change HD", b, 
                                                 hdList, 
                                                 1, (r1, index, val)=>
                                                 {
                _hdChange = index;
                RefreshItem();

            });

            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.hdCountButton);
            List<String> hdCountList = new List<string>();
            for (int i=1; i<=20; i++)
            {
                hdCountList.Add(i.ToString());
            }
            PopupUtils.AttachButtonStringPopover("HD Count", b, 
                                                 hdCountList, 
                                                 0, (r1, index, val)=>
                                                 {
                _hdCount = index + 1;
                RefreshItem();

            });

            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.bonusStatButton);
            List<String> statList = new List<string>();
            for (int i=0; i<6; i++)
            {
                statList.Add(Monster.StatText((Stat)i));
            }
            PopupUtils.AttachButtonStringPopover("Bonus Stat", b, 
                                                 statList, "Bonus Stat {0}",
                                                 0, (r1, index, val)=>
                                                 {
                _bonusStat = index;
                RefreshItem();

            });
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.statChangeButton);
            List<String> sizeChange50List = new List<string>() {"No Size Change", "Size Change 50% HD"};
            PopupUtils.AttachButtonStringPopover("Change Size", b, 
                                                 sizeChange50List, 
                                                 0, (r1, index, val)=>
                                                 {
                _sizeChange50HD = index == 1;
                RefreshItem();

            });

            
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.templateButton);
            List<String> templateList = new List<string>() {"No Template",
                "Half-Dragon", "Half-Celestial", "Half-Fiend", "Skeleton",
                "Vampire", "Zombie"};
            PopupUtils.AttachButtonStringPopover("Template", b, 
                                                 templateList, 
                                                 0, (r1, index, val)=>
                                                 {
                _selectedTemplate = index;
                UpdateTemplateView();
                RefreshItem();

            });

            
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.dragonColorButton);
            List<String> colorList = new List<string>(Monster.DragonColors );
            PopupUtils.AttachButtonStringPopover("Color", b, 
                                                 colorList, 
                                                 0, (r1, index, val)=>
                                                 {
                _dragonColor = val;
                RefreshItem();

            });

            
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.zombieButton);
            List<String> zombieList = new List<string>() { "Regular", "Fast", "Plague" };
            PopupUtils.AttachButtonStringPopover("Template", b, 
                                                 zombieList, 
                                                 0, (r1, index, val)=>
                                                 {
                _zombieType = index;
                RefreshItem();

            });


            cb = _AdvancerLayout.FindViewById<CheckBox>(Resource.Id.augmentSummoningBox);
            cb.Click += (object sender, EventArgs e) => { RefreshItem(); };

            foreach (int id in new List<int>(){
                Resource.Id.strBox,
                Resource.Id.dexBox,
                Resource.Id.conBox,
                Resource.Id.intBox,
                Resource.Id.wisBox,
                Resource.Id.chaBox,
                Resource.Id.bloodyBox,
                Resource.Id.burningBox,
                Resource.Id.championBox,})
            {
                
                cb = _AdvancerLayout.FindViewById<CheckBox>(id);
                cb.Click += (object sender, EventArgs e) => { RefreshItem(); };
            }

            UpdateTemplateView();

        }

        protected override void ShowItem(Monster item)
        {
            Monster monster = AdvanceMonster(item);

           

            base.ShowItem(monster);
        }

        bool AdvancerBoxChecked(int id)
        {
            
            CheckBox cb = _AdvancerLayout.FindViewById<CheckBox>(id);
            return cb.Checked;
        }

        Monster AdvanceMonster(Monster item)
        {
            Monster monster = item;

            if (monster != null && _AdvancerVisible)
            {
                monster = (Monster)item.Clone();

                if (_hdChange != 1)
                {
                    int dice = _hdCount * (_hdChange-1);

                    Stat stat = (Stat)_bonusStat;

                    bool size = _sizeChange50HD;

                    int res = monster.AddRacialHD(dice, stat, size);

                    if (res != 0)
                    {
                        monster.Name = monster.Name + " " + CMStringUtilities.PlusFormatNumber(res) + " HD";
                    }
                }

                if (_selectedTemplate == 1)
                {
                    if (monster.MakeHalfDragon(_dragonColor))
                    {
                        monster.Name = "Half-Dragon " + monster.Name;
                    }
                }
                if (_selectedTemplate == 2)
                {
                    HashSet<Stat> bonusStats = new HashSet<Stat>();

                    if (AdvancerBoxChecked(Resource.Id.strBox))
                    {
                        bonusStats.Add(Stat.Strength);
                    }
                    if (AdvancerBoxChecked(Resource.Id.dexBox))
                    {
                        bonusStats.Add(Stat.Dexterity);
                    }
                    if (AdvancerBoxChecked(Resource.Id.conBox))
                    {
                        bonusStats.Add(Stat.Constitution);
                    }
                    if (AdvancerBoxChecked(Resource.Id.intBox))
                    {
                        bonusStats.Add(Stat.Intelligence);
                    }
                    if (AdvancerBoxChecked(Resource.Id.wisBox))
                    {
                        bonusStats.Add(Stat.Wisdom);
                    }
                    if (AdvancerBoxChecked(Resource.Id.chaBox))
                    {
                        bonusStats.Add(Stat.Charisma);
                    }

                    if (monster.MakeHalfFiend(bonusStats))
                    {
                        monster.Name = "Half-Fiend " + monster.Name;
                    }
                }
                if (_selectedTemplate == 3)
                {
                    HashSet<Stat> bonusStats = new HashSet<Stat>();

                    if (AdvancerBoxChecked(Resource.Id.strBox))
                    {
                        bonusStats.Add(Stat.Strength);
                    }
                    if (AdvancerBoxChecked(Resource.Id.dexBox))
                    {
                        bonusStats.Add(Stat.Dexterity);
                    }
                    if (AdvancerBoxChecked(Resource.Id.conBox))
                    {
                        bonusStats.Add(Stat.Constitution);
                    }
                    if (AdvancerBoxChecked(Resource.Id.intBox))
                    {
                        bonusStats.Add(Stat.Intelligence);
                    }
                    if (AdvancerBoxChecked(Resource.Id.wisBox))
                    {
                        bonusStats.Add(Stat.Wisdom);
                    }
                    if (AdvancerBoxChecked(Resource.Id.chaBox))
                    {
                        bonusStats.Add(Stat.Charisma);
                    }

                    if (monster.MakeHalfCelestial(bonusStats))
                    {
                        monster.Name = "Half-Celestial " + monster.Name;
                    }
                }
                if (_selectedTemplate == 4)
                {
                    bool bloody = AdvancerBoxChecked(Resource.Id.bloodyBox);
                    bool burning = AdvancerBoxChecked(Resource.Id.burningBox);
                    bool champion = AdvancerBoxChecked(Resource.Id.championBox);

                    if (monster.MakeSkeleton(bloody, burning, champion))
                    {

                        if (champion)
                        {
                            monster.Name = "Skeletal Champion " + monster.Name;
                        }
                        else
                        {
                            monster.Name += " Skeleton";
                        }


                        if (burning)
                        {
                            monster.Name = "Burning " + monster.Name;
                        }

                        if (bloody)
                        {
                            monster.Name = "Bloody " + monster.Name;
                        }

                    }
                }
                if (_selectedTemplate == 5)
                {
                    if (monster.MakeVampire())
                    {
                        monster.Name = "Vampire " + monster.Name;
                    }
                }
                if (_selectedTemplate == 6)
                {
                    Monster.ZombieType zt = (Monster.ZombieType)_zombieType;

                    if (monster.MakeZombie(zt))
                    {

                        monster.Name = "Zombie " + monster.Name;

                        if (zt == Monster.ZombieType.Fast)
                        {
                            monster.Name = "Fast " + monster.Name;
                        }
                        else if (zt == Monster.ZombieType.Plague)
                        {
                            monster.Name = "Plague " + monster.Name;
                        }

                    }
                }


                CheckBox cb = _AdvancerLayout.FindViewById<CheckBox>(Resource.Id.advancedBox);

                if (cb.Checked)
                {
                    for (int i=0; i<_advMuliplier; i++)
                    {
                        monster.MakeAdvanced();
                    }
                    int advlevels = _advMuliplier;

                    monster.Name += " (Adv " + advlevels.PlusFormat() + ")";

                }
                int count = _sizeMultiplier;
                if (_sizeChange > 0)
                {

                    int added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (monster.MakeGiant())
                        {
                            added++;
                        }

                    }
                    if (added == 1)
                    {
                        monster.Name = "Giant " + monster.Name;
                    }
                    if (added > 1)
                    {
                        monster.Name = "Giant x" + added + " " + monster.Name;
                    }
                }
                else if (_sizeChange < 0)
                {
                    int added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (monster.MakeYoung())
                        {
                            added++;
                        }
                    }
                    if (added == 1)
                    {
                        monster.Name = "Young " + monster.Name;
                    }
                    if (added > 1)
                    {
                        monster.Name = "Young x" + added + " " + monster.Name;
                    }
                }

                if (_outsiderChange == 1)
                {
                    if (monster.MakeCelestial())
                    {
                        monster.Name = "Celestial " + monster.Name;
                    }
                }
                else if (_outsiderChange == 2)
                {
                    if (monster.MakeEntropic())
                    {
                        monster.Name = "Entopic " + monster.Name;
                    }
                }
                else if (_outsiderChange == 3)
                {
                    if (monster.MakeFiendish())
                    {
                        monster.Name = "Fiendish " + monster.Name;
                    }
                }
                else if (_outsiderChange == 4)
                {
                    if (monster.MakeResolute())
                    {
                        monster.Name = "Resolute " + monster.Name;
                    }
                }

                cb = _AdvancerLayout.FindViewById<CheckBox>(Resource.Id.augmentSummoningBox);
                if (cb.Checked)

                {
                    monster.AugmentSummoning();
                    monster.Name = "Augmented " + monster.Name;
                }
            }
                
                


            return monster;

        }



             

        void UpdateTemplateView()
        {


            Button b;
            LinearLayout l;
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.dragonColorButton);
            b.Visibility = (_selectedTemplate == 1)?ViewStates.Visible:ViewStates.Gone;

            l = _AdvancerLayout.FindViewById<LinearLayout>(Resource.Id.statsLayout);
            l.Visibility = (_selectedTemplate == 3||_selectedTemplate == 2)?ViewStates.Visible:ViewStates.Gone;

            l = _AdvancerLayout.FindViewById<LinearLayout>(Resource.Id.skeletonLayout);
            l.Visibility = (_selectedTemplate == 4)?ViewStates.Visible:ViewStates.Gone;
            
            b = _AdvancerLayout.FindViewById<Button>(Resource.Id.zombieButton);
            b.Visibility = (_selectedTemplate == 6)?ViewStates.Visible:ViewStates.Gone;
        }

        Monster CurrentMonster
        {
            get
            {
                return AdvanceMonster(SelectedItem);

            }
        }
    }
}

