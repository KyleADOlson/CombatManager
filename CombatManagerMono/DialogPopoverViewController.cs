/*
 *  DialogPopoverViewController.cs
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
using Foundation;
using UIKit;
using CoreGraphics;

namespace CombatManagerMono
{
	[Register ("DialogPopoverViewController")]
	public class DialogPopoverViewController : UIViewController
	{
		UIPopoverController _PopoverController;
		
		public DialogPopoverViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public DialogPopoverViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public DialogPopoverViewController (string name, NSBundle bundle) : base(name, bundle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			_PopoverController = new UIPopoverController(this);
		}
		
		public void ShowOnView(UIView view)
		{
			ParentView = view;
			Show();
		}
		
		public void Show()
		{
			_PopoverController.SetPopoverContentSize(PresentationSize, true);
			_PopoverController.PresentFromRect(ParentView.Frame, ParentView.Superview, UIPopoverArrowDirection.Any, true);
		}
		
		
		[Foundation.Export("Close:")]
		public void Close (UIKit.UIButton sender)
		{
			_PopoverController.Dismiss(true);	
		}
		
		public virtual CGSize PresentationSize
		{
			get
			{
				return View.Frame.Size;
			}
		}
		
		public object Data {get; set;}
		public UIView ParentView {get; set;}
		
		
	}
}

