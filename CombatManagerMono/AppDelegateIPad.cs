
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CombatManagerMono
{

	// The name AppDelegateIPad is referenced in the MainWindowIPad.xib file.
	[Register("AppDelegateIPad")]
	public partial class AppDelegateIPad : UIApplicationDelegate
	{
		//MainUI main =new MainUI();
		UIWindow window;
		UIWindow externalWindow;
		UIViewController viewController;
		UIViewController externalController;
		
		public static AppDelegateIPad Delegate {get; set;}
		
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new MainViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			Delegate = this;
			
			NSNotificationCenter.DefaultCenter.AddObserver(UIScreen.DidConnectNotification, ScreenDidConnect);
			
			NSNotificationCenter.DefaultCenter.AddObserver(UIScreen.DidDisconnectNotification, ScreenDidDisconnect);
			
			
			if (UIScreen.Screens.Count() > 1)
			{
				ShowWindowOnExternalScreen(UIScreen.Screens[1]);
			}
			return true;
		}
		
		public void ScreenDidConnect(NSNotification notification)
		{
			UIScreen newScreen = (UIScreen)notification.Object;
			
		}
		
		
		public void ScreenDidDisconnect(NSNotification notification)
		{
			externalWindow.Hidden = true;
			externalWindow = null;
		}
		
		private void ShowWindowOnExternalScreen(UIScreen newScreen)
		{
			if (externalWindow == null)
			{
				externalWindow = new UIWindow(newScreen.Bounds);
				externalController = new UIViewController();
				
				
			}
		}
	

		
	}
}

