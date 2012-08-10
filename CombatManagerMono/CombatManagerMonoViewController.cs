using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using CombatManager;

namespace CombatManagerMono
{
    public partial class CombatManagerMonoViewController : UIViewController, ILogTarget
    {
        bool uiLoaded;
        MainUI ui;

        List<UIImageView> _LoadingIcons = new List<UIImageView>();

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public CombatManagerMonoViewController ()
			: base (UserInterfaceIdiomIsPhone ? "CombatManagerMonoViewController_iPhone" : "CombatManagerMonoViewController_iPad", null)
        {
            DebugLogger.LogTarget = this;
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

        public void LayoutView()
        {
            
            RectangleF rect = View.Frame;
            rect = View.ConvertRectFromView(rect, View.Superview);
            

            if (ui != null)
            {
                rect.X = 0;
                rect.Y = 0;
                ui.Frame = rect;
            }
        }
        
        public void WriteLine(string text)
        {
            InvokeOnMainThread( delegate {
            if (!uiLoaded)
            {
                UIImage im = UIImage.FromFile("Images/Icon.png");
                float y = (View.Bounds.Height - im.Size.Height)/2.0f;
                float x = View.Bounds.Width/5.0f;

                if (_LoadingIcons.Count == 0)
                {
                    UIImageView die = new UIImageView(im);
                    die.Frame = new RectangleF(new PointF(x, y), im.Size);
                    
                    View.Add (die);
                    _LoadingIcons.Add(die);
                }
                else
                {
                    RectangleF last = _LoadingIcons[_LoadingIcons.Count - 1].Frame;

                    UIImageView die = new UIImageView(im);
                    die.Frame = new RectangleF(new PointF(last.X + im.Size.Width, last.Top), im.Size);
                    
                    View.Add (die);
                    _LoadingIcons.Add(die);
                }
            }
            });
            /*InvokeOnMainThread( delegate {
            if (MainController.LoadingView != null)
            {
                MainController.LoadingView.Text += "\r\n" + text ;
            }
            });*/

        }
    }
}

