/*
 *  AdvancerPanel.cs
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
    enum AdvancerTemplate
    {
        None,
        HalfDragon,
        HalfCelestial,
        HalfFiend,
        Skeleton,
        Vampire,
        Zombie
    }

	public partial class AdvancerPanel : UIViewController
	{



		List<GradientView> _Panels;
		List<GradientView> _Headers;
		
		int advancedLevel;
		int simpleSize;
		Monster.HalfOutsiderType ? simpleOutsiderType;
		bool augmentSummoning;
		
		int racialHDChange;
		Stat racialBonusStat = Stat.Strength;
		bool racialSizeChange = false;
        AdvancerTemplate template = AdvancerTemplate.None;

        Monster.ZombieType _ZombieType = Monster.ZombieType.Normal;
        string _DragonColor = Monster.DragonColors[0];
        bool [] _SelectedSkeletonTypes = new bool[3];
        bool [] _SelectedStats = new bool[6];
		
		ButtonStringPopover advancedPopover;
		ButtonStringPopover simpleSizePopover;
		ButtonStringPopover simpleOutsiderPopover;
		ButtonStringPopover augmentSummoningPopover;
		
		ButtonStringPopover racialHitDiePopover;
		ButtonStringPopover racialBonusPopover;
		ButtonStringPopover racialSizePopover;

        ButtonStringPopover otherTemplatePopover;

        List<ButtonStringPopover> _OtherPopovers;
		
		public event EventHandler AdvancementChanged;
		public event EventHandler AddMonsterClicked;


        GradientButton[] _OtherButtons;
		
		public AdvancerPanel () : base ("AdvancerPanel", null)
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
			
			_Panels = new List<GradientView>() {SimpleView, RacialView, OtherView};
			_Headers = new List<GradientView>() {SimpleHeaderView, RacialHeaderView, OtherHeaderView};
            
            _OtherButtons = new GradientButton[] 
                {OtherButton1, OtherButton2, OtherButton3, OtherButton4, OtherButton5, OtherButton6};
			
			foreach (var v in _Panels)
			{
				StylePanel(v);
			}
			
			foreach (var v in _Headers)
			{
				StyleHeader(v);
			}
			
			StyleButton(RacialHitDieButton, false);
			StyleButton(RacialBonusButton, false);
			StyleButton(RacialSizeButton, false);
			
			StyleButton(OtherTemplateButton, false);
			
			SetupButtons();
			
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
			return true;
		}
		
		public void SetupButtons()
		{
			//advanced button
			advancedPopover = new ButtonStringPopover(AdvancedButton);
			advancedPopover.Items.Add(new ButtonStringPopoverItem() {Text="None", Tag = 0});
			for (int i=1; i<4; i++)
			{
				advancedPopover.Items.Add(new ButtonStringPopoverItem() {Text="Advanced x" + i, Tag = i});
			}
			advancedPopover.ItemClicked += HandleAdvancedPopoverItemClicked;
			
			//simple size button
			simpleSizePopover = new ButtonStringPopover(SimpleSizeButton);
			for (int i=-3; i<0; i++)
			{
				simpleSizePopover.Items.Add(new ButtonStringPopoverItem() {Text="Young x" + (-i), Tag = i});
			}
			simpleSizePopover.Items.Add(new ButtonStringPopoverItem() {Text="None", Tag = 0});
			for (int i=1; i<4; i++)
			{
				simpleSizePopover.Items.Add(new ButtonStringPopoverItem() {Text="Giant x" + i, Tag = i});
			}
			simpleSizePopover.ItemClicked += HandleSimpleSizePopoverItemClicked;
			
			//simple outsider button
			simpleOutsiderPopover = new ButtonStringPopover(OutsiderButton);
			simpleOutsiderPopover.Items.Add(new ButtonStringPopoverItem() {Text="None", Tag = null});
			foreach (Monster.HalfOutsiderType t in Enum.GetValues(typeof(Monster.HalfOutsiderType)))
			{
				simpleOutsiderPopover.Items.Add(new ButtonStringPopoverItem() {Text = Monster.GetOutsiderTypeName(t).Capitalize(), 
					Tag = new Monster.HalfOutsiderType ?(t)});
			}
			simpleOutsiderPopover.ItemClicked += HandleSimpleOutsiderPopoverItemClicked;
			
			//simple outsider button
			augmentSummoningPopover = new ButtonStringPopover(AugmentSummoningButton);
			augmentSummoningPopover.Items.Add(new ButtonStringPopoverItem() {Text="On", Tag = true});
			augmentSummoningPopover.Items.Add(new ButtonStringPopoverItem() {Text="Off", Tag = false});
			
			augmentSummoningPopover.ItemClicked += HandleAugmentSummoningPopoverItemClicked;
			
			
			racialHitDiePopover = new ButtonStringPopover(RacialHitDieButton);

			for(int i = -20; i<21; i++)
			{
				if (i == 0)
				{
				
					racialHitDiePopover.Items.Add(new ButtonStringPopoverItem() {Text="None", Tag = 0});	
				}
				else
				{
					racialHitDiePopover.Items.Add(new ButtonStringPopoverItem() {Text=i.PlusFormat(), Tag = i});
				}
			}
			racialHitDiePopover.ItemClicked += HandleRacialHitDiePopoverItemClicked;
			UpdateButtonState();	
			
			
			racialBonusPopover = new ButtonStringPopover(RacialBonusButton);
			foreach (Stat t in Enum.GetValues(typeof(Stat)))
			{
				racialBonusPopover.Items.Add(new ButtonStringPopoverItem() {Text = Monster.StatText(t).Capitalize(), 
					Tag = t});
			}
			racialBonusPopover.ItemClicked += HandleRacialBonusPopoverItemClicked;
			
			
			racialSizePopover = new ButtonStringPopover(RacialSizeButton);
			racialSizePopover.Items.Add(new ButtonStringPopoverItem() {Text="50% HD", Tag = true});
			racialSizePopover.Items.Add(new ButtonStringPopoverItem() {Text="No Size Change", Tag = false});
			
			racialSizePopover.ItemClicked += HandleRacialSizePopoverItemClicked;
			
			ResetButton.SetImage(UIExtensions.GetSmallIcon("reset"), UIControlState.Normal);
			
			AdvancerHeaderView.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDarker);
			AdvancerHeaderView.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
			
			AddMonsterButton.SetSmallIcon("sword");

            otherTemplatePopover = new ButtonStringPopover(OtherTemplateButton);
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="None", Tag=AdvancerTemplate.None});
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="Half-Celestial", Tag=AdvancerTemplate.HalfCelestial});
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="Half-Dragon", Tag=AdvancerTemplate.HalfDragon});
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="Half-Fiend", Tag=AdvancerTemplate.HalfFiend});
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="Skeleton", Tag=AdvancerTemplate.Skeleton});
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="Vampire", Tag=AdvancerTemplate.Vampire});
            otherTemplatePopover.Items.Add(new ButtonStringPopoverItem() {Text="Zombie", Tag=AdvancerTemplate.Zombie});
		    otherTemplatePopover.ItemClicked += HandleOtherTemplateItemClicked;
            _OtherPopovers = new List<ButtonStringPopover>();
            for (int i=0; i<_OtherButtons.Length; i++)
            {
                _OtherPopovers.Add(new ButtonStringPopover(_OtherButtons[i]));
                _OtherPopovers[i].Data = i;
                _OtherPopovers[i].WillShowPopover += HandleWillShowOtherPopover;
                _OtherPopovers[i].ItemClicked += HandleOtherPopoverItemClicked;
            }
        }

        void HandleOtherPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            ButtonStringPopover popover = (ButtonStringPopover)sender;
            GradientButton b = (GradientButton)popover.Button;
            switch (template)
            {
            case AdvancerTemplate.HalfDragon:
                _DragonColor = (string)e.Tag;
                break;
            case AdvancerTemplate.Skeleton:
                _SelectedSkeletonTypes[b.Tag] = (bool)e.Tag;
                break;
            case AdvancerTemplate.HalfCelestial:
            case AdvancerTemplate.HalfFiend:
                _SelectedStats[b.Tag] = (bool)e.Tag;
                break;
            case AdvancerTemplate.Zombie:
                _ZombieType = (Monster.ZombieType)e.Tag;
                break;
            }
            HandleChange();

        }

        void HandleWillShowOtherPopover (object sender, WillShowPopoverEventArgs e)
        {
            ButtonStringPopover popover = (ButtonStringPopover)sender;
            int item = (int)popover.Data;
            popover.Items.Clear();

            switch (template)
            {
            case AdvancerTemplate.Skeleton:
                e.Cancel = true;
                _SelectedSkeletonTypes[item] = !_SelectedSkeletonTypes[item];
                HandleChange();

                break;
            case AdvancerTemplate.Zombie:
                popover.Items.Add (new ButtonStringPopoverItem() {Text=ZombieText(Monster.ZombieType.Normal, false), Tag=Monster.ZombieType.Normal});
                popover.Items.Add (new ButtonStringPopoverItem() {Text=ZombieText(Monster.ZombieType.Fast, false), Tag=Monster.ZombieType.Fast});
                popover.Items.Add (new ButtonStringPopoverItem() {Text=ZombieText(Monster.ZombieType.Plague, false), Tag=Monster.ZombieType.Plague});
                break;    
            case AdvancerTemplate.HalfDragon:
                foreach (string s in Monster.DragonColors)
                {
                    popover.Items.Add (new ButtonStringPopoverItem() {Text=s.Capitalize(), Tag=s});
                }
                break;
            case AdvancerTemplate.HalfCelestial:
            case AdvancerTemplate.HalfFiend:
                e.Cancel = true;                
                _SelectedStats[item] = !_SelectedStats[item];
                HandleChange();
                break;
            }
            
        }

        void HandleOtherTemplateItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            template = (AdvancerTemplate)e.Tag;
            HandleChange();
        }

		void HandleRacialBonusPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			racialBonusStat = (Stat)e.Tag;
			HandleChange();
		}

		void HandleRacialSizePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			racialSizeChange = (bool)e.Tag;
			HandleChange();
		}

		void HandleRacialHitDiePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			racialHDChange = (int)e.Tag;
			HandleChange();
		}

		void HandleAugmentSummoningPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			augmentSummoning = (bool)e.Tag;
			HandleChange();
		}

		void HandleSimpleOutsiderPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			simpleOutsiderType = (Monster.HalfOutsiderType?)e.Tag;
			HandleChange();
		}

		void HandleSimpleSizePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			simpleSize = (int)e.Tag;
			HandleChange();
		}

		void HandleAdvancedPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			advancedLevel = (int)e.Tag;
			HandleChange();
		}
		
		public void HandleChange()
		{
			
			UpdateButtonState();
			if (AdvancementChanged != null)
			{
				AdvancementChanged(this, new EventArgs());
			}
		}
		
		
		public void UpdateButtonState()
		{
			StyleButton (AdvancedButton, advancedLevel > 0);
			if (advancedLevel == 0)
			{
				AdvancedButton.SetText("Advanced");
			}
			else
			{
				AdvancedButton.SetText("Advanced x" + advancedLevel);	
			}
			
			StyleButton(SimpleSizeButton, simpleSize != 0);
			if (simpleSize == 0)
			{
				SimpleSizeButton.SetText("Size");
			}
			else if (simpleSize < 0)
			{
				SimpleSizeButton.SetText("Young x" + (-simpleSize));
			}
			else
			{
				SimpleSizeButton.SetText("Giant x" + simpleSize);	
			}
			
			StyleButton(OutsiderButton, simpleOutsiderType != null);
			if (simpleOutsiderType != null)
			{
				OutsiderButton.SetText(Monster.GetOutsiderTypeName(simpleOutsiderType.Value).Capitalize());
			}
			else
			{
				OutsiderButton.SetText("Outsider");	
			}
			
			StyleButton(AugmentSummoningButton, augmentSummoning);
			
			bool racialChange = racialHDChange != 0;
			StyleButton(RacialHitDieButton, racialChange);
			StyleButton(RacialBonusButton, racialChange);
			StyleButton(RacialSizeButton, racialChange);
			if (!racialChange)
			{
				RacialHitDieButton.SetText("Racial Hit Dice");
			}
			else
			{
				RacialHitDieButton.SetText(racialHDChange.PlusFormat() + " HD");	
			}
			
			RacialBonusButton.SetText("Bonus Stat " + Monster.ShortStatText(racialBonusStat));
			
			if (racialSizeChange)
			{
				RacialSizeButton.SetText("Size Change 50% HD");	
			}
			else
			{
				RacialSizeButton.SetText("No Size Change");
			}

            StyleButton(OtherTemplateButton, template != AdvancerTemplate.None);
            switch (template)
            {
            case AdvancerTemplate.None:
            case AdvancerTemplate.Vampire:
                OtherTemplateButton.SetText("Other Template");
                SetOtherButtonsVisible(0);
                break;
            case AdvancerTemplate.Skeleton:

                OtherTemplateButton.SetText("Skeleton");
                SetOtherButtonsVisible(3);
                for (int i=0; i<3; i++)
                {

                    _OtherButtons[i].SetText(SkeletonText(i));
                    _OtherButtons[i].Tag = i;
                    StyleButton(_OtherButtons[i], _SelectedSkeletonTypes[i]);
                }

                break;
            case AdvancerTemplate.Zombie:
                OtherTemplateButton.SetText("Zombie");
                SetOtherButtonsVisible(1);
                _OtherButtons[0].SetText(ZombieText(_ZombieType, false));
                break;
            case AdvancerTemplate.HalfCelestial:
            case AdvancerTemplate.HalfFiend:
                SetOtherButtonsVisible(6);
                OtherTemplateButton.SetText(template==AdvancerTemplate.HalfCelestial?"Half-Celestial":"Half-Fiend");
                for (int i=0; i<6; i++)
                {
                    _OtherButtons[i].SetText(Monster.StatText((Stat)i));
                    _OtherButtons[i].Tag = i;
                    StyleButton(_OtherButtons[i], _SelectedStats[i]);
                }
                break;
            case AdvancerTemplate.HalfDragon:
                OtherTemplateButton.SetText("Half-Dragon");
                _OtherButtons[0].SetText(_DragonColor.Capitalize());
                SetOtherButtonsVisible(1);
                break;
            }
				                         
		}


        public string SkeletonText(int skel)
        {
            switch (skel)
            {
            case 0:
                return "Bloody";
            case 1:
                return "Burning";
            case 2:
                return "Champion";
            }
            return "";
        }

        public string ZombieText(Monster.ZombieType type, bool append)
        {
            switch (type)
            {
            case Monster.ZombieType.Fast:
                return append?"Fast Zombie":"Fast";
    
            case Monster.ZombieType.Normal:
                return append?"Zombie": "Normal";
            case Monster.ZombieType.Plague:
                return append?"Plague Zombie":"Plague";
            }
            return "";
        }

        public void SetOtherButtonsVisible(int buttonCount)
        {

            for (int i=0; i<_OtherButtons.Length; i++)
            {
                _OtherButtons[i].Hidden = i >= buttonCount;
                StyleButton(_OtherButtons[i], true);

            }

           
        }
		
		public void StylePanel(GradientView panel)
		{
			panel.BackgroundColor = UIColor.Clear;
			panel.Gradient = new GradientHelper(CMUIColors.SecondaryColorAMedium);
			panel.Border = 0;
		}
		
		public void StyleHeader(GradientView header)
		{
			
			header.BackgroundColor = UIColor.Clear;
			header.Gradient = new GradientHelper(CMUIColors.PrimaryColorDark);
			header.Border = 0;
		}
		
		public void StyleButton(GradientButton button, bool selected)
		{
			button.SetTitleColor(UIColor.White, UIControlState.Normal);
			UIColor color1 = CMUIColors.PrimaryColorDarker;
			UIColor color2 = CMUIColors.PrimaryColorLight;
			if (selected)
			{
				button.Gradient = new GradientHelper(color1, color2);
				button.SetTitleColor(UIColor.White, UIControlState.Normal);
			}
			else
			{
				button.Gradient = new GradientHelper(UIExtensions.RGBColor(0xEEEEEE), UIExtensions.RGBColor(0xEEEEEE));
				button.SetTitleColor(UIExtensions.ARGBColor(0xFF999999), UIControlState.Normal);
			}
		}

        public HashSet<Stat> SelectedStats
        {
            get
            {
                HashSet<Stat> selStat = new HashSet<Stat>();
                for (int i=0; i<6; i++)
                {
                    if (_SelectedStats[i])
                    {
                        selStat.Add((Stat)i);
                    }
                }
                return selStat;
            }
        }
		
		public Monster AdvanceMonster(Monster oldMonster)
		{
			if (oldMonster == null)
			{
				return null;
			}
			
			Monster monster = (Monster)oldMonster.Clone();

            
            switch (template)
            {
            case AdvancerTemplate.HalfDragon:
                if (monster.MakeHalfDragon(_DragonColor))
                {
                    monster.Name =  "Half-Dragon " + monster.Name;
                }
                break;
            case AdvancerTemplate.Skeleton:
                if (monster.MakeSkeleton(_SelectedSkeletonTypes[0], _SelectedSkeletonTypes[1], _SelectedSkeletonTypes[2]))
                {
                    monster.Name = "Skeletal " + monster.Name;               
                    for (int i=2; i>=0; i--)
                    {
                        if (_SelectedSkeletonTypes[i])
                        {
                            monster.Name = SkeletonText(i)+ " " + monster.Name;
                        }
                    }
                }
                break;
            case AdvancerTemplate.Zombie:
                if (monster.MakeZombie(_ZombieType))
                {
                    monster.Name = ZombieText(_ZombieType, true) + " " + monster.Name;
                }
                break;
            case AdvancerTemplate.Vampire:

                if (monster.MakeVampire())
                {
                    monster.Name = "Vampire " + monster.Name;
                }
                break;
            case AdvancerTemplate.HalfCelestial:
                if (SelectedStats.Count == 3 &&monster.MakeHalfCelestial(SelectedStats))
                {
                    monster.Name = "Half-Celestial " + monster.Name;

                }
                break;
            case AdvancerTemplate.HalfFiend:
                if (SelectedStats.Count == 3 &&  monster.MakeHalfFiend(SelectedStats))
                {
                    monster.Name = "Half-Fiend " + monster.Name;                   
                }
                break;
               

            }
			
            if (racialHDChange != 0)
            {
                int dice = racialHDChange;

                Stat stat = racialBonusStat;

                bool size = racialSizeChange;

                int res = monster.AddRacialHD(dice, stat, size);

                if (res != 0)
                {
                    monster.Name = monster.Name + " " + CMStringUtilities.PlusFormatNumber(res) + " HD";
                }
            }

            if (advancedLevel > 0)
            {
                int count = advancedLevel;

                int added = 0;

                for (int i = 0; i < count; i++)
                {
                    if (monster.MakeAdvanced())
                    {
                        added++;
                    }
                }

                if (added > 0)
                {
                    monster.Name += " (Adv " + added.PlusFormat() + ")";
                }
            }
            if (simpleSize != 0)
            {

                if (simpleSize > 0)
                {
					int count = simpleSize;
                    int added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (monster.MakeGiant())
                        {
                            added++;
                        }

                    }
                    if (added == 1)
                    {
                        monster.Name = "Giant " + monster.Name;
                    }
                    if (added > 1)
                    {
                        monster.Name = "Giant x" + added + " " + monster.Name;
                    }
                }
                else
                {
					int count = -simpleSize;
                    int added = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (monster.MakeYoung())
                        {
                            added++;
                        }
                    }
                    if (added == 1)
                    {
                        monster.Name = "Young " + monster.Name;
                    }
                    if (added > 1)
                    {
                        monster.Name = "Young x" + added + " " + monster.Name;
                    }
                }
            }
            if (simpleOutsiderType != null)
            {
                switch (simpleOutsiderType)
				{
				case Monster.HalfOutsiderType.Celestial:
                    if (monster.MakeCelestial())
                    {
                        monster.Name = "Celestial " + monster.Name;
                    }
					break;
				case Monster.HalfOutsiderType.Entropic:
                    if (monster.MakeEntropic())
                    {
                        monster.Name = "Entopic " + monster.Name;
                    }
					break;
				case Monster.HalfOutsiderType.Fiendish:
                    if (monster.MakeFiendish())
                    {
                        monster.Name = "Fiendish " + monster.Name;
                    }
					break;
				case Monster.HalfOutsiderType.Resolute:
                    if (monster.MakeResolute())
                    {
                        monster.Name = "Resolute " + monster.Name;
                    }
					break;
                }
            }
            if (augmentSummoning)
            {
                monster.AugmentSummoning();
                monster.Name = "Augmented " + monster.Name;
            }
			
			
			return monster;
		}
		
		partial void AddMonsterButtonTouchUpInside (Foundation.NSObject sender)
		{
			if (AddMonsterClicked != null)
			{
				AddMonsterClicked(this, new EventArgs());
			}
		}
		
		partial void AdvancerHeaderButtonTouchUpInside (Foundation.NSObject sender)
		{
			
		}
		
		partial void ResetButtonTouchUpInside (Foundation.NSObject sender)
		{
			ResetFilters();
		}
		
		public void ResetFilters()
		{
			advancedLevel = 0;
			simpleSize = 0 ;
			simpleOutsiderType = null;
			augmentSummoning = false;
			
			racialHDChange = 0;
			racialBonusStat = Stat.Strength;
			racialSizeChange = false;	
			
			HandleChange();
		}
		
		
	}
}

