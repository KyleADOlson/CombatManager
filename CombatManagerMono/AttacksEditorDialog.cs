
using System;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CombatManager;
using System.Collections.ObjectModel;

namespace CombatManagerMono
{
    public partial class AttacksEditorDialog : StandardDialogView
    {
        Monster _Monster;
        CharacterAttacks _Attacks;

        GradientView _SetChoiceView;
        UIScrollView _ScrollView;
        GradientView _SetControlView;

        TypeTab _SelectedTab;

        List<AttackItemView> _AttackItems = new List<AttackItemView>();
        List<NaturalAttackItemView> _NaturalAttackItems = new List<NaturalAttackItemView>();

        GradientButton _AddAttackButton;
        GradientButton _AddSetButton;
        GradientButton _DeleteSetButton;

        GradientButton _NextSetButton;
        GradientButton _LastSetButton;
        UILabel _SetLabel;

        ButtonStringPopover _AddAttackPopover;

        enum TypeTab
        {
            MeleeTab,
            RangedTab,
            NaturalTab
        }

        int _SelectedSet;

        List<WeaponItem> _VisibleSet;
        ObservableCollection<WeaponItem> _RangedSet;



        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public AttacksEditorDialog (Monster m)
			: base (UserInterfaceIdiomIsPhone ? "AttacksEditorDialog_iPhone" : "AttacksEditorDialog_iPad", null)
        {
            _Monster = m;
            _Attacks = new CharacterAttacks(new ObservableCollection<AttackSet>(_Monster.MeleeAttacks),
                                            new ObservableCollection<Attack>(_Monster.RangedAttacks));



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
			
            StyleButton(OKButton);
            StyleButton(CancelButton);

            StylePanel1(this.AttackTextView);
            StyleBackground(BackgroundView);

            CMStyles.StyleBasicPanel(EditingView);

            MeleeView.Editable = false;
            MeleeView.Text = _Monster.MeleeString(_Attacks);
            RangedView.Editable = false;
            RangedView.Text = _Monster.RangedString(_Attacks);

            _ScrollView = new UIScrollView();
            _SetChoiceView = new GradientView();
            _SetControlView = new GradientView();

            _NextSetButton = new GradientButton();
            _NextSetButton.SetImage(UIExtensions.GetSmallIcon("next"), UIControlState.Normal);
            _NextSetButton.TouchUpInside += HandleNextSetButtonClicked;
            _LastSetButton = new GradientButton();
            _LastSetButton.SetImage(UIExtensions.GetSmallIcon("prev"), UIControlState.Normal);
            _LastSetButton.TouchUpInside += HandleLastSetButtonClicked;

            _SetLabel = new UILabel();
            _SetLabel.Font = UIFont.BoldSystemFontOfSize(16);
            _SetLabel.BackgroundColor = 0x00000000.UIColor();
            _SetLabel.TextColor = 0xFFFFFFFF.UIColor();
            _SetLabel.TextAlignment = UITextAlignment.Center;

            _SetChoiceView.Add(_NextSetButton);
            _SetChoiceView.Add (_LastSetButton);
            _SetChoiceView.Add (_SetLabel);


            _AddAttackButton = new GradientButton();
            _AddAttackButton.SetText("Add Attack");
            StyleButton(_AddAttackButton);
            
            _AddSetButton = new GradientButton();
            _AddSetButton.SetText("Add");
            StyleButton(_AddSetButton);
            _SetChoiceView.Add (_AddSetButton);


            _DeleteSetButton = new GradientButton();
            _DeleteSetButton.SetText("Delete");
            StyleButton(_DeleteSetButton);
            _SetChoiceView.Add (_DeleteSetButton);


            MeleeButton.Data = TypeTab.MeleeTab;
            RangedButton.Data = TypeTab.RangedTab;
            NaturalButton.Data = TypeTab.NaturalTab;

            foreach (var v in new GradientButton[] {MeleeButton, RangedButton, NaturalButton})
            {
                
                v.TouchUpInside += HandleTypeButtonClicked;
                v.StyleTab(false);
                v.Border = 2;
            }


            EditingView.Add (_ScrollView);
            EditingView.Add (_SetChoiceView);
            EditingView.Add (_SetControlView);
            
            _SetControlView.Add (_AddAttackButton);



            
            CMStyles.StyleBasicPanel(_SetChoiceView);
            CMStyles.StyleBasicPanel(_SetControlView);

            _SetChoiceView.CornerRadii = new float[] 
                {_SetChoiceView.CornerRadius, _SetChoiceView.CornerRadius, 0, 0};
            _SetControlView.CornerRadii = new float[] 
                {0, 0, _SetChoiceView.CornerRadius, _SetChoiceView.CornerRadius};

            
            _AddSetButton.TouchUpInside += HandleAddSetButtonClicked;
            _DeleteSetButton.TouchUpInside += HandleDeleteSetClicked;




            _AddAttackPopover = new ButtonStringPopover(_AddAttackButton);
            _AddAttackPopover.WillShowPopover += HandleWillShowAddAttacksPopover;
            _AddAttackPopover.ItemClicked += HandleAddAttackItemClicked;

            _SelectedTab = TypeTab.MeleeTab;
            MeleeButton.StyleTab(true);

            SetupMeleeTab();
            SetupAttackViews();


        }

        void HandleAddSetButtonClicked (object sender, EventArgs e)
        {
            var v = new List<WeaponItem>();
            _Attacks.MeleeWeaponSets.Add(v);
            _SelectedSet = _Attacks.MeleeWeaponSets.Count -1;
            _VisibleSet = v;
            PrepareAttackItems();
        }

        

        void HandleDeleteSetClicked (object sender, EventArgs e)
        {
            if (_VisibleSet != null)
            {
                _Attacks.MeleeWeaponSets.Remove(_VisibleSet);
                _SelectedSet = Math.Min(_Attacks.MeleeWeaponSets.Count - 1, _SelectedSet);
                if (_SelectedSet != -1)
                {
                    _VisibleSet = _Attacks.MeleeWeaponSets[_SelectedSet];
                }
                else
                {
                    _VisibleSet = null;
                }

            }
        }

        void HandleNextSetButtonClicked (object sender, EventArgs e)
        {
            if (_SelectedSet < _Attacks.MeleeWeaponSets.Count - 1 && _Attacks.MeleeWeaponSets.Count > 0)
            {
                _SelectedSet++;
                _VisibleSet = _Attacks.MeleeWeaponSets[_SelectedSet];
                PrepareAttackItems();
            }
        }

        void HandleLastSetButtonClicked (object sender, EventArgs e)
        {
            if (_SelectedSet > 0 && _Attacks.MeleeWeaponSets.Count > 0)
            {
                _SelectedSet--;
                _VisibleSet = _Attacks.MeleeWeaponSets[_SelectedSet];
                PrepareAttackItems();
            }
        }


        void HandleAddAttackItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
        {
            Weapon w = (Weapon)e.Tag;

            WeaponItem item = new WeaponItem(w);


            if (SetCount() == 0)
            {
                item.MainHand = true;
            }
            AddSetItem (item);
            UpdateAttackStrings();
            PrepareAttackItems();

        }

        int SetCount()
        {
            int count = 0;

            if (_SelectedTab == TypeTab.MeleeTab)
            {
                count = _VisibleSet.Count;
            }
            else if (_SelectedTab == TypeTab.RangedTab)
            {
                count = _RangedSet.Count;
            }

            return count;
        }

        void AddSetItem(WeaponItem w)
        {
             if (_SelectedTab == TypeTab.MeleeTab)
            {
                _VisibleSet.Add (w);
            }
            else if (_SelectedTab == TypeTab.RangedTab)
            {
                _RangedSet.Add (w);
            }
        }

        IEnumerable<WeaponItem> EnumerableSet()
        {
            if (_SelectedTab == TypeTab.MeleeTab)
            {
                return _VisibleSet;
            }
            else if (_SelectedTab == TypeTab.RangedTab)
            {
                return _RangedSet;
            }

            return null;
        }

        
        void RemoveSetItem(WeaponItem w)
        {
             if (_SelectedTab == TypeTab.MeleeTab)
            {
                _VisibleSet.Remove (w);
            }
            else if (_SelectedTab == TypeTab.RangedTab)
            {
                _RangedSet.Remove (w);
            }
        }

        void HandleWillShowAddAttacksPopover (object sender, WillShowPopoverEventArgs e)
        {

            
            bool ranged = _SelectedTab == TypeTab.RangedTab;
            bool melee = _SelectedTab == TypeTab.MeleeTab;


            int handsAvailable = melee?_Attacks.Hands - CountHands(_VisibleSet):0;



            _AddAttackPopover.Items.Clear();
            foreach (Weapon w in from x in Weapon.Weapons.Values 
                     where (x.HandsUsed <= handsAvailable || ranged) && x.Ranged == ranged 
                     orderby x.Name select x)
            {
                _AddAttackPopover.Items.Add (new ButtonStringPopoverItem() {Text = (w.Name + " " + w.DamageText).Capitalize(), Tag = w});
            }
        }

        void EnableButtons()
        {
            if (_SelectedTab == TypeTab.MeleeTab)
            {
                _AddAttackButton.Enabled = _VisibleSet != null && CountHands(_VisibleSet) < _Attacks.Hands;
            }
            else
            {
                _AddAttackButton.Enabled = true;
            }
        }

        void HandleTypeButtonClicked (object sender, EventArgs e)
        {
            TypeTab newTab = (TypeTab)((GradientButton)sender).Data;

            if (newTab != _SelectedTab)
            {
                _SelectedTab = newTab;

                switch (_SelectedTab)
                {
                case TypeTab.MeleeTab:
                    SetupMeleeTab();
                    break;
                case TypeTab.RangedTab:
                    SetupRangedTab();
                    break;
                }

                SetupAttackViews();

                MeleeButton.StyleTab(_SelectedTab == TypeTab.MeleeTab);
                RangedButton.StyleTab(_SelectedTab == TypeTab.RangedTab);
                NaturalButton.StyleTab(_SelectedTab == TypeTab.NaturalTab);
            }
        }

        void SetupMeleeTab()
        {
            if (_Attacks.MeleeWeaponSets.Count == 0)
            {
                _VisibleSet = null;
                _SelectedSet = -1;
            }
            else
            {
                _SelectedSet = 0;
                _VisibleSet = _Attacks.MeleeWeaponSets[0];
            }

        }

        void SetupRangedTab()
        {
            _RangedSet = _Attacks.RangedWeapons;
        }



        int CountHands(IEnumerable<WeaponItem> items)
        {
            int count = 0;
            if (items != null)
            {
                foreach (WeaponItem i in items)
                {
                    count += i.Weapon.HandsUsed;
                }
            }
            return count;

        }

        
        void HandleDeleteClicked (object sender, AttackEventArgs e)
        {
            AttackItemView view = (AttackItemView)sender;

            RemoveSetItem(view.WeaponItem);
            PrepareAttackItems();
            UpdateAttackStrings();


        }

        void UpdateAttackStrings()
        {
            MeleeView.Text = _Monster.MeleeString(_Attacks);
            RangedView.Text = _Monster.RangedString(_Attacks);
        }

        private void SetupAttackViews()
        {
            
            CGRect rect = EditingView.Bounds;
            rect.Location = new CGPoint(0, 0);

            float topScrollMargin = 0;
            float bottomScrollMargin = 0;
            if (_SelectedTab == TypeTab.MeleeTab || _SelectedTab == TypeTab.RangedTab)
            {
                _SetControlView.Hidden = false;

                bottomScrollMargin = 40;
                if (_SelectedTab != TypeTab.RangedTab)
                {
                    topScrollMargin = 40;
                    _SetChoiceView.Hidden = false;
                    _DeleteSetButton.Hidden = false;
                    
                    _SetLabel.Text = Math.Max(_SelectedSet + 1, 0) + "/" + _Attacks.MeleeWeaponSets.Count;
                }
                else
                {
                    _SetChoiceView.Hidden = true;
                    _DeleteSetButton.Hidden = true;
                }

                CGRect viewRect = rect;
                viewRect.Height = bottomScrollMargin;
                _SetChoiceView.Frame = viewRect;

                viewRect.Y = rect.Bottom - bottomScrollMargin;
                _SetControlView.Frame = viewRect;

                CGRect buttonRect = viewRect.Size.OriginRect();
                buttonRect.Width *= .5f;
                buttonRect.Inflate(-3, -3);
                _AddAttackButton.Frame = buttonRect;

                buttonRect.X = buttonRect.Right + 5;
                buttonRect.Width = (_SetControlView.Bounds.Width - buttonRect.X)/2.0f - 10.0f;
               
                _AddSetButton.Frame = buttonRect;

                buttonRect.X += buttonRect.Width + 5.0f;
                _DeleteSetButton.Frame = buttonRect;

                buttonRect.X = 3;
                buttonRect.Width = 40;

                _LastSetButton.Frame = buttonRect;
                buttonRect.X += buttonRect.Width;
                buttonRect.Width = 110;
                _SetLabel.Frame = buttonRect;
                buttonRect.X += buttonRect.Width;
                buttonRect.Width = 40;
                _NextSetButton.Frame = buttonRect;



            }
            else
            {
                
                _SetChoiceView.Hidden = true;
                _SetControlView.Hidden = true;

            }

            rect.Y += topScrollMargin;
            rect.Height -= bottomScrollMargin + topScrollMargin;

            _ScrollView.Frame = rect;


            PrepareAttackItems();            
            EnableButtons();
        }

        float _AttackMargin = 10f;

        void PrepareAttackItems ()
        {
            foreach (var v in _AttackItems)
            {
                v.View.RemoveFromSuperview();
                v.WeaponItem.PropertyChanged -= HandleWeaponItemChanged;
            }
            _AttackItems.Clear();

            foreach (var v in _NaturalAttackItems)
            {                
                v.View.RemoveFromSuperview();
                v.DeleteClicked -= HandleNaturalDeleteClicked;
                v.ItemChanged -= HandledNaturalItemChanged;
               
            }
            _NaturalAttackItems.Clear();

            
           float y = _AttackMargin;
            if (_SelectedTab != TypeTab.NaturalTab)
            {
                if (EnumerableSet() != null)
                {

                    foreach (WeaponItem a in EnumerableSet())
                    {
                        AttackItemView v = new AttackItemView(a);
                        _ScrollView.Add (v.View);
                        v.View.SetLocation(_AttackMargin, y);
                        float height = (float)v.View.Bounds.Height;
                        y+= height + _AttackMargin;
                        v.DeleteClicked += HandleDeleteClicked;
                        _AttackItems.Add(v);

                        a.PropertyChanged += HandleWeaponItemChanged;
                    }
                }


                
                if (_SelectedTab == TypeTab.MeleeTab)
                {

                    _SetLabel.Text = Math.Max(_SelectedSet + 1, 0) + "/" + _Attacks.MeleeWeaponSets.Count;
                }
            }
            else
            {
                foreach (WeaponItem w in _Attacks.NaturalAttacks)
                {

                    NaturalAttackItemView v = new NaturalAttackItemView(w, _Monster);
                    _ScrollView.Add (v.View);
                    v.View.SetLocation(_AttackMargin, y);
                    float height = (float)v.View.Bounds.Height;
                    y+= height + _AttackMargin;
                    v.DeleteClicked += HandleNaturalDeleteClicked;
                    v.ItemChanged += HandledNaturalItemChanged;
                    _NaturalAttackItems.Add(v);
                }
            }

            
            _ScrollView.ContentSize = new CGSize(_ScrollView.Bounds.Width, y);

            EnableButtons();
        }

        void HandleWeaponItemChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateAttackStrings();
        }

        void HandledNaturalItemChanged (object sender, EventArgs e)
        {
            
            UpdateAttackStrings();
        }

        public void HandleNaturalDeleteClicked(object sender, EventArgs e)
        {
            var v = (NaturalAttackItemView)sender;
            var atk = v.Item;

            _Attacks.NaturalAttacks.Remove(atk);

            PrepareAttackItems();
            UpdateAttackStrings();
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

            HandleOK();
        }

        partial void CancelButtonClicked (NSObject sender)
        {
            
            HandleCancel();
        }

        public CharacterAttacks Attacks
        {
            get
            {
                return _Attacks;
            }
            set
            {
                _Attacks = value;
            }
        }

    }
}

