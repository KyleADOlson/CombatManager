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
using Android.Webkit;

namespace CombatManagerDroid
{
    class TreasureFragment : LookupFragment<MagicItem>
    {
        View _GeneratorLayout;

        int _Level = 1;
        int _Coin = 1;
        int _Goods = 1;
        int _MagicItems = 1;

        int _ItemCount = 1;
        int _ItemLevel;

        static Treasure _LastTreasure;

        static bool _GeneratorVisible;
        static bool _LevelVisible = true;

        static string _Group;
        static int _CL = -1;

        ImageButton lookupButton;
        ImageButton generatorButton;

        protected override List<MagicItem> GetItems ()
        {

            return new List<MagicItem>(MagicItem.Items.Values);
        }

        protected override string ItemHtml (MagicItem item)
        {

            return MagicItemHtmlCreator.CreateHtml(item);
        }

        protected override string ItemName (MagicItem item)
        {
            return item.Name;
        }


        protected override void BuildFilters()
        {
            Button b = BuildFilterButton("group", 100);

            
            List<String> groups = new List<string>() { "All Groups" };
            groups.AddRange(MagicItem.Groups);
            int startindex = groups.IndexOf(_Group);
            if (startindex < 0)
            {
                startindex = 0;
            }
            PopupUtils.AttachButtonStringPopover("Group", b, 
                                                 groups,
            startindex, (r1, index, val)=>
            {
                if (index == 0)
                {
                    _Group = "";
                }
                else
                {
                    _Group = val;
                }
                UpdateFilter();

            });

            b = BuildFilterButton("cl", 80);
            List<string> cls = new List<string>() {"All CLs"};
            cls.AddRange(from x in MagicItem.CLs select x.ToString());
            startindex = 0;
            if (_CL != -1)
            {
                startindex = cls.IndexOf(_CL.ToString());
            }
            PopupUtils.AttachButtonStringPopover("CL", b, 
                                                 cls,
                                                 startindex, (r1, index, val)=>
                                                 {
                if (index == 0)
                {
                    _CL = -1;
                }
                else
                {
                    int.TryParse(val, out _CL);
                }
                UpdateFilter();

            });
        }
        protected override bool CustomFilterItem(MagicItem item)
        {
            return GroupFilter(item) && CLFilter(item);
        }

        bool GroupFilter(MagicItem item)
        {
            return _Group == null || _Group == "" || item.Group == _Group;
        }

        bool CLFilter(MagicItem item)
        {
            return _CL == -1 || item.CL == _CL;
        }

        protected override void BuildAdditionalLayouts()
        {
            ImageButton ib = new ImageButton(_v.Context);
            ib.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.wand16));
            LeftLayout.AddView(ib);
            ib.Click += (object sender, EventArgs e) => {ShowLookup();};
            ib.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.main_tab));
            ib.Selected = true;
            lookupButton = ib;

            ib = new ImageButton(_v.Context);
            ib.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.dice16));
            LeftLayout.AddView(ib);
            ib.Click += (object sender, EventArgs e) => {ShowGenerator();};
            ib.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.main_tab));
            generatorButton = ib;

            LayoutInflater vi = (LayoutInflater)_v.Context.GetSystemService(Context.LayoutInflaterService);
            _GeneratorLayout = vi.Inflate(Resource.Layout.TreasureGenerator, SearchReplacementLayout, false);
            SearchReplacementLayout.Visibility = ViewStates.Gone;
            SearchReplacementLayout.AddView(_GeneratorLayout);

            Button b;
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.byLevelButton);
            b.Click += (object sender, EventArgs e) => 
            {
                ShowLevel(true);
            };
            b.Selected = true;
            b.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.main_tab));
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.byItemsButton);
            b.Click += (object sender, EventArgs e) => 
            {
                ShowLevel(false);
            };
            b.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.main_tab));
            ShowLevel(_LevelVisible);

            //level generation
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.levelButton); 
                  
            List<String> levelList = new List<string>();
            for (int i=1; i<=20; i++)
            {
                levelList.Add(i.ToString());
            }
            PopupUtils.AttachButtonStringPopover("Level", b, 
                                                 levelList, "Level {0}",
                                                 0, (r1, index, val)=>
                                                 {
                _Level = index+1;
            });
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.goodsButton); 
            b.SetLeftDrawableResource(Resource.Drawable.emerald16);       
            List<String> goodsList = new List<string>(){"None", "Normal", "Double", "Triple"};

            for (int i=4; i<=100; i++)
            {
                goodsList.Add("x" + i.ToString());
            }
            PopupUtils.AttachButtonStringPopover("Goods", b, 
                                                 goodsList, "Goods {0}",
                                                 1, (r1, index, val)=>
                                                 {
                _Goods = index;

            });
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.coinButton);
            b.SetLeftDrawableResource(Resource.Drawable.coins16);
            PopupUtils.AttachButtonStringPopover("Coin", b, 
                                                 goodsList, "Coin {0}",
                                                 1, (r1, index, val)=>
                                                 {
                _Coin = index;

            });
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.magicItemsButton);
            b.SetLeftDrawableResource(Resource.Drawable.wand16);
            PopupUtils.AttachButtonStringPopover("Items", b, 
                                                 goodsList, "Items {0}",
                                                 1, (r1, index, val)=>
                                                 {
                _MagicItems = index;

            });
            
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.generateLevelButton);
            b.SetLeftDrawableResource(Resource.Drawable.treasure16);       
            b.Click += (object sender, EventArgs e) => {GenerateLevel();};


            //item generation
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.countButton);  
            List<String> countList = new List<string>();
            for (int i=1; i<=100; i++)
            {
                countList.Add(i.ToString());
            }
            PopupUtils.AttachButtonStringPopover("Item Count", b, 
                                                 countList, "{0} items(s)",
                                                 0, (r1, index, val)=>
                                                 {
                _ItemCount = index+1;

            });
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.itemsLevelButton);        
            List<String> itemLevelList = new List<string>() {"Minor", "Medium", "Major"};
            PopupUtils.AttachButtonStringPopover("Item Level", b, 
                                                 itemLevelList, "{0} Level",
                                                 0, (r1, index, val)=>
                                                 {
                _ItemLevel = index;

            });
            
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.generateItemsButton);
            b.Click += (object sender, EventArgs e) => 
            {
                GenerateItems();
            };
            b.SetCompoundDrawablesWithIntrinsicBounds(Resources.GetDrawable(Resource.Drawable.treasure16), null, null, null);

            if (_GeneratorVisible)
            {
                ShowGenerator();
            }
            else
            {
                ShowLookup();
            }


        }

        protected override bool SkipShowItem
        {
            get
            {
                return _GeneratorVisible;
            }
        }

        void ShowLookup()
        {
            lookupButton.Selected = true;
            generatorButton.Selected = false;
            SearchReplacementLayout.Visibility = ViewStates.Gone;
            SearchLayout.Visibility = ViewStates.Visible;
            FilterLayout.Visibility = ViewStates.Visible;
            _GeneratorVisible = false;
            RefreshItem();
        }

        void ShowGenerator()
        {
            lookupButton.Selected = false;
            generatorButton.Selected = true;
            SearchReplacementLayout.Visibility = ViewStates.Visible;
            SearchLayout.Visibility = ViewStates.Gone;
            FilterLayout.Visibility = ViewStates.Gone;
            _GeneratorVisible = true;
            ShowLastTreasure();

        }

        void ShowLevel(bool show)
        {
            LinearLayout levelLayout = _GeneratorLayout.FindViewById<LinearLayout>(Resource.Id.levelLayout); 
            LinearLayout itemsLayout = _GeneratorLayout.FindViewById<LinearLayout>(Resource.Id.itemsLayout); 
            levelLayout.Visibility = show?ViewStates.Visible:ViewStates.Gone;
            itemsLayout.Visibility = !show?ViewStates.Visible:ViewStates.Gone;
            _LevelVisible = show;

            Button b;
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.byLevelButton);
            b.Selected = show;
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.byItemsButton);
            b.Selected = !show;


        }

        void GenerateLevel()
        {
            TreasureGenerator gen = new TreasureGenerator() {
                Level = _Level,
                Items =  _MagicItems,
                Goods = _Goods,
                Coin =  _Coin
            };

            _LastTreasure = gen.Generate();

            ShowLastTreasure();

        }

        void GenerateItems()
        {
            TreasureGenerator.RandomItemType types = TreasureGenerator.RandomItemType.None;

            if (GenBoxChecked(Resource.Id.magicalArmorBox))
            {
                types |= TreasureGenerator.RandomItemType.MagicalArmor;
            }
            if (GenBoxChecked(Resource.Id.magicalWeaponBox))
            {
                types |= TreasureGenerator.RandomItemType.MagicalWeapon;
            }
            if (GenBoxChecked(Resource.Id.potionBox))
            {
                types |= TreasureGenerator.RandomItemType.Potion;
            }
            if (GenBoxChecked(Resource.Id.ringBox))
            {
                types |= TreasureGenerator.RandomItemType.Ring;
            }
            if (GenBoxChecked(Resource.Id.rodBox))
            {
                types |= TreasureGenerator.RandomItemType.Rod;
            }
            if (GenBoxChecked(Resource.Id.staffBox))
            {
                types |= TreasureGenerator.RandomItemType.Staff;
            }
            if (GenBoxChecked(Resource.Id.wandBox))
            {
                types |= TreasureGenerator.RandomItemType.Wand;
            }
            if (GenBoxChecked(Resource.Id.wondrousItemBox))
            {
                switch (_ItemLevel)
                {
                case 0:
                    types |= TreasureGenerator.RandomItemType.MinorWondrous;
                    break;
                case 1:
                    types |= TreasureGenerator.RandomItemType.MediumWondrous;
                    break;
                case 2:
                    types |= TreasureGenerator.RandomItemType.MajorWondrous;
                    break;
                }
            }


            TreasureGenerator gen = new TreasureGenerator();
            Treasure t = new Treasure();
            for (int i=0; i<_ItemCount; i++)
            {
                t.Items.Add(gen.GenerateRandomItem((ItemLevel)_ItemLevel, types));
            }
            _LastTreasure = t;

            ShowLastTreasure();

        }

        void ShowLastTreasure()
        {
            WebView wv = _v.FindViewById<WebView>(Resource.Id.itemView);
            if (_LastTreasure != null)
            {
                wv.LoadDataWithBaseURL(null, TreasureHtmlCreator.CreateHtml(_LastTreasure), "text/html", "utf-8", null);
            }
            else
            {
                
                wv.LoadUrl("about:blank");
            }
        }

        bool GenBoxChecked(int id)
        {
                
            CheckBox cb = _GeneratorLayout.FindViewById<CheckBox>(id);
            return cb.Checked;
        }
    }
}
