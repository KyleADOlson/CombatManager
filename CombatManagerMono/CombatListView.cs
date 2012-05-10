/*
 *  CombatListView.cs
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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CombatManager;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;

namespace CombatManagerMono
{
	public class CombatListView : UIView
	{
		CombatState _combatState;
		ViewDataSource viewDataSource;
		ViewDelegate viewDelegate;
		UITableView listView;
		
		GradientButton nextButton;
		GradientButton prevButton;
		GradientButton moveUpButton;
		GradientButton moveDownButton;
		GradientButton rollButton;
		GradientButton sortButton;
		
		GradientView currentCharacterView;
		UILabel currentCharacterLabel;
		GradientView roundView;
		UILabel roundLabel;
		
		Character _SelectedCharacter;
		
		
		UIFont _Font;
	 	UIFont _FontBold;
		
		public CombatListView ()
		{
			_Font = UIFont.SystemFontOfSize(14);
			_FontBold = UIFont.BoldSystemFontOfSize(14);
		
			listView = new UITableView();
			AddSubview(listView);
			
			currentCharacterView = new GradientView();
			AddSubview(currentCharacterView);
			currentCharacterLabel = new UILabel();
			currentCharacterLabel.TextAlignment = UITextAlignment.Center;
			currentCharacterLabel.BackgroundColor = UIExtensions.ARGBColor(0x0);
			currentCharacterLabel.TextColor = UIColor.White;
			currentCharacterLabel.AdjustsFontSizeToFitWidth = true;
			currentCharacterView.AddSubview(currentCharacterLabel);
			currentCharacterView.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDark);
			currentCharacterView.BorderColor = UIColor.Gray;
			currentCharacterView.CornerRadius = 0;
			
			roundView = new GradientView();
			AddSubview(roundView);
			roundLabel = new UILabel();
			roundLabel.BackgroundColor = UIExtensions.ARGBColor(0x0);
			roundLabel.TextAlignment = UITextAlignment.Center;
			roundLabel.TextColor = UIColor.White;
			currentCharacterLabel.AdjustsFontSizeToFitWidth = true;
			roundView.AddSubview(roundLabel);
			roundView.BorderColor = UIColor.Gray;
			roundView.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDark);
			roundView.CornerRadius = 0;
			
			
			nextButton = new GradientButton();
			StyleButton(nextButton);
			
			nextButton.SetTitle("Next", UIControlState.Normal);
			nextButton.SetImage(UIImage.FromFile("Images/External/next-16.png"), UIControlState.Normal);
			nextButton.TouchUpInside += HandleNextButtonTouchUpInside;
			AddSubview(nextButton);
			
			
			prevButton = new GradientButton();
			StyleButton(prevButton);
			prevButton.SetTitle("Prev", UIControlState.Normal);
			prevButton.SetImage(UIImage.FromFile("Images/External/prev-16.png"), UIControlState.Normal);
			prevButton.TouchUpInside += HandlePrevButtonTouchUpInside;
			AddSubview(prevButton);
			
			
			
			moveUpButton = new GradientButton();
			StyleButton(moveUpButton);
			moveUpButton.SetTitle("Move Up", UIControlState.Normal);
			moveUpButton.SetImage(UIImage.FromFile("Images/External/arrowup-16.png"), UIControlState.Normal);
			moveUpButton.TouchUpInside += HandleMoveUpButtonTouchUpInside;;
			AddSubview(moveUpButton);
			
			
			moveDownButton = new GradientButton();
			StyleButton(moveDownButton);
			moveDownButton.SetTitle("Move Down", UIControlState.Normal);
			moveDownButton.SetImage(UIImage.FromFile("Images/External/arrowdown-16.png"), UIControlState.Normal);
			moveDownButton.TouchUpInside += HandleMoveDownButtonTouchUpInside;;
			AddSubview(moveDownButton);
			
			
			rollButton = new GradientButton();
			StyleButton(rollButton);
			rollButton.SetImage(UIImage.FromFile("Images/External/d20-32.png"), UIControlState.Normal);
			rollButton.SetTitle("Roll Initiative", UIControlState.Normal);
			rollButton.TouchUpInside += HandleRollButtonTouchUpInside;
			AddSubview(rollButton);
			
			
			sortButton = new GradientButton();
			StyleButton(sortButton);
			sortButton.SetTitle("Sort", UIControlState.Normal);
			sortButton.SetImage(UIImage.FromFile("Images/External/sort-16.png"), UIControlState.Normal);
			sortButton.TouchUpInside += HandleSortButtonTouchUpInside;
			AddSubview(sortButton);
		}

		void HandleMoveDownButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_SelectedCharacter != null)
			{
				_combatState.MoveDownCharacter(_SelectedCharacter);	
			}
		}

		void HandleMoveUpButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_SelectedCharacter != null)
			{
				_combatState.MoveUpCharacter(_SelectedCharacter);
			}
		}
		
		
		private void StyleButton(GradientButton b)
		{
			b.CornerRadius = 0;
			b.TitleLabel.Font = UIFont.SystemFontOfSize(14);
		}
		
		
		void HandleSortButtonTouchUpInside (object sender, EventArgs e)
		{
			CombatState.SortCombatList(true, true);
			ReloadList();
			MainUI.SaveCombatState();
		}

		void HandleNextButtonTouchUpInside (object sender, EventArgs e)
		{
			CombatState.MoveNext();
			ReloadList();
			MainUI.SaveCombatState();
		}

		void HandlePrevButtonTouchUpInside (object sender, EventArgs e)
		{
			CombatState.MovePrevious();
			ReloadList();
			MainUI.SaveCombatState();
		}

		void HandleRollButtonTouchUpInside (object sender, EventArgs e)
		{
			CombatState.RollInitiative();
			CombatState.SortCombatList();
			ReloadList();
			MainUI.SaveCombatState();
		}
		
		void ReloadList()
		{
			Character ch = _SelectedCharacter;
			listView.ReloadData();
			
			int index = _combatState.CombatList.IndexOf(ch);
			
			if (index >= 0)
			{
				
				listView.SelectRow(NSIndexPath.FromRowSection(index, 0), false, UITableViewScrollPosition.Middle);	
			}
			
		}
		
		public CombatState CombatState
		{
			get
			{
				return _combatState;
			}
			set
			{
				if (_combatState != value)
				{
					if (_combatState != null)
					{
						
						_combatState.CombatList.CollectionChanged -= CombatListChanged;
						_combatState.PropertyChanged -= Handle_combatStatePropertyChanged;
					}
					
					_combatState = value;
					viewDelegate = new ViewDelegate(this);
					viewDataSource = new ViewDataSource(this);
					listView.Delegate = viewDelegate;
					listView.DataSource = viewDataSource;
					
					ReloadList();
					
					if (_combatState != null)
					{
						_combatState.CombatList.CollectionChanged += CombatListChanged;
						_combatState.PropertyChanged += Handle_combatStatePropertyChanged;
						
						UpdateStatus();
					}
				}
				
			}
		}

		void Handle_combatStatePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if ((e.PropertyName == "CurrentCharacter") || (e.PropertyName == "Round"))
			{
				UpdateStatus();
			}
		}
				
		private void CombatListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ReloadList();
		}
		
		

		void HandleChPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
			{
				ReloadList();	
			}
		}
		
		
		void HandleChMonsterActiveConditionsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			ReloadList();
		}
		
		void UpdateCell(Character ch, DataListViewCell cell)
		{
			cell.Data = ch;
			
			cell.TextLabel.Text = ch.Name;
			if (ch == _combatState.CurrentCharacter)
			{
				cell.TextLabel.Font = _FontBold;
			}
			else
			{
				cell.TextLabel.Font = _Font;
			}
			cell.TextLabel.AdjustsFontSizeToFitWidth = true;
			
			if (ch.HasCondition("dead") )
			{
				cell.TextLabel.TextColor = UIColor.Red;	
			}
			else if (ch.HasCondition("dying"))
			{
				cell.TextLabel.TextColor = UIColor.Brown;
			}
			
		}

		
		static float buttonHeight = 40;
		static float labelMarginX = 5;
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			RectangleF rect = ConvertRectFromView(Frame, Superview);
			
			RectangleF statusRect = new RectangleF(0, 0, (rect.Width/3.0f)*2.0f + 1.0f, buttonHeight);
			currentCharacterView.Frame = statusRect;
			RectangleF labelRect  = currentCharacterView.Bounds;
			labelRect.X += labelMarginX;
			labelRect.Width -= 2 * labelMarginX;
			
			currentCharacterLabel.Frame = labelRect;
			
			statusRect.X = statusRect.Width - 1.0f;
			statusRect.Width = rect.Width/3.0f;
			roundView.Frame = statusRect;
			labelRect  = roundView.Bounds;
			labelRect.X += labelMarginX;
			labelRect.Width -= 2 * labelMarginX;
			roundLabel.Frame = labelRect;
			
			
			float bWidth = (rect.Width + 1.0f) /2;
			RectangleF button = new RectangleF(0, buttonHeight, bWidth, buttonHeight);
			prevButton.Frame = button;
			button.X += bWidth - 1.0f;
			nextButton.Frame = button;
			
			button.X = 0;
			button.Y = button.Bottom;
			moveUpButton.Frame = button;
			button.X += bWidth - 1.0f;
			moveDownButton.Frame = button;
			
			RectangleF list = rect;
			list.Y += button.Bottom;
			list.Height -= list.Y + buttonHeight;
			listView.Frame = list;
			
			button.X = 0;
			button.Y = list.Bottom;
			rollButton.Frame = button;
			button.X += bWidth -1.0f;
			sortButton.Frame = button;
			
		}
		
		private void InitButtonTouchUpInside(object sender, EventArgs e)
		{
			GradientButton b = (GradientButton)sender;
			Character ch = (Character)b.Data;
			
			if (ch != null)
			{
				NumberModifyPopover pop = new NumberModifyPopover();
				pop.ShowOnView((UIView)sender);
				pop.Value = ch.CurrentInitiative;
				pop.ValueType = "CurrentInit";
				pop.Data = ch;
				pop.ValueFormat = "Initiative: {0}";
				pop.NumberModified += HandleInitPopNumberModified;
			}
		}

		void HandleInitPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			NumberModifyPopover pop = (NumberModifyPopover)sender;
			GradientButton b = (GradientButton)pop.ParentView;
			
			Character ch = (Character)pop.Data;
			
			if (args.Set)
			{
				ch.CurrentInitiative = args.Value.Value;
			}
			else
			{
				ch.CurrentInitiative += args.Value.Value;
			}
			
			pop.Value = ch.CurrentInitiative;
			b.SetText(ch.CurrentInitiative.ToString());
		}
		
		
		private void WillShowActionsPopover(object sender, EventArgs e)
		{
			
			ButtonStringPopover p = (ButtonStringPopover)sender;
		    
			
			Character ch = (Character)p.Data;
			
			List<CharacterActionItem> actions = CharacterActions.GetInitiativeItems(ch, _SelectedCharacter);
			p.Items.Clear();
			foreach (CharacterActionItem it in actions)
			{
				ButtonStringPopoverItem b = new ButtonStringPopoverItem();
				b.Tag = it;
				b.Text = it.Name;
				b.Icon = it.Icon;
				p.Items.Add(b);
			}
		}
		
		private void UpdateStatus()
		{
			currentCharacterLabel.Text = (_combatState.CurrentCharacter != null)?
				_combatState.CurrentCharacter.Name:"";
			roundLabel.Text = "RD " + ((_combatState.Round != null)?
				_combatState.Round.ToString():"");
		}
					                                                                                                                    
		
		private class ViewDataSource : UITableViewDataSource
		{
			CombatListView state;
			public ViewDataSource(CombatListView state)	
			{
				this.state = state;
				
				
			}
			
			public override int RowsInSection (UITableView tableView, int section)
			{
				return state.CombatState.CombatList.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("CombatListViewCell");
				
				if (cell == null)
				{
					cell = new DataListViewCell (UITableViewCellStyle.Default, "CombatListViewCell");
				}
			
				Character ch = state.CombatState.CombatList[indexPath.Row];
				
				ch.PropertyChanged += state.HandleChPropertyChanged;
				ch.Monster.ActiveConditions.CollectionChanged += state.HandleChMonsterActiveConditionsCollectionChanged;
				
				
				state.UpdateCell(ch, cell);
				
				
				UIView buttonView = new UIView(new RectangleF(0, 0, 79, 30));
				
				
				GradientButton b = new GradientButton();
				b.SetTitle(ch.CurrentInitiative.ToString(), UIControlState.Normal);
				b.CornerRadius = 0;
				b.Frame = new RectangleF(0, 0, 40, 30);
				b.TouchUpInside += state.InitButtonTouchUpInside;
				b.Data = ch;
				buttonView.AddSubview(b);
				b = new GradientButton();
				b.SetImage(UIExtensions.GetSmallIcon("lightning"), UIControlState.Normal);
				b.CornerRadius = 0;
				b.Frame = new RectangleF(39, 0, 40, 30);
				
				buttonView.AddSubview(b);
				b.Data = ch;
				
				
				ButtonStringPopover actionsPopover = new ButtonStringPopover(b);
				actionsPopover.WillShowPopover += state.WillShowActionsPopover;
				actionsPopover.Data = ch;
				actionsPopover.ItemClicked += HandleActionsPopoverItemClicked;
				
				
				
				cell.AccessoryView = buttonView;
				
	
				return cell;			
			}

			void HandleActionsPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
			{
				Character ch = (Character)((ButtonStringPopover)sender).Data;
			
				CharacterActionItem item = (CharacterActionItem)e.Tag;
			
				if (item.Action != CharacterActionType.None)
				{
					CharacterActions.TakeAction(state._combatState, item.Action, ch, new List<Character>() {ch}, item.Tag);
				}
				
			}
			
		}
		
		
		private class ViewDelegate : UITableViewDelegate
		{
			CombatListView state;
			public ViewDelegate(CombatListView state)	
			{
				this.state = state;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (state != null)
				{
					state._SelectedCharacter = state.CombatState.CombatList[indexPath.Row];
				}
			}
			
			public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
			{
				
				if (state != null)
				{
					state._SelectedCharacter = null;
				}
			}
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 28;
			}
			public CombatListView ListView
			{
				get
				{
					return state;
				}
			}
		}
	}
}

