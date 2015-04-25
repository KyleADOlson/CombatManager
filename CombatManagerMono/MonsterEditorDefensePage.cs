/*
 *  MonsterEditorDefensePage.cs
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

namespace CombatManagerMono
{
	public partial class MonsterEditorDefensePage : MonsterEditorPage
	{
		private ButtonPropertyManager _HPManager;
		private ButtonPropertyManager _HDManager;
		private ButtonPropertyManager _HPModsManager;
		
		private ButtonPropertyManager _ACManager;
		private ButtonPropertyManager _TouchManager;
		private ButtonPropertyManager _FlatFootedManager;
		
		private ButtonPropertyManager _ArmorManager;
		private ButtonPropertyManager _DodgeManager;
		private ButtonPropertyManager _NaturalManager;
		private ButtonPropertyManager _DeflectionManager;
		private ButtonPropertyManager _ShieldManager;
		
		private ButtonPropertyManager _FortManager;
		private ButtonPropertyManager _RefManager;
		private ButtonPropertyManager _WillManager;
		
		private ButtonPropertyManager _DefensiveAbilitiesManager;
		private ButtonPropertyManager _DRManager;
		private ButtonPropertyManager _ImmuneManager;
		private ButtonPropertyManager _SRManager;
		private ButtonPropertyManager _ResistManager;
		private ButtonPropertyManager _WeaknessManager; 
		
		private  List<GradientView> _SectionViews;
		
		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorDefensePage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorDefensePage_iPhone" : "MonsterEditorDefensePage_iPad", null)
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
			

			SetupManagers();
			_SectionViews = new List<GradientView>() {HPView, ACView, ACModsView, SavesView, OtherView};
			
			foreach (GradientView v in _SectionViews)
			{
				StylePanel(v);
			}
			
		
		}
		
		private void SetupManagers()
		{
			_HPManager = new ButtonPropertyManager(HPButton, DialogParent, CurrentMonster, "HP");	
			_HDManager = new ButtonPropertyManager(HDButton, DialogParent, CurrentMonster.Adjuster, "HD");	
            _HPModsManager = new ButtonPropertyManager(HPModsButton, DialogParent, CurrentMonster, "HP_Mods") {Title="HP Mods"};	
				
			
			_ACManager = new ButtonPropertyManager(ACButton, DialogParent, CurrentMonster, "FullAC");
			_ACManager.Title = "AC";
			_TouchManager = new ButtonPropertyManager(TouchButton, DialogParent, CurrentMonster, "TouchAC");
			_TouchManager.Title = "Touch AC";
			_FlatFootedManager = new ButtonPropertyManager(FlatFootedButton, DialogParent, CurrentMonster, "FlatFootedAC");
			_FlatFootedManager.Title = "Flat Footed AC";
			
			_ArmorManager = new ButtonPropertyManager(ArmorButton, DialogParent, CurrentMonster.Adjuster, "Armor");	
			_DodgeManager = new ButtonPropertyManager(DodgeButon, DialogParent, CurrentMonster.Adjuster, "Dodge");	
			_NaturalManager = new ButtonPropertyManager(NaturalButton, DialogParent, CurrentMonster.Adjuster, "NaturalArmor");	
			_NaturalManager.Title = "Natural Armor";
			_DeflectionManager = new ButtonPropertyManager(DeflectionButton, DialogParent, CurrentMonster.Adjuster, "Deflection");	
			_ShieldManager = new ButtonPropertyManager(ShieldButton, DialogParent, CurrentMonster.Adjuster, "Shield");	
				
			_FortManager = new ButtonPropertyManager(FortButton, DialogParent, CurrentMonster, "Fort");	
			_RefManager = new ButtonPropertyManager(RefButton, DialogParent, CurrentMonster, "Ref");	
			_WillManager = new ButtonPropertyManager(WillButton, DialogParent, CurrentMonster, "Will");	
				
            _DefensiveAbilitiesManager = new ButtonPropertyManager(DefensiveAbilitiesButton, DialogParent, CurrentMonster, "DefensiveAbilities") {Title="Defensive Abilities"};	
			_DRManager = new ButtonPropertyManager(DRButton, DialogParent, CurrentMonster, "DR");	
			_ImmuneManager = new ButtonPropertyManager(ImmuneButton, DialogParent, CurrentMonster, "Immune");	
			_SRManager = new ButtonPropertyManager(SRButton, DialogParent, CurrentMonster, "SR");	
			_ResistManager = new ButtonPropertyManager(ResistButton, DialogParent, CurrentMonster, "Resist");	
			_WeaknessManager = new ButtonPropertyManager(WeaknessButton, DialogParent, CurrentMonster, "Weaknesses");          	
			
			PropertyManagers.Add(_HPManager);
			PropertyManagers.Add(_HDManager);
			PropertyManagers.Add(_HPModsManager);
			
			PropertyManagers.Add(_ACManager);
			PropertyManagers.Add(_TouchManager);
			PropertyManagers.Add(_FlatFootedManager);
			
			PropertyManagers.Add(_ArmorManager);
			PropertyManagers.Add(_DodgeManager);
			PropertyManagers.Add(_NaturalManager);
			PropertyManagers.Add(_DeflectionManager);
			PropertyManagers.Add(_ShieldManager);
			
			PropertyManagers.Add(_FortManager);
			PropertyManagers.Add(_RefManager);
			PropertyManagers.Add(_WillManager);
			
			PropertyManagers.Add(_DefensiveAbilitiesManager);
			PropertyManagers.Add(_DRManager);
			PropertyManagers.Add(_ImmuneManager);
			PropertyManagers.Add(_SRManager);
			PropertyManagers.Add(_ResistManager);
			PropertyManagers.Add(_WeaknessManager); 
			
			foreach (ButtonPropertyManager m in PropertyManagers)
			{
				GradientButton b = (GradientButton)m.Button;
                CMStyles.TextFieldStyle(b);
				
			}
			
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

