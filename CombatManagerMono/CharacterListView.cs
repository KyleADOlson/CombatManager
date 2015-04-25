/*
 *  CharacterListView.cs
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
using System.Collections.Specialized;
using System.ComponentModel;
using CoreGraphics;

namespace CombatManagerMono
{
	public class CharacterSelectionChangedEventArgs
	{
		public Character OldCharacter;
		public Character NewCharacter;
	}
	
	public delegate void CharacterSelectionChangedEventHandler(object sender, CharacterSelectionChangedEventArgs e);
	
	
	public class CharacterListView : UIView
	{
		UITableView listView;
		CombatState _combatState;
		bool _monsters;
		List<Character> currentCharacters;
		
		Character _SelectedCharacter;
		
		ViewDataSource viewDataSource;
		ViewDelegate viewDelegate;
		
		GradientButton blankButton;
		GradientButton monsterButton;
		GradientButton openButton;
		GradientButton saveButton;
		
		GradientButton clearButton;
		GradientView bottomView;
		UILabel bottomLabel;
		
		OpenDialog openDialog;
		
		UIPopoverController _controller;
		MonsterAddView _monsterAddView;
		
		UIAlertView alertView;
		
		public event CharacterSelectionChangedEventHandler CharacterSelectionChanged;
		
		
		public CharacterListView (CombatState state, bool monsters)
		{
			listView = new UITableView();
			listView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			listView.SeparatorColor = CMUIColors.PrimaryColorMedium;
			listView.BackgroundColor = UIExtensions.ARGBColor(0x0);
			
			BackgroundColor = CMUIColors.PrimaryColorDarker;
			
			AddSubview(listView);
			
			viewDelegate = new ViewDelegate(this);
			viewDataSource = new ViewDataSource(this);
			listView.Delegate = viewDelegate;
			listView.DataSource = viewDataSource;
			
			_combatState = state;
			_monsters = monsters;
			_combatState.Characters.CollectionChanged += Handle_combatStateCombatListCollectionChanged;
			_combatState.CharacterSortCompleted += Handle_combatStateCharacterSortCompleted;
			_combatState.PropertyChanged += Handle_combatStatePropertyChanged;
			
			blankButton = new GradientButton();
			StyleButton(blankButton);
			blankButton.SetTitle("", UIControlState.Normal);
			blankButton.SetImage(UIExtensions.GetSmallIcon("invisible"), UIControlState.Normal);
			
			blankButton.TouchUpInside += HandleBlankButtonTouchUpInside;
			AddSubview(blankButton);
			
			
			monsterButton = new GradientButton();
			StyleButton(monsterButton);
			monsterButton.SetTitle("", UIControlState.Normal);
			monsterButton.SetImage(UIExtensions.GetSmallIcon("monster"), UIControlState.Normal);
			monsterButton.TouchUpInside += HandleMonsterButtonTouchUpInside;
			
			AddSubview(monsterButton);
			
			openButton = new GradientButton();
			StyleButton(openButton);
			openButton.SetText("");
			openButton.SetImage(UIExtensions.GetSmallIcon("openhs"), UIControlState.Normal);
			openButton.TouchUpInside += HandleOpenButtonTouchUpInside;
			AddSubview(openButton);
			
			
			saveButton = new GradientButton();
			StyleButton(saveButton);
			saveButton.SetText("");
			saveButton.SetImage(UIExtensions.GetSmallIcon("savehs"), UIControlState.Normal);
			saveButton.TouchUpInside += HandleSaveButtonTouchUpInside;
			AddSubview(saveButton);
			
			
			bottomView = new GradientView();
			AddSubview(bottomView);
			bottomLabel = new UILabel();
			bottomView.AddSubview(bottomLabel);
			bottomView.CornerRadius = 0f;
			bottomView.Gradient = new GradientHelper(
				CMUIColors.PrimaryColorLight, CMUIColors.PrimaryColorLight);
			
			
			
			clearButton = new GradientButton();
			StyleButton(clearButton);
			clearButton.SetText("");
			clearButton.SetImage(UIExtensions.GetSmallIcon("delete"), UIControlState.Normal);
			clearButton.TouchUpInside += HandleClearButtonTouchUpInside;
			AddSubview(clearButton);
			
			
			_monsterAddView = new MonsterAddView();
			_monsterAddView.IsMonsters = monsters;
			_monsterAddView.ShouldClose += Handle_monsterAddViewShouldClose;
			_controller = new UIPopoverController(_monsterAddView);
			_monsterAddView.CombatState = _combatState;
			
			SetBottomText();
			
		}



		void Handle_combatStatePropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CR" || e.PropertyName == "XP")
			{
				SetBottomText();
			}
		}

		void HandleClearButtonTouchUpInside (object sender, EventArgs e)
		{
			if (currentCharacters.Count > 0)
			{
				alertView = new UIAlertView    
				{        
					Title = "Are you sure you want to clear all characters?",
					Message = (currentCharacters.Count + " character(s) will be removed")
					
				};        
				alertView.AddButton("Cancel");    
				alertView.AddButton("OK");
				alertView.Show();
				alertView.Clicked += HandleAlertViewClicked;
			}
		}

		void HandleAlertViewClicked (object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 1)
			{
				            
                List<Character> removeList = new List<Character>();

                foreach (Character character in currentCharacters)
                {
                    removeList.Add(character);
                }
                foreach (Character character in removeList)
                {
                    _combatState.RemoveCharacter(character);
                }	
			}
		}

		void HandleSaveButtonTouchUpInside (object sender, EventArgs e)
		{
			
			openDialog = new OpenDialog(false);
			MainUI.MainView.AddSubview(openDialog.View);
			openDialog.FilesOpened += HandleSaveDialogFilesOpened;
		}

		void HandleOpenButtonTouchUpInside (object sender, EventArgs e)
		{
			openDialog = new OpenDialog();
			MainUI.MainView.AddSubview(openDialog.View);
			openDialog.FilesOpened += HandleOpenDialogFilesOpened;

		}
		
		void HandleSaveDialogFilesOpened (object sender, OpenDialogEventArgs e)
		{
			this._combatState.SavePartyFile(e.Files[0], _monsters);
			
		}

		void HandleOpenDialogFilesOpened (object sender, OpenDialogEventArgs e)
		{
			try
			{
				this._combatState.LoadPartyFiles(e.Files, _monsters);
			}
			catch (Exception ex)
			{
				alertView = new UIAlertView() {Title = "Unable to Load Files"};
				alertView.AddButton("OK");
				alertView.Show();
				
			}
			
		}

		void Handle_monsterAddViewShouldClose (object sender, EventArgs e)
		{
			_controller.Dismiss(true);
		}

		void HandleMonsterButtonTouchUpInside (object sender, EventArgs e)
		{
			UIButton button = (UIButton)sender;
			
			
			_controller.SetPopoverContentSize(_monsterAddView.View.Frame.Size, true);
			_controller.PresentFromRect(button.Frame, button.Superview, UIPopoverArrowDirection.Any, true);
		}
		

		void HandleBlankButtonTouchUpInside (object sender, EventArgs e)
		{
			
			this._combatState.AddBlank(this._monsters);
		}
		
		private void StyleButton(GradientButton b)
		{
			b.CornerRadius = 0;
			b.TitleLabel.Font = UIFont.SystemFontOfSize(14);
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			CGRect rect = ConvertRectFromView(Frame, Superview);
			
			double bWidth = (rect.Width + 3.0d) /4;
            double buttonHeight = 40;
            CGRect button = new CGRect(0, 0, new nfloat(bWidth), new nfloat(buttonHeight));
			blankButton.Frame = button;
            button.X += new nfloat(bWidth - 1.0d);
			monsterButton.Frame = button;
            button.X += new nfloat(bWidth - 1.0d);
			openButton.Frame = button;
            button.X += new nfloat(bWidth - 1.0d);
			saveButton.Frame = button;
			
			
			
			CGRect list = rect;
            list.Y += new nfloat(button.Y + buttonHeight);
            list.Height -= new nfloat(buttonHeight * 2);
			listView.Frame = list;
			
			button.Y = listView.Frame.Bottom;
			
			clearButton.Frame = button;
			
			CGRect bottomFrame = button;
			bottomFrame.X = 0;
			bottomFrame.Width = button.X;
			bottomFrame.Height++;
			bottomView.Frame = bottomFrame;
			CGRect labelFrame = bottomView.Bounds;
			labelFrame.X += 5;
			labelFrame.Width -= 10;
			bottomLabel.Frame = labelFrame;
			bottomLabel.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
			
			
		}

		void Handle_combatStateCombatListCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!_combatState.SortingList)
			{
				UpdateAfterListChanged();
			}
		}
		
		
		void Handle_combatStateCharacterSortCompleted (object sender, EventArgs e)
		{
			UpdateAfterListChanged();
		}
		
		void UpdateAfterListChanged()
		{
			FilterCharacters();
			listView.ReloadData();
			SetBottomText();
			
		}
		
		void SetBottomText()
		{
			if (_monsters)
			{
				bottomLabel.Text = "CR " + _combatState.CR + ", XP " + _combatState.XP;
			}
			else
			{
				bottomLabel.Text = "";
			}
		}
		
		
		void FilterCharacters()
		{
			if (currentCharacters != null)
			{
				foreach (Character ch in currentCharacters)
				{
					
					ch.PropertyChanged -= HandleChPropertyChanged;
				}
			}
			
			currentCharacters = new List<Character>();
			
			foreach (Character ch in _combatState.Characters)
			{
				if (ch.IsMonster == _monsters)
				{
					currentCharacters.Add(ch);	
					ch.PropertyChanged += HandleChPropertyChanged;
				}
			}
		}

		void ReloadCharacter(Character ch)
		{
			System.Diagnostics.Debug.WriteLine("Reload Character " + ch.Name);
			int index = currentCharacters.IndexOf(ch);
			if (index >= 0)
			{
				NSIndexPath path = NSIndexPath.Create(new int[] {0, index});
				listView.ReloadRows(new NSIndexPath[] {path}, UITableViewRowAnimation.None);
			}
		}

		void HandleChPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			
		}
		
        public Character SelectedCharacter
        {
            get
            {
                NSIndexPath ip = listView.IndexPathForSelectedRow;
                if (ip != null)
                {
                    int index = ip.Row;

                    return currentCharacters[index];
                }
                return null;
            }
        }
		
		private class ViewDataSource : UITableViewDataSource
		{
			CharacterListView state;
			
					protected Dictionary<int, CharacterListCellView> _cellControllers = 
			new Dictionary<int, CharacterListCellView>();
			
			public ViewDataSource(CharacterListView state)	
			{
				this.state = state;
				
				
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				if (state.currentCharacters == null)
				{
					state.FilterCharacters();
				}
				return state.currentCharacters.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell ("CharacterListCellView");
				CharacterListCellView customCellController = null;
				
				if (cell == null)
				{
					customCellController = new CharacterListCellView ();
					cell = customCellController.Cell;
					cell.Tag = Environment.TickCount;
                    this._cellControllers.Add ((int)cell.Tag, customCellController);
					customCellController.CharacterListView = this.state;
				}
				else
				{
                    customCellController = this._cellControllers[(int)cell.Tag];
				}
				
				Character ch = state.currentCharacters[indexPath.Row];
				
				
				customCellController.CombatState = state._combatState;
				customCellController.Character = ch;
				
				return cell;		
			}
			
			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				Character ch = state.currentCharacters[indexPath.Row];
				state._combatState.RemoveCharacter(ch);
			}
			
			
		}
		
		private class ViewDelegate : UITableViewDelegate
		{
			CharacterListView state;
			public ViewDelegate(CharacterListView state)	
			{
				this.state = state;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (state != null)
				{
					UITableViewCell cell = tableView.CellAt(indexPath);
					if (cell != null)
					{
						cell.SetNeedsDisplay();
					}
					
					CharacterSelectionChangedEventArgs e = new CharacterSelectionChangedEventArgs();
					e.OldCharacter = state._SelectedCharacter;		
					state._SelectedCharacter = state.currentCharacters[indexPath.Row];
					e.NewCharacter = state._SelectedCharacter;
					if (state.CharacterSelectionChanged != null)
					{
						state.CharacterSelectionChanged(state, e);
					}
				}
			}
			public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
			{
				if (state != null)
				{
					UITableViewCell cell = tableView.CellAt(indexPath);
					if (cell != null)
					{
						cell.SetNeedsDisplay();
					}
					
					Character ch = state.currentCharacters[indexPath.Row];
					
					if (state._SelectedCharacter == ch)
					{
						
						CharacterSelectionChangedEventArgs e = new CharacterSelectionChangedEventArgs();
						e.OldCharacter = state._SelectedCharacter;		
						e.NewCharacter = null;
						if (state.CharacterSelectionChanged != null)
						{
							state.CharacterSelectionChanged(state, e);
						}
					}
					
				}
			}
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
                return 79 + CharacterListCellView.ConditionViewHeight(state.currentCharacters[indexPath.Row], (double)tableView.Bounds.Width);
			}
			
			public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return UITableViewCellEditingStyle.Delete;
			}
			
		}
	}
}

