using MonoTouch.UIKit;
using System.Drawing;
using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using CombatManager;

namespace CombatManagerMono
{
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
		
		ButtonStringPopover advancedPopover;
		ButtonStringPopover simpleSizePopover;
		ButtonStringPopover simpleOutsiderPopover;
		ButtonStringPopover augmentSummoningPopover;
		
		ButtonStringPopover racialHitDiePopover;
		ButtonStringPopover racialBonusPopover;
		ButtonStringPopover racialSizePopover;
		
		public event EventHandler AdvancementChanged;
		public event EventHandler AddMonsterClicked;
		
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
		
		public Monster AdvanceMonster(Monster oldMonster)
		{
			if (oldMonster == null)
			{
				return null;
			}
			
			Monster monster = (Monster)oldMonster.Clone();
			
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
            }/*
            if (OtherTemplateTabControl.SelectedItem == HalfDragonTab)
            {
                if (monster.MakeHalfDragon((string)((ComboBoxItem)HalfDragonColorCombo.SelectedValue).Content))
                {
                    monster.Name = "Half-Dragon " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == HalfFiendTab)
            {
                HashSet<Stat> bonusStats = new HashSet<Stat>();

                if (HalfFiendStrengthCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Strength);
                }
                if (HalfFiendDexterityCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Dexterity);
                }
                if (HalfFiendConstitutionCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Constitution);
                }
                if (HalfFiendIntelligenceCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Intelligence);
                }
                if (HalfFiendWisdomCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Wisdom);
                }
                if (HalfFiendCharismaCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Charisma);
                }

                if (monster.MakeHalfFiend(bonusStats))
                {
                    monster.Name = "Half-Fiend " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == HalfCelestialTab)
            {
                HashSet<Stat> bonusStats = new HashSet<Stat>();

                if (HalfCelestialStrengthCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Strength);
                }
                if (HalfCelestialDexterityCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Dexterity);
                }
                if (HalfCelestialConstitutionCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Constitution);
                }
                if (HalfCelestialIntelligenceCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Intelligence);
                }
                if (HalfCelestialWisdomCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Wisdom);
                }
                if (HalfCelestialCharismaCheck.IsChecked == true)
                {
                    bonusStats.Add(Stat.Charisma);
                }

                if (monster.MakeHalfCelestial(bonusStats))
                {
                    monster.Name = "Half-Celestial " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == SkeletonTab)
            {
                bool bloody = (BloodySkeletonCheckBox.IsChecked == true);
                bool burning = (BurningSkeletonCheckBox.IsChecked == true);
                bool champion = (SkeletalChampionCheckBox.IsChecked == true);

                if (monster.MakeSkeleton(bloody, burning, champion))
                {

                    if (champion)
                    {
                        monster.Name = "Skeletal Champion " + monster.Name;
                    }
                    else
                    {
                        monster.Name += " Skeleton";
                    }


                    if (burning)
                    {
                        monster.Name = "Burning " + monster.Name;
                    }

                    if (bloody)
                    {
                        monster.Name = "Bloody " + monster.Name;
                    }

                }
            }
            if (OtherTemplateTabControl.SelectedItem == VampireTab)
            {
                if (monster.MakeVampire())
                {
                    monster.Name = "Vampire " + monster.Name;
                }
            }
            if (OtherTemplateTabControl.SelectedItem == ZombieTab)
            {
                Monster.ZombieType zt = (Monster.ZombieType)ZombieTypeComboBox.SelectedIndex;

                if (monster.MakeZombie(zt))
                {

                    monster.Name = "Zombie " + monster.Name;

                    if (zt == Monster.ZombieType.Fast)
                    {
                        monster.Name = "Fast " + monster.Name;
                    }
                    else if (zt == Monster.ZombieType.Plague)
                    {
                        monster.Name = "Plague " + monster.Name;
                    }

                }
            }*/

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
		
		partial void AddMonsterButtonTouchUpInside (MonoTouch.Foundation.NSObject sender)
		{
			if (AddMonsterClicked != null)
			{
				AddMonsterClicked(this, new EventArgs());
			}
		}
		
		partial void AdvancerHeaderButtonTouchUpInside (MonoTouch.Foundation.NSObject sender)
		{
			
		}
		
		partial void ResetButtonTouchUpInside (MonoTouch.Foundation.NSObject sender)
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

