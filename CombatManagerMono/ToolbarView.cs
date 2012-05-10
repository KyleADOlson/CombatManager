/*
 *  ToolbarView.cs
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

namespace CombatManagerMono
{
	public class ToolbarView : UIView
	{
		
		public delegate void ToolbarClickEventHandler(object sender, int button);
		
		List<GradientButton> buttons = new List<GradientButton>();
		
		
		public event ToolbarClickEventHandler ButtonClicked;
		
		GradientButton clickedButton = null;
		
		public ToolbarView ()
		{
			
			BackgroundColor = new UIColor(1, 0, 0, 0);
			
			List<String> names = new List<String>() {"Combat", "Monsters", "Feats", "Spells", "Rules"};
			List<String> images = new List<String>() {"sword", "monster", "star", "scroll", "book"};

			float pos = 0;
			float buttonWidth = 110;
			float buttonGap = -1;
			int i=0;
			foreach (string s in names)
			{
				GradientButton b = new GradientButton();
				b.Frame = (new RectangleF(pos, 0, buttonWidth, 40));
				b.SetImage(UIExtensions.GetSmallIcon(images[i]), UIControlState.Normal);
				b.Border = 1;
				b.CornerRadius = 0;
				
				b.SetTitle(s, UIControlState.Normal);
				UIEdgeInsets si = b.ImageEdgeInsets;
				si.Right += 10;
				b.ImageEdgeInsets = si;
				
				pos += buttonWidth + buttonGap;
				buttons.Add(b);
				b.Tag = i;
				b.TouchUpInside += HandleBTouchUpInside;
				i++;
				
				
				AddSubview(b);
			}
			
			clickedButton = buttons[0];
			UIColor c = clickedButton.Color2;
			clickedButton.Color2 = clickedButton.Color1;
			clickedButton.Color1 = c;
			
			BackgroundColor = CMUIColors.SecondaryColorBDark;
			
		}
		

		void HandleBTouchUpInside (object sender, EventArgs e)
		{
			UIColor c = clickedButton.Color2;
			clickedButton.Color2 = clickedButton.Color1;
			clickedButton.Color1 = c;
			clickedButton = (GradientButton)sender;
			if (ButtonClicked != null)
			{
				ButtonClicked(this, ((UIButton)sender).Tag);
			}
			c = clickedButton.Color2;
			clickedButton.Color2 = clickedButton.Color1;
			clickedButton.Color1 = c;
		}
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
		}
		
	}
}

