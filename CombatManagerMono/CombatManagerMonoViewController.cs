using System;
using CoreGraphics;
using System.Threading;
using System.Collections.Generic;

using Foundation;
using UIKit;

using CombatManager;
using CombatManager.LocalService;

namespace CombatManagerMono
{
    public partial class CombatManagerMonoViewController : UIViewController, ILogTarget
    {
        bool uiLoaded;
        MainUI ui;

        List<UIImageView> _LoadingIcons = new List<UIImageView>();

        UIImageView _BackgroundView;

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

            _BackgroundView = new UIImageView();

            _BackgroundView.Frame = View.Bounds;
            View.Add(_BackgroundView);
            _BackgroundView.Image = UIImage.FromFile("Images/DiePencil.jpg");

			
            Thread t = new Thread(LoadStuff);
            t.Start();
            
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            _BackgroundView.Frame = View.Bounds;

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
            ui.StartupUrl = StartupUrl;
            CGRect rect = View.Frame;
            rect = View.ConvertRectFromView(rect, View.Superview);
            rect.X = 0;
            rect.Y = 0;
            ui.Frame = rect;
            View.AddSubview(ui);
            LayoutView();

            if (MobileSettings.Instance.RunLocalService)
            {
                RunLocalService();
            }

            MobileSettings.Instance.PropertyChanged += MobileSettingsPropertyChanged;
        }

        LocalCombatManagerService localService;

        private void MobileSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RunLocalService":
                    if (MobileSettings.Instance.RunLocalService)
                    {
                        RunLocalService();
                    }
                    else
                    {
                        StopLocalService();

                    }

                    break;
                case "LocalServicePort":
                    if (MobileSettings.Instance.RunLocalService)
                    {
                        StopLocalService();
                        RunLocalService();
                    }

                    break;
                case "LocalServicePasscode":
                    if (localService != null)
                    {
                        localService.Passcode = MobileSettings.Instance.LocalServicePasscode;
                    }

                    break;
            }
        }

        void RunLocalService()
        {
            if (localService == null)
            {
                localService = new LocalCombatManagerService(MainUI.CombatState, (ushort)MobileSettings.Instance.LocalServicePort, MobileSettings.Instance.LocalServicePasscode);
                localService.HPMode = Character.HPMode.Default;
                localService.StateActionCallback = act =>
                {
                    InvokeOnMainThread(act);

                };
                localService.UIActionTaken += (sender, e) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        switch (e.Action)
                        {
                            case LocalCombatManagerService.UIAction.BringToFront:
                                break;
                            case LocalCombatManagerService.UIAction.Minimize:

                            case LocalCombatManagerService.UIAction.Goto:
                                UIGoto((String)e.Data);
                                break;

                            case LocalCombatManagerService.UIAction.ShowCombatListWindow:
                                break;
                            case LocalCombatManagerService.UIAction.HideCombatListWindow:
                                break;

                        }
                    }
                    );
                };
                localService.Start();
            }

        }

        void StopLocalService()
        {
            if (localService != null)
            {
                localService.Close();
                localService = null;
            }
        }

        void UIGoto(string target)
        {
            switch (target.ToLower())
            {
                case "combat":
                    ui.SwitchTab(MainUI.UITab.Combat);
                    break;
                case "feats":
                    ui.SwitchTab(MainUI.UITab.Feats);
                    break;
                case "spells":
                    ui.SwitchTab(MainUI.UITab.Spells);
                    break;
                case "monsters":
                    ui.SwitchTab(MainUI.UITab.Monsters);
                    break;
                case "rules":
                    ui.SwitchTab(MainUI.UITab.Rules);
                    break;
                case "treasure":
                    ui.SwitchTab(MainUI.UITab.Treasure);
                    break;
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

        public void LayoutView()
        {
            
            CGRect rect = View.Frame;
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
                double y = (View.Bounds.Height - im.Size.Height)/2.0d;
                    double x = View.Bounds.Width/5.0d;


                if (_LoadingIcons.Count == 0)
                {
                    UIImageView die = new UIImageView(im);
                        die.Frame = new CGRect(new CGPoint(new nfloat(x), new nfloat(y)), im.Size);
                    
                    View.Add (die);
                    _LoadingIcons.Add(die);
                }
                else
                {
                    CGRect last = _LoadingIcons[_LoadingIcons.Count - 1].Frame;

                    UIImageView die = new UIImageView(im);
                    die.Frame = new CGRect(new CGPoint(last.X + im.Size.Width, last.Top), im.Size);
                    
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

        public NSUrl StartupUrl { get; set; }

        public void LoadUrl(NSUrl url) 
        {
            ui.LoadUrl(url);
        }

    }
}

