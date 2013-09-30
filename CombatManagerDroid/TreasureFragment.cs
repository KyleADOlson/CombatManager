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

        Treasure _LastTreasure;

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

        protected override void BuildAdditionalLayouts()
        {
            ImageButton ib = new ImageButton(_v.Context);
            ib.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.wand16));
            LeftLayout.AddView(ib);
            ib.Click += (object sender, EventArgs e) => {ShowLookup();};

            ib = new ImageButton(_v.Context);
            ib.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.dice16));
            LeftLayout.AddView(ib);
            ib.Click += (object sender, EventArgs e) => {ShowGenerator();};

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
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.byItemsButton);
            b.Click += (object sender, EventArgs e) => 
            {
                ShowLevel(false);
            };
            ShowLevel(true);

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
            PopupUtils.AttachButtonStringPopover("Coin", b, 
                                                 goodsList, "Coin {0}",
                                                 1, (r1, index, val)=>
                                                 {
                _Coin = index;

            });
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.magicItemsButton);
            PopupUtils.AttachButtonStringPopover("Items", b, 
                                                 goodsList, "Items {0}",
                                                 1, (r1, index, val)=>
                                                 {
                _MagicItems = index;

            });
            
            b = _GeneratorLayout.FindViewById<Button>(Resource.Id.generateLevelButton);
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

        }

        void ShowLookup()
        {
            SearchReplacementLayout.Visibility = ViewStates.Gone;
            SearchLayout.Visibility = ViewStates.Visible;
            RefreshItem();
        }

        void ShowGenerator()
        {
            SearchReplacementLayout.Visibility = ViewStates.Visible;
            SearchLayout.Visibility = ViewStates.Gone;
            ShowLastTreasure();

        }

        void ShowLevel(bool show)
        {
            LinearLayout levelLayout = _GeneratorLayout.FindViewById<LinearLayout>(Resource.Id.levelLayout); 
            LinearLayout itemsLayout = _GeneratorLayout.FindViewById<LinearLayout>(Resource.Id.itemsLayout); 
            levelLayout.Visibility = show?ViewStates.Visible:ViewStates.Gone;
            itemsLayout.Visibility = !show?ViewStates.Visible:ViewStates.Gone;
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
            wv.LoadUrl("about:blank");
            if (_LastTreasure != null)
            {
                wv.LoadData(TreasureHtmlCreator.CreateHtml(_LastTreasure), "text/html", null);
            }
        }

        bool GenBoxChecked(int id)
        {
                
            CheckBox cb = _GeneratorLayout.FindViewById<CheckBox>(id);
            return cb.Checked;
        }
    }
}
