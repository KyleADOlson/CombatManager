/*
 *  MainViewController.xib.cs
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
using System.Drawing;
using System.Threading;
using CombatManager;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;

namespace CombatManagerMono
{
	public partial class MainViewController : UIViewController, ILogTarget
	{
		#region Constructors
		
		UIView ui;
		
		bool uiLoaded;
		float spin;
		
		public static UIWindow MainWindow {get; set;}
		public static MainViewController MainController {get; set;}

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public MainViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
			
			
			View.BackgroundColor = UIExtensions.RGBColor(0);
		}

		[Export("initWithCoder:")]
		public MainViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MainViewController () : base("MainViewController", null)
		{
			Initialize ();
			
		}

		void Initialize ()
		{
			MainController = this;
			DebugLogger.LogTarget = this;
			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			MainWindow = window;

			
			//view.AddSubview(backview);
			
			
			//this.CenterImage.Image = UIImage.FromFile("Images/icon.png");
			//Spin();
			
			Thread t = new Thread(LoadStuff);
			t.Start();
			
		}
		
		[Export("Spin")]
		private void Spin()
		{
			if (!uiLoaded)
			{
				spin += (float)Math.PI / 4.0f;
				
				UIView.BeginAnimations("SpinAnimation");
				UIView.SetAnimationDuration(.2);
				//CenterImage.Transform = CGAffineTransform.MakeRotation(spin);
				UIView.SetAnimationDelegate(this);
				UIView.SetAnimationDidStopSelector(new Selector("Spin"));
				UIView.CommitAnimations();
			}
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
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || 
				toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
		}
		
		public override void DidChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey)
		{
			base.DidChange (changeKind, indexes, forKey);
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			
			LayoutView();
			
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
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
		
		public void WriteLine(string text)
		{
			InvokeOnMainThread( delegate {
			if (MainController.LoadingView != null)
			{
				MainController.LoadingView.Text += "\r\n" + text ;
			}
			});

		}
		
		#endregion
		
		
		
	}
}

