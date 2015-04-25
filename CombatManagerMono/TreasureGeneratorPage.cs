using System;
using System.Collections.Generic;
using CoreGraphics;

using Foundation;
using UIKit;
using System.IO;

using CombatManager;
using MessageUI;

namespace CombatManagerMono
{
    public class TreasureGeneratorPage : UIView
    {
        GradientView _SelectionView;
        UIWebView _WebView;

        GradientButton _EmailButton;

        GradientButton _LevelTab;
        GradientButton _ItemsTab;

        GradientView _LevelView;
        GradientView _ItemsView;

        
        GradientButton _GenerateButton;

        //by level buttons
        GradientButton _LevelButton;
        GradientButton _CoinButton;
        GradientButton _GoodsButton;
        GradientButton _ItemsButton;

        ButtonStringPopover _LevelPopover;
        ButtonStringPopover _CoinPopover;
        ButtonStringPopover _GoodsPopover;
        ButtonStringPopover _ItemsPopover;

        //generate items buttons
        GradientButton _CountButton;
        GradientButton _ChartLevelButton;
        List<GradientButton> _ItemCheckButtons = new List<GradientButton>();
        GradientButton _SelectAllButton;
        GradientButton _UnselectAllButton;

        ButtonStringPopover _CountPopover;
        ButtonStringPopover _ChartLevelPopover;

        GradientButton _RodButton;
        GradientButton _StaffButton;

        bool _ShowingItems;

        string _GeneratedHtml;


        public TreasureGeneratorPage ()
        {
            _SelectionView = new GradientView();
            _WebView = new UIWebView();
            _EmailButton = new GradientButton();
            _EmailButton.SetText("Email");
            _EmailButton.SetImage(UIExtensions.GetSmallIcon("mail"), UIControlState.Normal);
            _EmailButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 15);
            _EmailButton.TouchUpInside += EmailButtonClicked;

            Add (_SelectionView);
            Add (_WebView);
            Add (_EmailButton);



            _LevelTab = new GradientButton();
            _LevelTab.SetText("Level");
            CMStyles.StyleTab(_LevelTab, true);
            _SelectionView.StyleBasicPanel();
            _SelectionView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);
            _SelectionView.CornerRadius = 0f;
            _LevelTab.TouchUpInside += LevelTabClicked;

            _ItemsTab = new GradientButton();
            _ItemsTab.SetText("Items");
            CMStyles.StyleTab(_ItemsTab, false);
            _ItemsTab.TouchUpInside += ItemsTabClicked;

            
            _SelectionView.Add (_LevelTab);
            _SelectionView.Add (_ItemsTab);

            _LevelView = new GradientView();

            _ItemsView = new GradientView();

            foreach (var v in new GradientView[] {_LevelView, _ItemsView})
            {
                v.StyleBasicPanel();
                v.Gradient = new GradientHelper(CMUIColors.SecondaryColorADarker);
            }

            _ItemsView.Hidden = true;

            _SelectionView.Add (_LevelView);
            _SelectionView.Add (_ItemsView);

            _GenerateButton = new GradientButton();
            _GenerateButton.SetText("Generate");
            _GenerateButton.SetSmallIcon("treasure");
            _SelectionView.Add (_GenerateButton);
            _GenerateButton.TouchUpInside += GenerateButtonClicked;


            //create level items
            _LevelButton = new GradientButton();
            _LevelButton.SetText("Level 1");
            _LevelButton.Tag = 1;
            _LevelView.Add (_LevelButton);
            _LevelPopover = new ButtonStringPopover(_LevelButton);
            _LevelPopover.SetButtonText = true;
            _LevelPopover.ItemClicked += HandleLevelPopoverItemClicked;

            for (int i=1; i<=20; i++)
            {
                _LevelPopover.Items.Add (new ButtonStringPopoverItem() {Text = "Level " + i, Tag = i});
            }

            _CoinButton = new GradientButton();
            _CoinButton.SetText("Normal Coins");
            _LevelView.Add (_CoinButton);
            _CoinPopover = new ButtonStringPopover(_CoinButton);
            AddLevelPopoverItems(_CoinPopover, "Coins");
            _CoinButton.Tag = 1;

            _GoodsButton = new GradientButton();
            _GoodsButton.SetText("Normal Goods");
            _LevelView.Add (_GoodsButton);
            _GoodsPopover = new ButtonStringPopover(_GoodsButton);
            AddLevelPopoverItems(_GoodsPopover, "Goods");
            _GoodsButton.Tag = 1;

            _ItemsButton = new GradientButton();
            _ItemsButton.SetText("Normal Items");
            _ItemsPopover = new ButtonStringPopover(_ItemsButton);
            _LevelView.Add (_ItemsButton);
            _ItemsButton.Tag = 1;
            AddLevelPopoverItems(_ItemsPopover, "Items");

            //create item items
            _CountButton = new GradientButton();
            _CountButton.SetText("1 Item");
            _CountButton.Tag = 1;

            _ItemsView.Add (_CountButton);
            _CountPopover = new ButtonStringPopover(_CountButton);
            for (int i=1; i<61; i++)
            {
                string text = i + (i==1?" Item":" Items");
                _CountPopover.Items.Add (new ButtonStringPopoverItem() {Text = text, Tag=i});
            }
            _CountPopover.ItemClicked += HandleLevelPopoverItemClicked;
            _CountPopover.SetButtonText = true;

            _ChartLevelButton = new GradientButton();
            _ChartLevelButton.SetText ("Minor");
            _ChartLevelButton.Tag = 0;
            _ItemsView.Add(_ChartLevelButton);
            _ChartLevelPopover= new ButtonStringPopover(_ChartLevelButton);
            _ChartLevelPopover.SetButtonText = true;
            _ChartLevelPopover.ItemClicked += HandleItemLevelClicked;;
            _ChartLevelPopover.Items.Add(new ButtonStringPopoverItem() {Text = "Minor", Tag = 0});
            _ChartLevelPopover.Items.Add(new ButtonStringPopoverItem() {Text = "Medium", Tag = 1});
            _ChartLevelPopover.Items.Add(new ButtonStringPopoverItem() {Text = "Major", Tag = 2});

            foreach (var t in new TreasureGenerator.RandomItemType []{
                TreasureGenerator.RandomItemType.MagicalArmor,
                TreasureGenerator.RandomItemType.MagicalWeapon,
                TreasureGenerator.RandomItemType.Potion,
                TreasureGenerator.RandomItemType.Wand,
                TreasureGenerator.RandomItemType.Ring,
                TreasureGenerator.RandomItemType.Rod,
                TreasureGenerator.RandomItemType.Scroll,
                TreasureGenerator.RandomItemType.Staff,
                TreasureGenerator.RandomItemType.MinorWondrous,})
            {
                var b = new GradientButton();
                b.SetText (TreasureGenerator.RandomItemString(t));
                b.Tag = (int)t;
                b.MakeCheckButtonStyle(true);
                b.Data = true;
                b.TouchUpInside += CheckButtonChecked;
                _ItemsView.Add(b);
                _ItemCheckButtons.Add (b);

                if (t == TreasureGenerator.RandomItemType.Rod)
                {
                    _RodButton = b;
                    _RodButton.Enabled = false;
                }
                else if (t == TreasureGenerator.RandomItemType.Staff)
                {
                    _StaffButton = b;
                    _StaffButton.Enabled = false;
                }
            }

            _SelectAllButton = new GradientButton();
            _SelectAllButton.SetText("Select All");
            _ItemsView.Add(_SelectAllButton);
            _SelectAllButton.TouchUpInside += SelectButtonClicked;
            _SelectAllButton.Data = true;

            _UnselectAllButton = new GradientButton();
            _UnselectAllButton.SetText("Unselect All");
            _ItemsView.Add (_UnselectAllButton);
            _UnselectAllButton.TouchUpInside += SelectButtonClicked;
            _UnselectAllButton.Data = false;

        }

        void EmailButtonClicked (object sender, EventArgs e)
        {
            if (_GeneratedHtml != null)
            {
                MFMailComposeViewController mv = new MFMailComposeViewController();
                mv.MailComposeDelegate = new MailDelegate(this, mv);
                mv.SetMessageBody(_GeneratedHtml, true);
                mv.SetSubject("Treasure");


                AppDelegate.RootController.PresentModalViewController(mv, true);

            }

        }

        private class MailDelegate : MFMailComposeViewControllerDelegate
        {
            private TreasureGeneratorPage _Owner;
            private MFMailComposeViewController _MV;

            public MailDelegate(TreasureGeneratorPage owner, MFMailComposeViewController mv)
            {
                _MV = mv;
                _Owner = owner;
            }

            public override void Finished (MFMailComposeViewController controller, MFMailComposeResult result, NSError error)
            {
                _MV.DismissModalViewController(true);
            }
        }

        void SelectButtonClicked (object sender, EventArgs e)
        {
            bool state = (bool)((GradientButton)sender).Data;

            foreach (var v in _ItemCheckButtons)
            {
                v.Data = state;
                v.MakeCheckButtonStyle(state);
            }
        }

        void HandleItemLevelClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            
            ((ButtonStringPopover)sender).Button.Tag = (int)e.Tag;
            ItemLevel level = (ItemLevel)e.Tag;
            _RodButton.Enabled = level != ItemLevel.Minor;
            _StaffButton.Enabled = level != ItemLevel.Minor;

        }

        void ItemsTabClicked (object sender, EventArgs e)
        {
            ShowTab(true);
        }

        void LevelTabClicked (object sender, EventArgs e)
        {            
            ShowTab(false);
        }

        void ShowTab(bool itemsTab)
        {
            _ShowingItems = itemsTab;
            _LevelView.Hidden = itemsTab;
            _ItemsView.Hidden = !itemsTab;
            _LevelTab.StyleTab(!itemsTab);
            _ItemsTab.StyleTab(itemsTab);
        }

        void CheckButtonChecked (object sender, EventArgs e)
        {
            var b = (GradientButton)sender;
            b.Data = !((bool)b.Data);
            b.MakeCheckButtonStyle((bool)b.Data);
        }

        void GenerateButtonClicked (object sender, EventArgs e)
        {
            Treasure t;
            if (_ShowingItems)
            {
                
                TreasureGenerator gen = new TreasureGenerator();

                t = new Treasure();
                int count = (int)_CountButton.Tag;
                ItemLevel level = (ItemLevel)(int)_ChartLevelButton.Tag;

                TreasureGenerator.RandomItemType types = (TreasureGenerator.RandomItemType)0;

                foreach (var b in _ItemCheckButtons)
                {
                    if ((bool)b.Data)
                    {
                        var v = (TreasureGenerator.RandomItemType)(int)b.Tag;

                        switch (v )
                        {
                        case TreasureGenerator.RandomItemType.MinorWondrous:

                            switch (level)
                            {
                            case ItemLevel.Medium:
                                v = TreasureGenerator.RandomItemType.MediumWondrous;
                                break;
                            case ItemLevel.Major:
                                v = TreasureGenerator.RandomItemType.MinorWondrous;
                                break;
                            }
                            break;
                        case TreasureGenerator.RandomItemType.Rod:
                        case TreasureGenerator.RandomItemType.Staff:
                            if (level == ItemLevel.Minor)
                            {
                                v = (TreasureGenerator.RandomItemType)0;
                            }
                            break;
                              
                        }
                        types |= v;
                    }
                }


                if (types != 0)
                {

                    for(int i=0; i<count; i++)
                    {
                        TreasureItem item = gen.GenerateRandomItem(level, types);
                        t.Items.Add(item);
                    }
                    
                }
            }
            else
            {
                TreasureGenerator gen = new TreasureGenerator();
                gen.Coin = (int)_CoinButton.Tag;
                gen.Level = (int)_LevelButton.Tag;
                gen.Items = (int)_ItemsButton.Tag;
                gen.Goods = (int)_GoodsButton.Tag;

                t = gen.Generate();
            }

            _GeneratedHtml = TreasureHtmlCreator.CreateHtml(t);

            _WebView.LoadHtmlString(_GeneratedHtml, new NSUrl("http://localhost"));
        }

        public void AddLevelPopoverItems(ButtonStringPopover v, string text)
        {
            v.SetButtonText = true;            
            v.Items.Add (new ButtonStringPopoverItem() {Text = "No " + text, Tag = 0});            
            v.Items.Add (new ButtonStringPopoverItem() {Text = "Normal " + text, Tag = 1});
            for (int i=2; i<=20; i++)
            {
                v.Items.Add (new ButtonStringPopoverItem() {Text = text + " x" + i, Tag = i});
            }
            v.ItemClicked += HandleLevelPopoverItemClicked;
        }

        void HandleLevelPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            ((ButtonStringPopover)sender).Button.Tag = (int)e.Tag;
        }

        float _SelectionWidth = 275;
        float _ButtonHeight = 40f;
        float _ButtonMargin = 5f;

        public override void LayoutSubviews ()
        {
            base.LayoutSubviews ();

            _SelectionView.SetLocation(0, 0);
            _SelectionView.SetSize((float)_SelectionWidth, (float)Bounds.Height);

            _WebView.SetLocation((float)_SelectionView.Frame.Right, (float)_ButtonHeight + _ButtonMargin * 2.0f);
            _WebView.SetSize((float)Bounds.Width - (float)_SelectionView.Frame.Right, (float)Bounds.Height - _ButtonMargin * 2.0f);

            _EmailButton.SetSize(100f, _ButtonHeight);
            _EmailButton.SetLocation((float)Bounds.Width - (float)_EmailButton.Bounds.Width - _ButtonMargin, _ButtonMargin);



            //size selection view items
            _LevelTab.SetLocation(14f, 10f);
            _LevelTab.SetWidth(((float)_SelectionView.Bounds.Width - 28f)/2.0f);
            _LevelTab.SetHeight(40f);

            _ItemsTab.SetLocation((float)_LevelTab.Frame.Right, 10f);
            _ItemsTab.SetSize(_LevelTab.Bounds.Size);


            _GenerateButton.SetSize((float)_SelectionView.Bounds.Width - 10f, 40f);
            _GenerateButton.SetLocation(5f, (float)_SelectionView.Bounds.Height - 50f);
            _GenerateButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 10);

            _ItemsView.SetLocation(5f, (float)_ItemsTab.Frame.Bottom);
            _ItemsView.SetSize((float)_SelectionView.Bounds.Width - 10.0f, (float)_GenerateButton.Frame.Top - (float)_ItemsView.Frame.Top - 5.0f);

            _LevelView.Frame = _ItemsView.Frame;

            //size level view items

            CGRect levelRect = _LevelView.Bounds.Size.OriginRect();

            CGRect buttonRect = levelRect;
            buttonRect.Inflate(-_ButtonMargin, 0);
            buttonRect.Y = _ButtonMargin;
            buttonRect.Height = _ButtonHeight;
           
            foreach (var v in new GradientButton[] {_LevelButton, _CoinButton, _GoodsButton, _ItemsButton})
            {

                v.Frame = buttonRect;

                buttonRect.Y += buttonRect.Height + _ButtonMargin;
            }


            //size item view items
            buttonRect = _ItemsView.Bounds.Size.OriginRect();
                        buttonRect.Inflate(-_ButtonMargin, 0);
            buttonRect.Y = _ButtonMargin;
            buttonRect.Height = _ButtonHeight;


            foreach (var v in new GradientButton[] {_CountButton, _ChartLevelButton})
            {
                
                v.Frame = buttonRect;

                buttonRect.Y += buttonRect.Height + _ButtonMargin;
            }

            foreach (var v in _ItemCheckButtons)
            {                
                v.Frame = buttonRect;

                buttonRect.Y += buttonRect.Height + _ButtonMargin;
            }

            buttonRect.Height = buttonRect.Height *2.0f;
            buttonRect.Width = buttonRect.Width/2.0f - _ButtonMargin;

            _SelectAllButton.Frame = buttonRect;
            buttonRect.X += buttonRect.Width + _ButtonMargin;
            _UnselectAllButton.Frame = buttonRect;

            

        }
    }
}

