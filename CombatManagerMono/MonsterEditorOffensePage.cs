/*
 *  MonsterEditorOffensePage.cs
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
using System.Linq;
using Foundation;
using CombatManager;

namespace CombatManagerMono
{
	public partial class MonsterEditorOffensePage : MonsterEditorPage
	{

        List<ButtonPropertyManager> _Managers = new List<ButtonPropertyManager>();
        AttacksEditorDialog _AEDialog; 

		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorOffensePage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorOffensePage_iPhone" : "MonsterEditorOffensePage_iPad", null)
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

            foreach (GradientView v in new GradientView[] {SpeedView, AttacksView, AbilitiesView})
            {
                StylePanel(v);
            }

            CurrentMonster.Adjuster.PropertyChanged += MonsterAdjusterPropertyChanged;
			
            ButtonPropertyManager m;
            m = new ButtonPropertyManager(SpeedButton, DialogParent, CurrentMonster.Adjuster, "LandSpeed") {Title = "Land Speed", MinIntValue=0};
            _Managers.Add(m);
            m = new ButtonPropertyManager(ClimbButton, DialogParent, CurrentMonster.Adjuster, "ClimbSpeed") {Title = "Climb Speed", MinIntValue=0};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SpaceButton, DialogParent, CurrentMonster.Adjuster, "Space") {Title = "Space"};
            _Managers.Add(m);
            m = new ButtonPropertyManager(FlyButton, DialogParent, CurrentMonster.Adjuster, "FlySpeed") {Title = "Fly Speed", MinIntValue=0};
            _Managers.Add(m);
            m = new ButtonPropertyManager(BurrowButton, DialogParent, CurrentMonster.Adjuster, "BurrowSpeed") {Title = "Burrow Speed", MinIntValue=0};
            _Managers.Add(m);
            m = new ButtonPropertyManager(ReachButton, DialogParent, CurrentMonster.Adjuster, "Reach") {Title = "Reach"};
            _Managers.Add(m);
            m = new ButtonPropertyManager(FlyQualityButton, DialogParent, CurrentMonster.Adjuster, "FlyQuality") {Title = "Fly Quality"};

            var list = new List<KeyValuePair<object, string>>();
            for (int i=0; i<5; i++)
            {
                list.Add (new KeyValuePair<object, string>(i, Monster.GetFlyQualityString(i).Capitalize()));
            }
            m.ValueList = list;
            m.FormatDelegate = a => 
            {
                if (CurrentMonster.Adjuster.FlySpeed == null)
                {
                    return "-";
                }
                else
                {
                    return Monster.GetFlyQualityString((int)a).Capitalize();
                }
            };
            _Managers.Add(m);

            m = new ButtonPropertyManager(SwimButton, DialogParent, CurrentMonster.Adjuster, "SwimSpeed") {Title = "Swim Speed"};
            _Managers.Add(m);

            
            m = new ButtonPropertyManager(SpecialAttacksButton, DialogParent, CurrentMonster, "SpecialAttacks") {Title = "Special Attacks", Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SpellLikeAbilitiesButton, DialogParent, CurrentMonster.Adjuster, "SpellLikeAbilities") {Title = "Spell-Like Abilities", Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SpellsKnownButton, DialogParent, CurrentMonster.Adjuster, "SpellsKnown") {Title = "Spells Known", Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(SpellsPreparedButton, DialogParent, CurrentMonster.Adjuster, "SpellsPrepared") {Title = "SpellsPrepared", Multiline=true};
            _Managers.Add(m);

            m = new ButtonPropertyManager(MeleeButton, DialogParent, CurrentMonster, "Melee") {Title = "Melee", Multiline=true};
            _Managers.Add(m);
            m = new ButtonPropertyManager(RangedButton, DialogParent, CurrentMonster, "Ranged") {Title = "Ranged", Multiline=true};
            _Managers.Add(m);
           

            foreach (GradientButton b in from x in _Managers select x.Button)
            {
                CMStyles.TextFieldStyle(b, 15f);
            }

            AttacksEditorButton.TouchUpInside += (sender, e) => 
            {
                _AEDialog = new AttacksEditorDialog(CurrentMonster);
                _AEDialog.OKClicked += (senderx, e1) => 
                {
                    CharacterAttacks at = _AEDialog.Attacks;
                    CurrentMonster.Melee = CurrentMonster.MeleeString(at);
                    CurrentMonster.Ranged = CurrentMonster.RangedString(at);
                };
                DialogParent.Add (_AEDialog.View);
            };

            UpdateButtonState();
			
		}

        void MonsterAdjusterPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Speed")
            {
                UpdateButtonState();
            }
        }

        void UpdateButtonState()
        {
            FlyQualityButton.Enabled = CurrentMonster.Adjuster.FlySpeed != null;
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

