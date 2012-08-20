/*
 *  AppDelegateIPad.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */


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
			/*window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new MainViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			*/
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

