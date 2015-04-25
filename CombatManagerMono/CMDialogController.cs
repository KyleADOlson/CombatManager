/*
 *  CMDialogController.cs
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

using Foundation;
using UIKit;

namespace CombatManagerMono
{
	
	[Register ("CMDialogController")]
	public class CMDialogController : UIViewController
	{
		public CMDialogController() : base ()
		{
			Initialize();
		}
		
		public CMDialogController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		public CMDialogController(NSObjectFlag t) : base(t)
		{
			Initialize();
		}
		
		public CMDialogController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		
		
		public CMDialogController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
		
		
		[Outlet]
		public CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		public CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		public CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		public CombatManagerMono.GradientView TitleGradient { get; set; }

		[Action ("CancelButtonClicked:")]
		public void CancelButtonClicked (Foundation.NSObject sender)
		{
			if (HandleCancel())
			{
				View.RemoveFromSuperview();
			}
		}

		[Action ("OKButtonClicked:")]
		public void OKButtonClicked (Foundation.NSObject sender)
		{
			if (HandleOK())
			{
				View.RemoveFromSuperview();
			}
		}
		
		public virtual bool HandleOK()
		{
			return true;	
		}
		
		public virtual bool HandleCancel()
		{
			return true;	
		}
	}
}

