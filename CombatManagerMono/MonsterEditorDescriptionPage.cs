/*
 *  MonsterEditorDescriptionPage.cs
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
	public partial class MonsterEditorDescriptionPage : MonsterEditorPage
	{


        List<ButtonPropertyManager> _Managers = new List<ButtonPropertyManager>();


		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorDescriptionPage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorDescriptionPage_iPhone" : "MonsterEditorDescriptionPage_iPad", null)
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
            
            _Managers.Add( new ButtonPropertyManager(EnvironmentButton, DialogParent, CurrentMonster, "Environment") {Multiline = true});
            _Managers.Add( new ButtonPropertyManager(OrganizationButton, DialogParent, CurrentMonster, "Organization") {Multiline = true}); 
            _Managers.Add( new ButtonPropertyManager(TreasureButton, DialogParent, CurrentMonster, "Treasure") {Multiline = true}); 
            _Managers.Add( new ButtonPropertyManager(BeforeCombatButton, DialogParent, CurrentMonster, "BeforeCombat") {Multiline = true, Title="Before Combat"}); 
            _Managers.Add( new ButtonPropertyManager(DuringCombatButton, DialogParent, CurrentMonster, "DuringCombat") {Multiline = true, Title="During Combat"}); 
            _Managers.Add( new ButtonPropertyManager(MoraleButton, DialogParent, CurrentMonster, "Morale") {Multiline = true}); 
            _Managers.Add( new ButtonPropertyManager(VisualDescriptionButton, DialogParent, CurrentMonster, "Description_Visual") {Multiline = true, Title="Visual Description"}); 
            _Managers.Add( new ButtonPropertyManager(DescriptionButton, DialogParent, CurrentMonster, "Description") {Multiline = true}); 
 


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
	}
}

