
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using System.Threading;
using CombatManager;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;

namespace CombatManagerMono
{
    public partial class CMViewController : UIViewController
    {

        bool uiLoaded;
        MainUI ui;

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public CMViewController ()
			: base (UserInterfaceIdiomIsPhone ? "CMViewController_iPhone" : "CMViewController_iPad", null)
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
			

            
            //view.AddSubview(backview);
            
            
            //this.CenterImage.Image = UIImage.FromFile("Images/icon.png");
            //Spin();
            
            Thread t = new Thread(LoadStuff);
            t.Start();
            
        }
        

        
                     
        public void LoadStuff()
        {
            int count = Monster.Monsters.Count;
            MainUI.LoadCombatState();
            
            if (count > 0)
            {
                InvokeOnMainThread(LoadUI);
            }
        }
        
        public void LoadUI()
        {
            uiLoaded = true;
            ui = new MainUI();
            RectangleF rect = View.Frame;
            rect = View.ConvertRectFromView(rect, View.Superview);
            rect.X = 0;
            rect.Y = 0;
            ui.Frame = rect;
            View.AddSubview(ui);
            LayoutView();
        }

        public void LayoutView()
        {
            
            RectangleF rect = View.Frame;
            rect = View.ConvertRectFromView(rect, View.Superview);
            
            
            PointF pt = new PointF(rect.Width/2.0f, rect.Height/2.0f);
            //CenterImage.Center = pt;
            if (ui != null)
            {
                rect.X = 0;
                rect.Y = 0;
                ui.Frame = rect;
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
                return toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || 
                    toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            }
        }
    }
}

