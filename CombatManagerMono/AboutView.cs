
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using System.IO;

namespace CombatManagerMono
{
    public partial class AboutView : UIViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public AboutView ()
			: base (UserInterfaceIdiomIsPhone ? "AboutView_iPhone" : "AboutView_iPad", null)
        {
        }
		
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
			
            // Release any cached data, images, etc that aren't in use.
        }
		
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			
            OKButton.TouchUpInside += HandleOKClicked;
            using (StreamReader r = new StreamReader("about.html"))
            {
                WebView.LoadHtmlString(r.ReadToEnd(), new NSUrl("http://localhost"));
            }
        }

        void HandleOKClicked (object sender, EventArgs e)
        {
            DismissViewController(true, null);
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

