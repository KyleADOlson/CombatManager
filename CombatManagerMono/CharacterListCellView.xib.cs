/*
 *  CharacterListCellView.xib.cs
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


using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CombatManager;
using System.ComponentModel;
using CoreGraphics;

namespace CombatManagerMono
{
	public partial class CharacterListCellView : UIViewController
	{
		

		private Character _Character;
		private CombatState _CombatState;
		
		private ButtonStringPopover _ActionsPopover;
		
		private ConditionViewController _ConditionView;
		
		private TextBoxDialog _TextBoxDialog;
		
		private MonsterEditorDialog _MonsterEditorDialog;
		
		private CharacterListView _CharacterListView;

        private static UIImage _IdleImage;
        private static UIImage _FollowerImage;
        private static UIImage _ActionImage;

        private static bool _ActionsPopoverShown;
		
		//private UIImageView _IdleImage;
		//private UIImageView _HiddenImage;
		
		List<GradientButton> ConditionButtons = new List<GradientButton>();
		
		static CharacterListCellView()
        {
            _IdleImage = UIExtensions.GetSmallIcon("zzz");
            _FollowerImage = UIExtensions.GetSmallIcon("lock");
            _ActionImage = UIImage.FromFile("Images/External/d20-32.png");
        }


		public CharacterListCellView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public CharacterListCellView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public CharacterListCellView ()
		{
			Foundation.NSBundle.MainBundle.LoadNib ("CharacterListCellView", this, null);
			Initialize ();
			
			
		}
		
		private void StyleButton(GradientButton button)
		{
			button.CornerRadius = 0;
			button.SetTitleColor(UIColor.White, UIControlState.Normal);
			button.SetTitleColor(UIColor.Black,UIControlState.Selected);
            button.TitleLabel.Font = UIFont.BoldSystemFontOfSize(17f);
            button.TitleLabel.AdjustsFontSizeToFitWidth = true;
            button.TitleLabel.MinimumFontSize = 14f;


			button.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark, CMUIColors.SecondaryColorADarker);
		}

		void Initialize ()
		{


			
			GradientView view = new GradientView();
			view.Border = 1f;
			view.CornerRadius = 0;
			view.Color1 = CMUIColors.PrimaryColorMedium;
			view.Color2 = CMUIColors.PrimaryColorMedium;
			view.BorderColor = CMUIColors.PrimaryColorLight;
			cellmain.BackgroundView = view;
			
			view = new GradientView();
			view.Border = 1f;
			view.CornerRadius = 0;
			view.Color1 = CMUIColors.SecondaryColorAMedium;
			view.Color2 = CMUIColors.SecondaryColorAMedium;
			view.BorderColor = CMUIColors.SecondaryColorALight;
			cellmain.SelectedBackgroundView = view;
			
			actionsButton.SetImage(_ActionImage, UIControlState.Normal);
			maxHPButton.TouchUpInside += HandleMaxHPButtonTouchUpInside;
			hpButton.TouchUpInside += HandleHpButtonTouchUpInside;
			modButton.TouchUpInside += HandleModButtonTouchUpInside;
			tempHPButton.TouchUpInside += HandleTempHPButtonTouchUpInside;
			nonlethalButton.TouchUpInside += HandleNonlethalButtonTouchUpInside;
			_ActionsPopover = new ButtonStringPopover(actionsButton);
			_ActionsPopover.WillShowPopover += Handle_ActionsPopoverWillShowPopover;
			_ActionsPopover.ItemClicked += Handle_ActionsPopoverItemClicked;
			
			nameField.TouchUpInside += HandleNameFieldhandleTouchUpInside;
			nameField.BorderColor =  UIExtensions.RGBColor(0xFFFFFF);
			nameField.SetTitleColor(UIColor.White, UIControlState.Normal);
			nameField.SetTitleColor(UIColor.LightGray, UIControlState.Highlighted);
			nameField.SetTitleColor(UIColor.White, UIControlState.Selected);
			nameField.Border = 2;
			nameField.CornerRadius = 4;
			nameField.TitleLabel.AdjustsFontSizeToFitWidth = true;
			
			StyleButton(hpButton);
			StyleButton(maxHPButton);
			StyleButton(modButton);
			StyleButton(actionsButton);
			StyleButton(nonlethalButton);
			StyleButton(tempHPButton);
			
			

		}
		public override void ViewDidLoad ()
		{
			if (_Character != null)
			{
				UpdateConditionDisplay();
			}
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear(animated);
			if (_Character != null)
			{
				UpdateConditionDisplay();
			}
		}
		
		
		void Handle_ActionsPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			CharacterActionItem item = (CharacterActionItem)e.Tag;
			
			if (item.Action != CharacterActionType.None)
			{
				CharacterActionResult res = CharacterActions.TakeAction(_CombatState, item.Action, _Character, new List<Character>() {_Character}, item.Tag);
				switch (res)
                {
                case CharacterActionResult.NeedConditionDialog:
				
					_ConditionView = new ConditionViewController();
					_ConditionView.ConditionApplied += ConditionApplied;
					MainUI.MainView.AddSubview(_ConditionView.View);
                    break;
                case CharacterActionResult.NeedNotesDialog:
				
					_TextBoxDialog = new TextBoxDialog();
					_TextBoxDialog.HeaderText = "Notes";
					_TextBoxDialog.Value = _Character.Notes;
					MainUI.MainView.AddSubview(_TextBoxDialog.View);
					_TextBoxDialog.OKClicked += Handle_NotesTextBoxDialogOKClicked;
                    break;
                case CharacterActionResult.NeedMonsterEditorDialog:
				
                    _MonsterEditorDialog = new MonsterEditorDialog(Character.Monster);
                    MainUI.MainView.AddSubview(_MonsterEditorDialog.View);
                 
                    break;
                case CharacterActionResult.RollAttack:
                    DieRollerView.Roller.RollAttack((Attack)item.Tag, _Character);
                    break;
                case CharacterActionResult.RollAttackSet:
                    DieRollerView.Roller.RollAttackSet((AttackSet)item.Tag, _Character);
                    break;
                case CharacterActionResult.RollSave:
                    DieRollerView.Roller.RollSave((Monster.SaveType)item.Tag, _Character);
                    break;
                    
                case CharacterActionResult.RollSkill:
                    var sks = (Tuple<string, string>)item.Tag;
                    DieRollerView.Roller.RollSkill(sks.Item1, sks.Item2, _Character);
                    break;
                }
			}
		}

		void Handle_NotesTextBoxDialogOKClicked (object sender, EventArgs e)
		{
			_Character.Notes = _TextBoxDialog.Value;
		}
		
		
		
		void ConditionApplied( object sender, ConditionViewEventArgs args)
		{
			Condition c = args.Condition;
			
			ActiveCondition a = new ActiveCondition();
			a.Condition = c;
			_Character.Stats.AddCondition(a);
			Condition.PushRecentCondition(a.Condition);
		}
		
		void Handle_ActionsPopoverWillShowPopover (object sender, EventArgs e)
		{
            UIAlertView view = null;
			if (!_ActionsPopoverShown)
            {
                 view = new UIAlertView("Loading", "Accessing Database...", new UIAlertViewDelegate(), null, new string[]{});
                view.Show();
                NSRunLoop.Current.RunUntil(NSDate.Now.AddSeconds(.1));
                //show waiting dialog
            }

			UIWebView v = new UIWebView(new CGRect(0, 0, 180, 120));
			v.LoadHtmlString(MonsterHtmlCreator.CreateHtml(_Character.Monster, _Character, true), new NSUrl("http://localhost/")); 
			_ActionsPopover.AccessoryView = v;

			List<CharacterActionItem> actions = CharacterActions.GetActions(_Character, _CharacterListView.SelectedCharacter);
			
            if (!_ActionsPopoverShown)
            {
                view.DismissWithClickedButtonIndex(0, false);
                //hide waiting dialog
                _ActionsPopoverShown = true;
            }
			AddActionItems(actions, _ActionsPopover.Items);
			


		}


		void AddActionItems(List<CharacterActionItem> actionList, List<ButtonStringPopoverItem> items)
		{
			
			items.Clear();
			
			foreach (CharacterActionItem it in actionList)
			{
				ButtonStringPopoverItem p = new ButtonStringPopoverItem();
				p.Text = it.Name;
				p.Tag = it;
				p.Icon = it.Icon;
				items.Add(p);
				
				if (it.SubItems != null)
				{
					AddActionItems(it.SubItems, p.Subitems);
				}

                if (it.Action == CharacterActionType.None && it.SubItems == null)
                {
                    p.Disabled = true;
                }
			}
		}

		void HandleModButtonTouchUpInside (object sender, EventArgs e)
		{
			NumberModifyPopover pop = new NumberModifyPopover();
			pop.ShowOnView((UIView)sender);
			pop.Value = Character.Monster.Init;
			pop.ValueType = "Init";
			pop.ValueFormat = "Init Mod: {0}";
			pop.NumberModified += HandleInitPopNumberModified;
		}

		void HandleInitPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			
			if (args.Set)
			{
				Character.Monster.Init = args.Value.Value;
			}
			else
			{
				Character.Monster.Init += args.Value.Value;
			}
			
			pop.Value = Character.Monster.Init;
		}
			

		void HandleHpButtonTouchUpInside (object sender, EventArgs e)
		{
			NumberModifyPopover pop = new NumberModifyPopover();
			pop.ShowOnView((UIView)sender);
			pop.Value = Character.HP;
			pop.ValueType = "HP";
			pop.ValueFormat = "HP: {0}";
			pop.NumberModified += HandleHPPopNumberModified;
		}

		void HandleHPPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			
			if (args.Set)
			{
				int change = args.Value.Value - Character.HP;
				Character.AdjustHP(change);
			}
			else
			{
			
				Character.AdjustHP(args.Value.Value);
			}
			
			pop.Value = Character.HP;
		}

		void HandleMaxHPButtonTouchUpInside (object sender, EventArgs e)
		{
			
			NumberModifyPopover pop = new NumberModifyPopover();
			pop.ShowOnView((UIView)sender);
			pop.Value = Character.MaxHP;
			pop.ValueType = "MaxHP";
			pop.ValueFormat = "Max HP: {0}";
			pop.NumberModified += HandleMaxHPPopNumberModified;
		}
	
	

		void HandleMaxHPPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			
			if (args.Set)
			{
				Character.MaxHP = args.Value.Value;
			}
			else
			{
				Character.MaxHP += args.Value.Value;
			}
			
			pop.Value = Character.MaxHP;
		}
		
		

		void HandleNonlethalButtonTouchUpInside (object sender, EventArgs e)
		{
			NumberModifyPopover pop = new NumberModifyPopover();
			pop.ShowOnView((UIView)sender);
			pop.Value = Character.NonlethalDamage;
			pop.ValueType = "Nonlethal";
			pop.ValueFormat = "Nonlethal: {0}";
			pop.NumberModified += HandleNonlethalPopNumberModified;
		}

		void HandleNonlethalPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			
			if (args.Set)
			{
				int change = args.Value.Value - Character.NonlethalDamage;
				Character.AdjustHP(0, change, 0);
			}
			else
			{
			
				Character.AdjustHP(0, args.Value.Value, 0);
			}
			
			pop.Value = Character.NonlethalDamage;
		}

		void HandleTempHPButtonTouchUpInside (object sender, EventArgs e)
		{
			NumberModifyPopover pop = new NumberModifyPopover();
			pop.ShowOnView((UIView)sender);
			pop.Value = Character.TemporaryHP;
			pop.ValueType = "TempHP";
			pop.ValueFormat = "Temporary HP: {0}";
			pop.NumberModified += HandleTempHPPopNumberModified;
		}

		void HandleTempHPPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			
			if (args.Set)
			{
				int change = args.Value.Value - Character.TemporaryHP;
				Character.AdjustHP(0, 0, change);
			}
			else
			{
			
				Character.AdjustHP(0, 0, args.Value.Value);
			}
			
			pop.Value = Character.TemporaryHP;
		}
	

		void HandleCloseButtonTouchUpInside (object sender, EventArgs e)
		{
			_CombatState.RemoveCharacter(_Character);
		}
		

		void HandleActionsButtonTouchUpInside (object sender, EventArgs e)
		{
			
		}
		public CombatState CombatState
		{
			get
			{
				return _CombatState;
			}
			set
			{
				_CombatState = value;
			}
		}
			
		
		public Character Character
		{
			get
			{
				return _Character;
				
			}
			set
			{
				if (_Character != value)
				{
					if (_Character != null)
					{
						_Character.PropertyChanged -= Handle_CharacterPropertyChanged;
						_Character.Monster.PropertyChanged -= Handle_CharacterMonsterPropertyChanged;
						_Character.Monster.ActiveConditions.CollectionChanged -= Handle_CharacterMonsterActiveConditionsCollectionChanged;
					}
					
					
					_Character = value;
					nameField.SetText(_Character.Name);
					UpdateHP();
					UpdateMaxHP();
					UpdateMod();
					UpdateNonlethal();
					UpdateTempHP();
					UpdateConditionDisplay();
                    UpdateIndicatorImage();
					
					_Character.PropertyChanged += Handle_CharacterPropertyChanged;
					_Character.Monster.PropertyChanged += Handle_CharacterMonsterPropertyChanged;
					_Character.Monster.ActiveConditions.CollectionChanged += Handle_CharacterMonsterActiveConditionsCollectionChanged;
				
				}
				
			}
		}
		
		

		void HandleNameFieldhandleTouchUpInside (object sender, EventArgs e)
		{
			_TextBoxDialog = new TextBoxDialog();
			_TextBoxDialog.HeaderText = "Name";
			_TextBoxDialog.Value = _Character.Name;
			_TextBoxDialog.SingleLine = true;
			_TextBoxDialog.OKClicked += delegate(object s, EventArgs ex) 
			
			{
				_Character.Name = _TextBoxDialog.Value;
			};
			
			MainUI.MainView.AddSubview(_TextBoxDialog.View);
			
		}

		

		void Handle_CharacterMonsterActiveConditionsCollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (View.Superview != null)
			{
				
				UpdateConditionDisplay();

                UIView v = this.View.Superview;

                if (!(v is UITableView))
                {
                    v = v.Superview;
                }
               
                ((UITableView)v).ReloadData();
			}
			else
			{					
				DisconnectCharacter();
			}
		}

		void Handle_CharacterMonsterPropertyChanged (object sender, PropertyChangedEventArgs e)
		{	
			if (View.Superview != null)
			{
				
				if (e.PropertyName == "Init")
				{
					
					UpdateMod();
				}
			}
			else
			{
				DisconnectCharacter();
			}
		}
		
		void DisconnectCharacter()
		{
			_Character.PropertyChanged -= Handle_CharacterPropertyChanged;
			_Character.Monster.PropertyChanged -= Handle_CharacterMonsterPropertyChanged;
			_Character.Monster.ActiveConditions.CollectionChanged -= Handle_CharacterMonsterActiveConditionsCollectionChanged;

		}

		void Handle_CharacterPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (View.Superview != null)
			{
			
				if (e.PropertyName == "Name")
				{
					nameField.SetText(_Character.Name);
				}
				else if (e.PropertyName == "HP")
				{
					UpdateHP();
				}
				else if (e.PropertyName == "MaxHP")
				{
					UpdateMaxHP();
				}
				else if (e.PropertyName == "NonlethalDamage")
				{
					UpdateNonlethal();
				}
				else if (e.PropertyName == "TemporaryHP")
				{
					UpdateTempHP();
				}
				else if (e.PropertyName == "IsIdle" || e.PropertyName == "IsHidden")
				{
					UpdateIndicatorImage();
				}
                else if (e.PropertyName == "InitiativeLeader")
                {
                    UpdateIndicatorImage();
                }
				
			}
			else
			{
				DisconnectCharacter();
			}
		}
		
		void UpdateHP()
		{
			
			hpButton.SetText("HP " + _Character.HP.ToString());
		}
		
		void UpdateMaxHP()
		{
			
			maxHPButton.SetText("/ " + _Character.MaxHP.ToString());
		}
		
		void UpdateMod()
		{
		
			modButton.SetText("Init " + _Character.Monster.Init.ToString());	
		}
		void UpdateNonlethal()
		{
			
			string damage = _Character.NonlethalDamage.ToString();
			
			if (damage == "0")
			{
				damage = "-";
			}
			
			nonlethalButton.SetText("NL " + damage);	
		}
		void UpdateTempHP()
		{
			string damage = _Character.TemporaryHP.ToString();
			
			if (damage == "0")
			{
				damage = "-";
			}
			
		
			tempHPButton.SetText("T " + damage);	
		}
		
		
		void UpdateIndicatorImage()
		{
            if (_Character.InitiativeLeader != null)
            {
                IndicatorView.Image = _FollowerImage;
            }
            else if (_Character.IsIdle)
            {
                IndicatorView.Image = _IdleImage;
            }
            else
            {
                IndicatorView.Image = null;
            }
		}
		
		static int ConditionWidth = 30;
		static int ConditionHeight = 30;
		static int ConditionMargin = 3;
		
		
		void UpdateConditionDisplay()
		{
			if (View != null)
			{
				foreach (GradientButton b in ConditionButtons)
				{
					b.RemoveFromSuperview();	
				}
				
				
				ConditionButtons.Clear();
				
				float xLoc = ConditionMargin;
				float yLoc = 79 + ConditionMargin;
				foreach (ActiveCondition c in _Character.Monster.ActiveConditions)
				{
					GradientButton b = new GradientButton();
					
					b.Frame = new CGRect(xLoc, yLoc, ConditionWidth, ConditionHeight);
					b.SetImage(UIExtensions.GetSmallIcon(c.Condition.Image), UIControlState.Normal);
					View.AddSubview(b);
					if (c.Turns != null)
					{
						b.SetText(c.Turns.ToString());	
					}
					b.Data = c;
					c.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) 
					{
						if (e.PropertyName == "Turns")
						{
							string text = "";
							if (c.Turns != null)
							{
								text = c.Turns.ToString();
							}
							b.SetText(text);
							b.TitleLabel.AdjustsFontSizeToFitWidth = true;
						}
					};
					
					ButtonStringPopover p = new ButtonStringPopover(b);
					BuildConditionMenu(p, c);
					
					xLoc += ConditionWidth + ConditionMargin;
					
					if (xLoc + ConditionWidth > View.Bounds.Width)
					{
						xLoc = ConditionMargin;
						yLoc += ConditionHeight + ConditionMargin;
					}
					UIWebView v = new UIWebView(new CGRect(0, 0, 300, 300));
					v.LoadHtmlString(ConditionViewController.ConditionHTML(c.Condition), new NSUrl("http://localhost/")); 
					p.AccessoryView = v;
						
				}
			}
		}

		private void BuildConditionMenu(ButtonStringPopover p, ActiveCondition c)
		{
			p.Data = c;
			p.Items.Add(new ButtonStringPopoverItem() {Text = "Add 5 Turns", Icon="arrowsup", Tag=5});
			p.Items.Add(new ButtonStringPopoverItem() {Text = "Add Turn", Icon="arrowup", Tag=1});
			p.Items.Add(new ButtonStringPopoverItem() {Text = "Remove Turn", Icon="arrowdown", Tag=-1});
			p.Items.Add(new ButtonStringPopoverItem() {Text = "Remove 5 Turns", Icon="arrowsdown", Tag=-5});
			p.Items.Add(new ButtonStringPopoverItem());
			p.Items.Add(new ButtonStringPopoverItem() {Text = "Delete", Icon="delete", Tag="delete"});
			p.Items.Add(new ButtonStringPopoverItem() {Text = "Delete From All Characters", Tag="deleteall"});
			p.ItemClicked += HandleConditionMenuItemClicked;
		}

		void HandleConditionMenuItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			ButtonStringPopover p = (ButtonStringPopover)sender;
			ActiveCondition ac =  (ActiveCondition)p.Data;
			if (e.Tag is int)
			{
				int turns = (int)e.Tag;
				
				if (turns > 0)
				{
					_CombatState.AddConditionTurns(_Character, ac, turns);
				}
				else
				{
					_CombatState.RemoveConditionTurns(_Character, ac, -turns);
				}

			}
			else if (e.Tag is string)
			{
				string text = (string)e.Tag;
				if (text == "delete")
				{
					_Character.Monster.RemoveCondition(ac);
				}
				else if (text == "deleteall")
				{
					foreach (Character ch in _CombatState.Characters)
	                {
	                    ch.RemoveConditionByName(ac.Condition.Name);
	                }
				}
			}
		}
		
		public UITableViewCell Cell
		{
			get { return this.cellmain; }
		}
		
		
		public static float ConditionViewHeight(Character ch, double width)
		{
			
			
			if (ch.Monster.ActiveConditions.Count > 0)
			{
				float itemWidth = ConditionWidth + ConditionMargin;
				
				int count = (int)((width - ConditionMargin)/itemWidth);
				
				int rows = (ch.Monster.ActiveConditions.Count -1)/count + 1;
				
				return ConditionMargin + rows * (ConditionHeight + ConditionMargin);
			}
			else
			{
				return 0;
			}
		}
		
		public CharacterListView CharacterListView
		{
			get
			{
				return _CharacterListView;
			}
			set
			{
				_CharacterListView = value;
			
			}
		}
		
		
	}
}

