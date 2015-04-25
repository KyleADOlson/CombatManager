using System;

using CombatManager;
using CoreGraphics;
using System.Collections.Generic;

namespace CombatManagerMono
{
    public class TreasureTab : LookupTab<MagicItem>
    {
        ButtonStringPopover _GroupFilterPopover;
        ButtonStringPopover _CLFilterPopover;

        GradientButton _GroupFilterButton;
        GradientButton _CLFilterButton;

        String _GroupFilterValue = null;
        int ? _CLFilterValue;

        TreasureGeneratorPage _GeneratorPage;


        public TreasureTab (CombatState state) : base (state)
        {


            BuildFilters();
        }


        protected override bool ItemFilter (MagicItem item)
        {
            return GroupFilter(item) && CLFilter(item);
        }

        private bool GroupFilter(MagicItem item)
        {
            return _GroupFilterValue == null || item.Group == _GroupFilterValue;
        }

        private bool CLFilter(MagicItem item)
        {
            return _CLFilterValue == null || item.CL == _CLFilterValue.Value;
        }
        
        protected override bool CompareItems (MagicItem item1, MagicItem item2)
        {
            return item1 == item2;
        }

        protected override string ItemFilterText (MagicItem item)
        {
            return item.Name;
        }

        protected override string SortText (MagicItem item)
        {
            return item.Name;
        }

        protected override string DisplayText (MagicItem item)
        {
            return item.Name;
        }

        protected override string ItemHtml (MagicItem item)
        {
            
            return MagicItemHtmlCreator.CreateHtml(item);
        }


        protected override System.Collections.Generic.IEnumerable<MagicItem> ItemsSource 
        {
            get 
            {
                return MagicItem.Items.Values;
            }
        }

        protected override List<LookupSideTabItem> LoadTabItems ()
        {
            if (_GeneratorPage == null)
            {
                _GeneratorPage = new TreasureGeneratorPage();
                _GeneratorPage.BackgroundColor = CMUIColors.PrimaryColorDark;
                _GeneratorPage.UserInteractionEnabled = true;

            }
            return new List<LookupSideTabItem>() {new LookupSideTabItem(){Name="Generator", Icon=UIExtensions.GetSmallIcon("dice"), View=_GeneratorPage}};
        }
        
        private void BuildFilters()
        {

            float locX = 0;
            float locY = 5;
            float bHeight = 30;
            float marginX = 10;
            
            GradientButton b;
            
            
            //group filter
            b = new GradientButton();
            StyleFilterButton(b);
            b.Frame = new CGRect(locX, locY, 120, bHeight);
            locX += (float)b.Frame.Width + marginX;
            b.SetText(AllGroupText);
            _GroupFilterPopover = new ButtonStringPopover(b);
            _GroupFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllGroupText, Tag=null});
            foreach (string s in MagicItem.Groups)
            {
                _GroupFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = s, Tag=s});
            }
            _GroupFilterPopover.SetButtonText = true;
            _GroupFilterPopover.ItemClicked += HandleTypeFilterItemClicked;;
            _GroupFilterButton = b;
            
            FilterView.AddSubview(b);
            
            //cl filter
            b = new GradientButton();
            StyleFilterButton(b);
            b.Frame = new CGRect(locX, locY, 75, bHeight);
            locX += (float)b.Frame.Width + marginX;
            
            b.SetText(AllCLText);
            _CLFilterPopover = new ButtonStringPopover(b);
            
            _CLFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text =  AllCLText, Tag=null});
            foreach (int x in MagicItem.CLs)
            {           
                _CLFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = "CL " + x.PastTense(), Tag=new int?(x)});
            }
            _CLFilterPopover.SetButtonText = true;
            _CLFilterPopover.ItemClicked += HandleCLFilterItemClicked;
            _CLFilterButton = b;
            
            FilterView.AddSubview(b);



        }

        void HandleCLFilterItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            _CLFilterValue = (int?)e.Tag;
            Filter ();
        }

        void HandleTypeFilterItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            _GroupFilterValue = (string)e.Tag;

            Filter ();
        }

        private string AllGroupText
        {
            get
            {
                return "All Groups";
            }
        }

        private string AllCLText

        {
            get
            {
                return "All CLs";
            }
        }

        protected override bool ShowSideBar
        {
            get
            {
                return true;
            }
        }

        protected override string DefaultTabName
        {
            get
            {
                return "Magic";
            }
        }

        protected override UIKit.UIImage DefaultTabImage
        {
            get
            {
                return UIExtensions.GetSmallIcon("wand");
            }
        }

        protected override void ResetButtonClicked ()
        {

            _CLFilterButton.SetText(AllGroupText);
            _CLFilterValue = null;

            _GroupFilterButton.SetText(AllGroupText);
            _GroupFilterValue = null;

            filterField.Text = "";
            Filter();
        }
    
    }
}

