/*
 *  TextBoxDialog.cs
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

using UIKit;
using CoreGraphics;
using System;
using Foundation;

namespace CombatManagerMono
{
	public partial class TextBoxDialog : StandardDialogView
	{
		private string _Value;
		
		
		public bool SingleLine{get; set;}
		
		TextFieldDelegate _textFieldDelegate;
		
		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public TextBoxDialog ()
			: base (UserInterfaceIdiomIsPhone ? "TextBoxDialog_iPhone" : "TextBoxDialog_iPad", null)
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
			
			StyleBackground(BackgroundView);
			
			StyleHeader(HeaderView, HeaderLabel);
			
			if (SingleLine)
			{
				TextField.Text = _Value;
				TextField.Hidden = false;
				TextView.Hidden = true;
				
				CGRect rect = BackgroundView.Frame;
				rect.Height -= 220;
				rect.Y += 100;
				BackgroundView.Frame = rect;
				TextField.BecomeFirstResponder();
				TextField.ReturnKeyType = UIReturnKeyType.Done;
				_textFieldDelegate = new TextFieldDelegate(this);
				TextField.Delegate = _textFieldDelegate;
				TextField.ClearButtonMode = UITextFieldViewMode.Always;
			}
			else
			{
				TextView.Text = _Value;
				TextField.Hidden = true;
				TextView.Hidden = false;
				TextView.BecomeFirstResponder();
			}
			
			StyleButton(OKButton);
			StyleButton(CancelButton);
			
			
		}
		
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			if (UserInterfaceIdiomIsPhone)
			{
				return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
			} else
			{
				return true;
			}
		}
		
		partial void CancelButtonTouchUpInside (Foundation.NSObject sender)
		{
			HandleCancel();
		}
		
		partial void OKButtonTouchUpInside (Foundation.NSObject sender)
		{
			if (SingleLine)
			{
				_Value = TextField.Text;	
			}
			else
			{
				_Value = TextView.Text;
			}
			HandleOK();
		}
		
		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
				
				if (TextView != null)
				{
					if (SingleLine)
					{
						TextField.Text = _Value;
					}
					else
					{
						TextView.Text = _Value;
					}
				}
			}
		}
		
		public class TextFieldDelegate : UITextFieldDelegate
		{
			TextBoxDialog _owner;
			public TextFieldDelegate(TextBoxDialog owner)
			{
				this._owner = owner;
			}
			
			public override bool ShouldReturn (UITextField textField)
			{
				_owner.OKButtonTouchUpInside(textField);
				return true;
			}
			
			
		}
	}
}

