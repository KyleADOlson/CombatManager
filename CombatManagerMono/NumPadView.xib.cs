/*
 *  NumPadView.xib.cs
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

namespace CombatManagerMono
{
	
	public class NumPadEventArgs
	{
		public int Key {get; set; }
	}
	
	public delegate void NumPadEventHandler(object sender, NumPadEventArgs e);
	
	public partial class NumPadView : UIViewController
	{
		public event NumPadEventHandler NumberPressed;
		public event EventHandler EnterPressed;
		public event EventHandler DeletePressed;
		
		public NumPadView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public NumPadView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public NumPadView () : base("NumPadView", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		partial void numberPressed (Foundation.NSObject sender)
		{
			if (NumberPressed != null)
			{
                NumberPressed(this, new NumPadEventArgs() {Key = (int)((UIButton)sender).Tag});
			}
		}

		partial void delPressed (Foundation.NSObject sender)
		{
			if (DeletePressed != null)
			{
				DeletePressed(this, new EventArgs());
			}
		}

		partial void enterPressed (Foundation.NSObject sender)
		{
			if (EnterPressed != null)
			{
				EnterPressed(this, new EventArgs());
			}
		}
		
	}
}

