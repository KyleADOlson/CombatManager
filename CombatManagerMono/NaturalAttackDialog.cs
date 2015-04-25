
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;

using Foundation;
using UIKit;

using CombatManager;

namespace CombatManagerMono
{
    public partial class NaturalAttackDialog : StandardDialogView
    {
        WeaponItem _WeaponItem;
        WeaponItem _OriginalItem;
        Monster _Monster;

        ButtonPropertyManager _PlusManager;
        ButtonPropertyManager _CountManager;
        ButtonStringPopover _DamagePopover;
        ButtonStringPopover _AttackPopover;

        List<ButtonPropertyManager> _Managers = new List<ButtonPropertyManager>();

        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public NaturalAttackDialog (WeaponItem item, Monster monster)
			: base (UserInterfaceIdiomIsPhone ? "NaturalAttackDialog_iPhone" : "NaturalAttackDialog_iPad", null)
        {
            _WeaponItem = (WeaponItem)item.Clone();
            _OriginalItem = item;
            _Monster = monster;
        }

		
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
			

        }
		
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            StyleBackground(BackgroundView);
            StyleHeader(HeaderView, HeaderLabel);
            HeaderText = "Natural Attack";
            StyleButton(PlusButton);
            StyleButton(CountButton);
            StyleButton(DamageButton);
            StyleButton(AttackButton);
            StyleButton(OKButton);
            StyleButton (CancelButton);

            _PlusManager = new ButtonPropertyManager(PlusButton,  MainUI.MainView, _WeaponItem, "Plus");
            _CountManager = new ButtonPropertyManager(CountButton, MainUI.MainView, _WeaponItem, "Count");
            var countList = new List<KeyValuePair<object, string>>();
            for (int i=1; i<20; i++)
            {
                countList.Add(new KeyValuePair<object, string>(i, i.ToString()));
            }
            _CountManager.ValueList = countList;
            _DamagePopover = new ButtonStringPopover(DamageButton);
            _DamagePopover.WillShowPopover += HandleWillShowDamagePopover;
            _DamagePopover.ItemClicked += HandleDamageItemClicked;

            _AttackPopover = new ButtonStringPopover(AttackButton);
            _AttackPopover.WillShowPopover += HandleWillShowAttackPopover;
            _AttackPopover.ItemClicked += HandleAttackItemClicked;

            _Managers.Add(_PlusManager);
            _Managers.Add (_CountManager);

            UpdateWeaponDamageText();
        }

        void UpdateWeaponDamageText()
        {
            AttackButton.SetText(_WeaponItem.Weapon.Name);

            DamageButton.SetText(_WeaponItem.Step.Text);
        }

        void HandleDamageItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            DieStep die = (DieStep)e.Tag;
            
            _WeaponItem.Step = new DieStep(die.Count, die.Die);

            DieStep step = DieRoll.StepDie(_WeaponItem.Weapon.DamageDie.Step, 
                                           -SizeMods.StepsFromMedium(SizeMods.GetSize(_Monster.Size)));


            _WeaponItem.Weapon.DmgM = step.Text;
            _WeaponItem.Weapon.DmgS = DieRoll.StepDie(step, -1).Text;
            UpdateWeaponDamageText();




        }

        void HandleAttackItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            Weapon w = (Weapon)e.Tag;
            _WeaponItem.Weapon = (Weapon)w.Clone();
            UpdateWeaponDamageText();

        }

        void HandleWillShowDamagePopover (object sender, WillShowPopoverEventArgs e)
        {
            if (_DamagePopover.Items.Count == 0)
            {
                DieStep die = new DieStep(0, 1);
                for (int i=0; i<11; i++)
                {
                    _DamagePopover.Items.Add(new ButtonStringPopoverItem(){Text=die.Text, Tag=die});
                    DieRoll roll = new DieRoll(die.Count, die.Die, 0);
                    roll = DieRoll.StepDie(roll, 1);
                    die = roll.Step;
                }
            }
        }

        void HandleWillShowAttackPopover (object sender, WillShowPopoverEventArgs e)
        {
            if (_AttackPopover.Items.Count == 0)
            {
                foreach (var r in from w in Weapon.Weapons where w.Value.Natural orderby w.Value.Name ascending select w.Value)
                {
                    _AttackPopover.Items.Add(new ButtonStringPopoverItem(){Text=r.Name, Tag=r});
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

        partial void OKButtonClicked (NSObject sender)
        {
            _OriginalItem.Weapon = (Weapon)_WeaponItem.Weapon.Clone();
            _OriginalItem.Count = _WeaponItem.Count;
            _OriginalItem.MagicBonus = _WeaponItem.MagicBonus;
            _OriginalItem.Masterwork = _WeaponItem.Masterwork;
            _OriginalItem.Broken = _WeaponItem.Broken;
            _OriginalItem.SpecialAbilities = _WeaponItem.SpecialAbilities;
            _OriginalItem.MainHand = _WeaponItem.MainHand;
            _OriginalItem.Plus = _WeaponItem.Plus;
            _OriginalItem.Step = _WeaponItem.Step;

            HandleOK();
        }

        partial void CancelButtonClicked (NSObject sender)
        {
            HandleCancel();
        }
    }
}

