/*
 *  MonsterEditorBasicPage.cs
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
using CombatManager;
using Foundation;

namespace CombatManagerMono
{
	public partial class MonsterEditorBasicPage : MonsterEditorPage
	{
		private ButtonPropertyManager _NameManager;
		private ButtonPropertyManager _ClassManager;
		private ButtonPropertyManager _SensesManager;
		private ButtonPropertyManager _RaceManager;
		private ButtonPropertyManager _InitManager;
		private ButtonPropertyManager _SubtypeManager;
		private ButtonPropertyManager _CRManager;
		private ButtonPropertyManager _AlignmentManager;
		private ButtonPropertyManager _CreatureTypeManager;
		private ButtonPropertyManager _CreatureSizeManager;
	
		
		private Dictionary<Stat, UIButton> _StatButtons = 
			new Dictionary<Stat, UIButton>();
		private Dictionary<Stat,  ButtonPropertyManager> _StatManagers = 
			new Dictionary<Stat, ButtonPropertyManager>();
		
		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MonsterEditorBasicPage ()
			: base (UserInterfaceIdiomIsPhone ? "MonsterEditorBasicPage_iPhone" : "MonsterEditorBasicPage_iPad", null)
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
			
			StylePanel(this.AbilityArea);
			StylePanel(this.HeaderArea);
						
			_NameManager = new ButtonPropertyManager(NameButton, DialogParent, CurrentMonster, "Name"); 	
			PropertyManagers.Add(_NameManager);
			
			_ClassManager = new ButtonPropertyManager(ClassButton, DialogParent, CurrentMonster, "Class");  	
			PropertyManagers.Add(_ClassManager);
			_SensesManager = new ButtonPropertyManager(SensesButton, DialogParent, CurrentMonster, "Senses"); 	 	
			PropertyManagers.Add(_SensesManager);
			_RaceManager = new ButtonPropertyManager(RaceButton, DialogParent, CurrentMonster, "Race"); 	 	
			PropertyManagers.Add(_RaceManager);
			_InitManager = new ButtonPropertyManager(InitButton, DialogParent, CurrentMonster, "Init"); 	 	
			PropertyManagers.Add(_InitManager);
			_SubtypeManager = new ButtonPropertyManager(CreatureSubtypeButton, DialogParent, CurrentMonster.Adjuster, "Subtype");
			_SubtypeManager.Title = "Subtype"; 	
			PropertyManagers.Add(_SubtypeManager);
			
			
			_StatButtons.Add(Stat.Strength, StrengthButton);
			_StatButtons.Add(Stat.Dexterity, DexterityButton);
			_StatButtons.Add(Stat.Constitution, ConstitutionButton);
			_StatButtons.Add(Stat.Intelligence, IntelligenceButton);
			_StatButtons.Add(Stat.Charisma, CharismaButton);
			_StatButtons.Add(Stat.Wisdom, WisdomButton);
			
			foreach (KeyValuePair<Stat, UIButton> pair in _StatButtons)
			{
				ButtonPropertyManager m = new ButtonPropertyManager(pair.Value, DialogParent, 
				                                                    CurrentMonster.Adjuster, Monster.StatText(pair.Key));
				m.MinIntValue = 0;
				m.MaxIntValue = 99;
				m.FormatDelegate = delegate (object num)
				{
					string numText;
					if (num == null)
					{
						numText = "-";	
					}
					else
					{
						numText = ((int?)num).ToString();	
					}
					
					numText += "  ";
					numText += Monster.AbilityBonus((int?)num).PlusFormat();
					
					return numText;
				};
				_StatManagers[pair.Key] = m;
                PropertyManagers.Add(m);
			}
			
			_CRManager = new ButtonPropertyManager(CRButton, DialogParent, CurrentMonster.Adjuster, "CR");
			var crList = new List<KeyValuePair<object, string>>();
			
			
			crList.Add(new KeyValuePair<object, string>("1/8", "1/8"));
			crList.Add(new KeyValuePair<object, string>("1/6", "1/6"));
			crList.Add(new KeyValuePair<object, string>("1/4", "1/4"));
			crList.Add(new KeyValuePair<object, string>("1/3", "1/3"));
			crList.Add(new KeyValuePair<object, string>("1/2", "1/2"));
			for (int i = 1; i<31; i++)
			{
				crList.Add(new KeyValuePair<object, string>(i.ToString(), i.ToString()));
			}
				           
			_CRManager.ValueList = crList; 
            PropertyManagers.Add(_CRManager);
			
			_AlignmentManager = new ButtonPropertyManager(AlignmentButton, DialogParent, CurrentMonster, "Alignment");
			PropertyManagers.Add(_AlignmentManager);

			var alignmentList = new List<KeyValuePair<object, string>>();
			for (int i=0; i<3; i++)
			{
				for (int j=0; j<3; j++)
				{
					string al = Monster.AlignmentText(
						new Monster.AlignmentType(){Order = (Monster.OrderAxis)i, Moral=(Monster.MoralAxis)j});
					
					alignmentList.Add(new KeyValuePair<object, string>(al, al));	
				}
			}
			_AlignmentManager.ValueList = alignmentList;
			
			
			_CreatureTypeManager = new ButtonPropertyManager(CreatureTypeButton, DialogParent, CurrentMonster, "Type");
			PropertyManagers.Add(_CreatureTypeManager);
			var typeList = new List<KeyValuePair<object, string>>();
			foreach (CreatureType t in Enum.GetValues(typeof(CreatureType)))
			{
				string name = Monster.CreatureTypeNames[(int)t];
				
				typeList.Add(new KeyValuePair<object, string>(name, name.Capitalize()));
			}
			_CreatureTypeManager.ValueList = typeList;
			
			_CreatureSizeManager = new ButtonPropertyManager(SizeButton, DialogParent, CurrentMonster.Adjuster, "MonsterSize");
			PropertyManagers.Add(_CreatureSizeManager);
			var sizeList = new List<KeyValuePair<object, string>>();
			foreach (MonsterSize s in Enum.GetValues(typeof(MonsterSize)))
			{
				string name = SizeMods.GetSizeText(s);
				
				
				sizeList.Add(new KeyValuePair<object, string>((int)s, name));
			}
			_CreatureSizeManager.FormatDelegate = delegate (object s)
				{
					return SizeMods.GetSizeText((MonsterSize)s);	
				};
			_CreatureSizeManager.ValueList = sizeList;
			
			foreach (ButtonPropertyManager m in PropertyManagers)
            {
                GradientButton b = m.Button as GradientButton;

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
		
		public override void MonsterUpdated (CombatManager.Monster oldMonster, CombatManager.Monster newMonster)
		{
			base.MonsterUpdated (oldMonster, newMonster);

			
			
		}

			
	}
}

