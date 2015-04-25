
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using CombatManager;

namespace CombatManagerMono
{
    public partial class NaturalAttackItemView : UIViewController
    {
        WeaponItem _WeaponItem;

        public EventHandler<EventArgs> DeleteClicked;
        public EventHandler<EventArgs> ItemChanged;

        NaturalAttackDialog _NADialog;
        Monster _Monster;

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public NaturalAttackItemView (WeaponItem item, Monster monster)
			: base (UserInterfaceIdiomIsPhone ? "NaturalAttackItemView_iPhone" : "NaturalAttackItemView_iPad", null)
        {
            _WeaponItem = item;
            _Monster = monster;
        }
		
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
			
        }
		
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();


            BackgroundView.StyleBasicPanel();

            TitleButton.SetText(ItemText);

            DeleteButton.SetImage(UIImage.FromFile ("Images/External/redx.png"), UIControlState.Normal);

            TitleButton.TouchUpInside += HandleTitleButtonClicked;
            DeleteButton.TouchUpInside += HandleDeleteButtonClicked;

            TitleButton.SetTitleColor(0xFFFFFFFF.UIColor(), UIControlState.Normal);
            TitleButton.AlignButtonLeft();
            DeleteButton.SetTitleColor(0xFFFFFFFF.UIColor(), UIControlState.Normal);


        }

        string ItemText
        {
            get
            {
                return _WeaponItem.FullName;
            }
        }

        public WeaponItem Item
        {
            get
            {
                return _WeaponItem;
            }
        }

        void HandleDeleteButtonClicked (object sender, EventArgs e)
        {
            if (DeleteClicked != null)
            {
                DeleteClicked(this, new EventArgs());
            }
        }

        void HandleTitleButtonClicked (object sender, EventArgs e)
        {
            _NADialog = new NaturalAttackDialog(_WeaponItem, _Monster);
            _NADialog.OKClicked += HandleAttackDialogOKClicked;
            MainUI.MainView.Add(_NADialog.View);

        }

        void HandleAttackDialogOKClicked (object sender, EventArgs e)
        {
            if (ItemChanged != null)
            {
                ItemChanged(this, new EventArgs());
            }
        }
		
        public override void ViewDidUnload ()
        {
            base.ViewDidUnload ();
			
            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;
			
            ReleaseDesignerOutlets ();
        }
		
        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            if (UserInterfaceIdiomIsPhone)
            {
                return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
            } else
            {
                return true;
            }
        }
    }
}

