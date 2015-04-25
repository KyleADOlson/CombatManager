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
using Foundation;
using UIKit;
using CoreGraphics;

namespace CombatManagerMono
{
	public class ToolbarView : UIView
	{
		
		public delegate void ToolbarClickEventHandler(object sender, int button);
		
		List<GradientButton> buttons = new List<GradientButton>();
		
		
		public event ToolbarClickEventHandler ButtonClicked;
		
		GradientButton clickedButton = null;

        GradientButton _AboutButton;
        GradientButton _SettingsButton;
		
		public ToolbarView ()
		{
			
			BackgroundColor = new UIColor(1, 0, 0, 0);
			
			List<String> names = new List<String>() {"Combat", "Monsters", "Feats", "Spells", "Rules", "Treasure"};
			List<String> images = new List<String>() {"sword", "monster", "star", "scroll", "book", "treasure"};

			float pos = 0;
			float buttonWidth = 110;
			float buttonGap = -1;
			int i=0;
			foreach (string s in names)
			{
				GradientButton b = new GradientButton();
				b.Frame = (new CGRect(pos, 0, buttonWidth, 50));
				b.SetImage(UIExtensions.GetSmallIcon(images[i]), UIControlState.Normal);

				b.Border = 1;
                b.CornerRadii = new float[] {4, 16, 0, 0};
				
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
            clickedButton.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker, CMUIColors.PrimaryColorMedium);
		
            _SettingsButton = new GradientButton();
            _SettingsButton.SetImage(UIImage.FromFile("Images/settings.png"), UIControlState.Normal);
            //_SettingsButton.Border = 0;
            //_SettingsButton.BackgroundColor = UIColor.Clear;
            //_SettingsButton.Gradient = new GradientHelper(0x00000000.UIColor());
            _SettingsButton.CornerRadius = 0;
            _SettingsButton.TouchUpInside += SettingsButtonClicked;            
            _SettingsButton.Frame = new CGRect(Bounds.Width - 64, (Bounds.Height - 48.0f)/2.0f, 48f, 48f);

            //AddSubview (_SettingsButton);
            	
            _AboutButton = new GradientButton();
            _AboutButton.SetImage(UIImage.FromFile("Images/External/info.png"), UIControlState.Normal);
            // _AboutButton.Border = 0;
            //_AboutButton.BackgroundColor = UIColor.Clear;
            //_AboutButton.Gradient = new GradientHelper(0x00000000.UIColor());
            _AboutButton.CornerRadius = 0;
            _AboutButton.TouchUpInside += AboutButtonClicked;            
            _AboutButton.Frame = new CGRect(Bounds.Width - 23, (Bounds.Height - 48.0f)/2.0f, 48f, 48f);

            Add (_AboutButton);
			BackgroundColor = UIColor.Black;
			
		}


        void SettingsButtonClicked (object sender, EventArgs e)
        {

        }

        void AboutButtonClicked (object sender, EventArgs e)
        {
            AboutView view = new AboutView();
            view.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            AppDelegate.RootController.PresentModalViewController(view, true);
        }
		

		void HandleBTouchUpInside (object sender, EventArgs e)
		{
            
            clickedButton.Gradient = new GradientHelper(CMUIColors.PrimaryColorMedium, CMUIColors.PrimaryColorDarker);
			clickedButton = (GradientButton)sender;
			if (ButtonClicked != null)
			{
                ButtonClicked(this, (int)((UIButton)sender).Tag);
			}
            clickedButton.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker, CMUIColors.PrimaryColorMedium);
		}
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
            float buttonSize = 44f;

            _AboutButton.Frame = new CGRect(Bounds.Width - 54, (Bounds.Height - buttonSize)/2.0f, buttonSize, buttonSize);
            _SettingsButton.Frame = new CGRect(Bounds.Width - 102, (Bounds.Height - buttonSize)/2.0f, buttonSize, buttonSize);


		}
		
	}
}

