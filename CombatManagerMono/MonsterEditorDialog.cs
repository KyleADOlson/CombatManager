/*
 *  MonsterEditorDialog.cs
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
using CombatManager;
using Foundation;
using System.Collections.Generic;
using System.Diagnostics;

namespace CombatManagerMono
{
	public partial class MonsterEditorDialog : UIViewController
	{
		Monster _Monster;
		List<MonsterEditorPage> _Pages;
		List<GradientButton> _TabButtons;
		MonsterEditorPage _CurrentPage;		
		int _PageIndex = -1;
		
		MonsterEditorBasicPage _BasicPage;
		MonsterEditorDefensePage _DefensePage;
		MonsterEditorOffensePage _OffensePage;
		MonsterEditorStatisticsPage _StatisticsPage;
		MonsterEditorFeatsPage _FeatsPage;
		MonsterEditorSpecialPage _SpecialPage;
		MonsterEditorDescriptionPage _DescriptionPage;
		
		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorDialog (Monster monster)
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorDialog_iPhone" : "MonsterEditorDialog_iPad", null)
		{
			this.Monster = monster;
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
			
			BackgroundView.BackgroundColor = UIColor.Clear;
			BackgroundView.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
			BackgroundView.Border = 2.0f;
			BackgroundView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);
			
			HeaderView.BackgroundColor = UIColor.Clear;
			HeaderView.Border = 0;
			HeaderView.Gradient = new GradientHelper(CMUIColors.PrimaryColorDark);
			
			HeaderLabel.TextColor = UIColor.White;
			HeaderLabel.BackgroundColor = UIColor.Clear;
			
			_BasicPage = new MonsterEditorBasicPage();
			_DefensePage = new MonsterEditorDefensePage();
			_OffensePage = new MonsterEditorOffensePage();
			_StatisticsPage = new MonsterEditorStatisticsPage();
			_FeatsPage = new MonsterEditorFeatsPage();
			_SpecialPage = new MonsterEditorSpecialPage();
			_DescriptionPage = new MonsterEditorDescriptionPage();
			
			_Pages = new List<MonsterEditorPage>() { _BasicPage, _DefensePage, _OffensePage, _StatisticsPage, _FeatsPage, _SpecialPage, _DescriptionPage };
			
			_TabButtons = new List<GradientButton>() { BasicButton, DefenseButton, OffenseButton, StatisticsButton, FeatsButton, SpecialButton, DescriptionButton};
			
			PageBorderView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADarker);
			PageBorderView.Border = 2;
			PageBorderView.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
			
			foreach (MonsterEditorPage p in _Pages)
			{
				if (p != null)
				{
					p.CurrentMonster = Monster;
					p.DialogParent = this.View;
				}
			}
			
			ShowPage(0);

			UpdateOK();
			
			
		}

		
		public void UpdateOK()
		{
			
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Release any retained subviews of the main view.
			// e.g. this.myOutlet = null;
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
		
		partial void OKButtonTouchUpInside (Foundation.NSObject sender)
		{
			
			View.RemoveFromSuperview();
		}
		partial void CancelButtonTouchUpInside (Foundation.NSObject sender)
		{
			
			View.RemoveFromSuperview();
		}
		
		void ShowPage(int page)
		{
			Debug.Assert(page < _Pages.Count);
			
			if (_PageIndex != page)
			{
				if (_CurrentPage != null)
				{
					_CurrentPage.View.RemoveFromSuperview();
					
				}
				_PageIndex = page;
				_CurrentPage = _Pages[_PageIndex];
				if (_CurrentPage != null)
				{
					PageView.AddSubview(_CurrentPage.View);
				}
			}
			
			foreach (GradientButton b in _TabButtons)
			{
				StyleTab(b, b.Tag == _PageIndex);
			}
		}
		
		public Monster Monster
		{
			get
			{
				return _Monster;
			}
			set
			{
				if (_Monster != value)
				{
					_Monster = value;
					if (_Pages != null)
					{
						foreach (MonsterEditorPage page in _Pages)
						{
							page.CurrentMonster = _Monster;	
						}
					}
				}
			}
		}
		
		partial void TabButtonTouchUpInside (Foundation.NSObject sender)
		{
            ShowPage((int)((UIView)sender).Tag);
		}
		
		private void StyleTab(GradientButton b, bool selected)
		{
			b.CornerRadius = 0;
			
			UIColor colorA = CMUIColors.PrimaryColorDarker;
			UIColor colorB = CMUIColors.PrimaryColorMedium;
			
			if (selected)
			{
				b.Gradient = new GradientHelper(colorA, colorB);
			}
			else
			{
				b.Gradient = new GradientHelper(colorB, colorA);
			}
			b.SetTitleColor(UIColor.White, UIControlState.Normal);
			b.SetTitleColor(UIColor.Gray, UIControlState.Highlighted);
		}
	}
}

