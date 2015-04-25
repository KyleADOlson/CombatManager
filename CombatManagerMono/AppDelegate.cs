using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CombatManager;

namespace CombatManagerMono
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;
        static CombatManagerMonoViewController viewController;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            DBSettings.UseDB = true;

            NSUrl url = null;
            if (options != null)
            {
                url = (NSUrl)options.ValueForKey(UIApplication.LaunchOptionsUrlKey);
            }

            window = new UIWindow (UIScreen.MainScreen.Bounds);
			
            viewController = new CombatManagerMonoViewController ();
            viewController.StartupUrl = url;
            window.RootViewController = viewController;
            window.MakeKeyAndVisible ();
			

            return true;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            if (url != null)
            {
                viewController.LoadUrl(url);
                return true;
            }
            return false;
        }
        public static CombatManagerMonoViewController RootController
        {
            get
            {
                return viewController;
            }
        }
    }
}

