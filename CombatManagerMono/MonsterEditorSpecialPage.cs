/*
 *  MonsterEditorSpecialPage.cs
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
using System.Collections.Generic;
using Foundation;
using CombatManager;

namespace CombatManagerMono
{
	public partial class MonsterEditorSpecialPage : MonsterEditorPage
	{
        List<SpecialAbilityView> _AbilityViews;

		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorSpecialPage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorSpecialPage_iPhone" : "MonsterEditorSpecialPage_iPad", null)
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
			
            AddAbilityButton.TouchUpInside += HandleAddTouchUpInside;
            CurrentMonster.SpecialAbilitiesList.CollectionChanged += (sender, e) => CreateSpecialItems();

            CreateSpecialItems();
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

        private void CreateSpecialItems()
        {
            if (_AbilityViews != null)
            {
                foreach (var view in _AbilityViews)
                {
                    view.RemoveFromSuperview();
                }
            }

            _AbilityViews = new List<SpecialAbilityView>();

            foreach (SpecialAbility ab in CurrentMonster.SpecialAbilitiesList)
            {
                SpecialAbilityView view = new SpecialAbilityView(CurrentMonster, ab);                
                SpecialScrollView.Add (view);
                _AbilityViews.Add(view);
            }

            LayoutSpecialItems();
        }

        private static float itemWidth = 400;
        private static float itemHeight = 170;
        private static float itemXGap = 20;
        private static float itemYGap = 20;

        private void LayoutSpecialItems()
        {
            float yLoc = itemYGap;
            int count = 0;
            float lastY = 0;

            foreach (SpecialAbilityView v in _AbilityViews)
            {
                float xLoc = ((count%2) == 0)?itemXGap:(itemXGap*2.0f+itemWidth);
                v.Frame = new CGRect(xLoc, yLoc, itemWidth, itemHeight);
                if (count%2==1)
                {
                    yLoc += itemHeight + itemYGap;
                }
                lastY = (float)v.Frame.Bottom;
                count++;
            }


            SpecialScrollView.ContentSize = new CGSize((itemXGap + itemWidth) * 2.0f, lastY);
        }

        
        void HandleAddTouchUpInside (object sender, EventArgs e)
        {
            SpecialAbility ab = new SpecialAbility();
            ab.Name = "Ability";
            ab.Type = "Ex";
            CurrentMonster.SpecialAbilitiesList.Add(ab);
        }
	}
}

