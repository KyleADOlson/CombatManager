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

