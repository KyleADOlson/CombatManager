
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

using Foundation;
using UIKit;

using CombatManager;

namespace CombatManagerMono
{
    public class AttackEventArgs : EventArgs
    {
        public WeaponItem Item{get; set;}
    }

    public partial class AttackItemView : UIViewController
    {
        WeaponItem _WeaponItem;

            
        ButtonStringPopover _BonusPopover;
        ButtonStringPopover _SpecialPopover;

        public event EventHandler<AttackEventArgs> DeleteClicked;

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public AttackItemView (WeaponItem item)
			: base (UserInterfaceIdiomIsPhone ? "AttackItemView_iPhone" : "AttackItemView_iPad", null)
        {
            _WeaponItem = item;
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
			
            NameLabel.Text = _WeaponItem.Name.Capitalize();

            SpecialButton.SetText(_WeaponItem.SpecialAbilities);
            SpecialButton.SetTitleColor(UIColor.White, UIControlState.Normal); 
            _SpecialPopover = new ButtonStringPopover(SpecialButton);
            _SpecialPopover.ItemClicked += HandleSpecialItemClicked;
            _SpecialPopover.WillShowPopover += HandleWillShowBonusPopover;   

            BonusButton.SetText (BonusText);
            BonusButton.SetTitleColor(UIColor.White, UIControlState.Normal); 
            _BonusPopover = new ButtonStringPopover(BonusButton);
            _BonusPopover.WillShowPopover += HandleWillShowBonus;
            _BonusPopover.ItemClicked += HandleBonusItemClicked;

            DeleteButton.Gradient = new GradientHelper(0x00000000.UIColor());
            DeleteButton.Border = 0;
            DeleteButton.SetImage(UIImage.FromFile("Images/External/redx.png"), UIControlState.Normal);
            DeleteButton.TouchUpInside += DeleteButtonClicked;

            ((GradientView)View).StyleBasicPanel();

            NameView.BackgroundColor = 0x0.UIColor();
            NameView.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDarker);

            HandView.BackgroundColor = 0x0.UIColor();
            HandView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADarker);

            HandLabel.Text = _WeaponItem.Weapon.HandsUsed + " Hand" + (_WeaponItem.Weapon.HandsUsed!=1?"s":"");

            EnableButtons();

        }

        void EnableButtons()
        {
            SpecialButton.Enabled = _WeaponItem.MagicBonus > 0;
        }

        void HandleWillShowBonusPopover (object sender, WillShowPopoverEventArgs e)
        {
            _SpecialPopover.Items.Clear();

            List<Tuple<bool, WeaponSpecialAbility>> list = new List<Tuple<bool, WeaponSpecialAbility>>();

            if (_WeaponItem.Weapon.Ranged)
            {
                foreach (WeaponSpecialAbility ability in WeaponSpecialAbility.RangedAbilities)
                {

                    bool contains = false;

                    if (_WeaponItem.SpecialAbilitySet.ContainsKey(ability.Name))
                    {
                        contains = true;
                    }
                    else if (ability.AltName != null && ability.AltName.Length > 0 &&
                        _WeaponItem.SpecialAbilitySet.ContainsKey(ability.AltName))
                    {
                        contains = true;
                    }

                    list.Add(new Tuple<bool, WeaponSpecialAbility>(contains, ability));

                }

            }
            else
            {
                foreach (WeaponSpecialAbility ability in WeaponSpecialAbility.MeleeAbilities)
                {
                    bool contains = false;

                    if (_WeaponItem.SpecialAbilitySet.ContainsKey(ability.Name))
                    {
                        contains = true;
                    }
                    else if (ability.AltName != null && ability.AltName.Length > 0 &&
                        _WeaponItem.SpecialAbilitySet.ContainsKey(ability.AltName))
                    {
                        contains = true;
                    }


 
                    list.Add(new Tuple<bool, WeaponSpecialAbility>(contains, ability));


                }
            }

            list.Sort((a, b) => a.Item2.Name.CompareTo(b.Item2.Name));

            foreach (var v in list)
            {
                _SpecialPopover.Items.Add(new ButtonStringPopoverItem() {Text=v.Item2.Name, Tag = v, 
                Icon=v.Item1?"check":null});
            }           
        }

        void HandleSpecialItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            var v = (Tuple<bool, WeaponSpecialAbility>) e.Tag;
            if (v.Item1)
            {
                _WeaponItem.SpecialAbilitySet.Remove(v.Item2.Name);
            }
            else
            {
                _WeaponItem.SpecialAbilitySet.Add (v.Item2.Name, v.Item2.Name);
            }
            _WeaponItem.SpecialAbilitySet = _WeaponItem.SpecialAbilitySet;
            SpecialButton.SetText(_WeaponItem.SpecialAbilities);
        }

        void HandleBonusItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            if (e.Tag == null)
            {
                _WeaponItem.MagicBonus = 0;
                _WeaponItem.Masterwork = true;
                _WeaponItem.SpecialAbilities = "";
            }
            else
            {
                int bonus = (int)e.Tag;
                _WeaponItem.Masterwork = false;
                _WeaponItem.MagicBonus = bonus;
                if (bonus == 0)
                {
                    _WeaponItem.SpecialAbilities = "";
                }

            }
            BonusButton.SetText (BonusText);
            SpecialButton.SetText (_WeaponItem.SpecialAbilities);
            EnableButtons();
        }

        void HandleWillShowBonus (object sender, WillShowPopoverEventArgs e)
        {
            _BonusPopover.Items.Clear();

            _BonusPopover.Items.Add(new ButtonStringPopoverItem() {Text="-", Tag=0});
            
            _BonusPopover.Items.Add(new ButtonStringPopoverItem() {Text="mwk", Tag=null});
            for (int i=1 ; i<6; i++)
            {                
                _BonusPopover.Items.Add(new ButtonStringPopoverItem() {Text=i.PlusFormat(), Tag=i});
            }
        }

        void DeleteButtonClicked (object sender, EventArgs e)
        {
            if (DeleteClicked != null)
            {
                DeleteClicked(this, new AttackEventArgs() {Item = _WeaponItem});
            }
        }

        private string BonusText
        {
            get
            {
                if (_WeaponItem.Masterwork)
                {
                    return "mwk";
                }
                else if (_WeaponItem.MagicBonus > 0)
                {
                    return _WeaponItem.MagicBonus.PlusFormat();
                }
                else
                {
                    return "-";
                }
            }
        }
		
        public override void ViewDidUnload ()
        {
            base.ViewDidUnload ();
			
            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;
			
            ReleaseDesignerOutlets ();
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

        public WeaponItem WeaponItem
        {
            get
            {
                return _WeaponItem;
            }
        }
    }
}

