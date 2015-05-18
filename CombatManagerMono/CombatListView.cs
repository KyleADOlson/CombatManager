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
using Foundation;
using UIKit;
using CombatManager;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CoreGraphics;

namespace CombatManagerMono
{
	public class CombatListView : UIView
	{
		CombatState _CombatState;
		ViewDataSource viewDataSource;
		ViewDelegate viewDelegate;
		UITableView _ListView;
		
		GradientButton _NextButton;
		GradientButton _PrevButton;
		GradientButton _MoveUpButton;
		GradientButton _MoveDownButton;

		GradientButton _RollButton;		
        GradientButton _SortButton;
        GradientButton _ResetButton;
		
		GradientView _CurrentCharacterView;
		UILabel _CurrentCharacterLabel;
		GradientView _RoundView;
		UILabel _RoundLabel;
		
		Character _SelectedCharacter;
		
		
		UIFont _Font;
	 	UIFont _FontBold;

        UIImage _ReadyingImage;
        UIImage _DelayingImage;
        UIImage _LinkImage;
		
		public CombatListView ()
		{
            ClipsToBounds = true;

			_Font = UIFont.SystemFontOfSize(14);
			_FontBold = UIFont.BoldSystemFontOfSize(14);

            _ReadyingImage = UIExtensions.GetSmallIcon("target");
            _DelayingImage = UIExtensions.GetSmallIcon("hourglass");
            _LinkImage = UIExtensions.GetSmallIcon("link");

		
			_ListView = new UITableView();
			AddSubview(_ListView);
			
			_CurrentCharacterView = new GradientView();
			AddSubview(_CurrentCharacterView);
			_CurrentCharacterLabel = new UILabel();
			_CurrentCharacterLabel.TextAlignment = UITextAlignment.Center;
			_CurrentCharacterLabel.BackgroundColor = UIExtensions.ARGBColor(0x0);
			_CurrentCharacterLabel.TextColor = UIColor.White;
			_CurrentCharacterLabel.AdjustsFontSizeToFitWidth = true;
            _CurrentCharacterLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
			_CurrentCharacterView.AddSubview(_CurrentCharacterLabel);
			_CurrentCharacterView.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDark);
			_CurrentCharacterView.BorderColor = UIColor.Gray;
			_CurrentCharacterView.CornerRadius = 0;
			
			_RoundView = new GradientView();
			AddSubview(_RoundView);
			_RoundLabel = new UILabel();
			_RoundLabel.BackgroundColor = UIExtensions.ARGBColor(0x0);
			_RoundLabel.TextAlignment = UITextAlignment.Center;
			_RoundLabel.TextColor = UIColor.White;
            _RoundLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
			_CurrentCharacterLabel.AdjustsFontSizeToFitWidth = true;
			_RoundView.AddSubview(_RoundLabel);
			_RoundView.BorderColor = UIColor.Gray;
			_RoundView.Gradient = new GradientHelper(CMUIColors.SecondaryColorBDark);
			_RoundView.CornerRadius = 0;
			
			
			_NextButton = new GradientButton();
			StyleButton(_NextButton);
			
			_NextButton.SetTitle("Next", UIControlState.Normal);
			_NextButton.SetImage(UIImage.FromFile("Images/External/RightArrow-24.png"), UIControlState.Normal);
            _NextButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 5);  
            _NextButton.TouchUpInside += HandleNextButtonTouchUpInside;
			AddSubview(_NextButton);
			
			
			_PrevButton = new GradientButton();
			StyleButton(_PrevButton);
			_PrevButton.SetTitle("Prev", UIControlState.Normal);
			_PrevButton.SetImage(UIImage.FromFile("Images/External/LeftArrow-24.png"), UIControlState.Normal);
			_PrevButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 5); 
            _PrevButton.TouchUpInside += HandlePrevButtonTouchUpInside;
			AddSubview(_PrevButton);
			
			
			
			_MoveUpButton = new GradientButton();
			StyleButton(_MoveUpButton);
			_MoveUpButton.SetTitle("Up", UIControlState.Normal);
			_MoveUpButton.SetImage(UIImage.FromFile("Images/External/arrowup-16.png"), UIControlState.Normal);
			_MoveUpButton.TouchUpInside += HandleMoveUpButtonTouchUpInside;;
			AddSubview(_MoveUpButton);
			
			
			_MoveDownButton = new GradientButton();
			StyleButton(_MoveDownButton);
			_MoveDownButton.SetTitle("Down", UIControlState.Normal);
			_MoveDownButton.SetImage(UIImage.FromFile("Images/External/arrowdown-16.png"), UIControlState.Normal);
			_MoveDownButton.TouchUpInside += HandleMoveDownButtonTouchUpInside;;
			AddSubview(_MoveDownButton);
			
			
			_RollButton = new GradientButton();
			StyleButton(_RollButton);
			_RollButton.SetImage(UIImage.FromFile("Images/External/d20-32.png"), UIControlState.Normal);
			_RollButton.SetTitle("Roll Initiative", UIControlState.Normal);
			_RollButton.TouchUpInside += HandleRollButtonTouchUpInside;
			AddSubview(_RollButton);
			
			
			_SortButton = new GradientButton();
			StyleButton(_SortButton);
			_SortButton.SetTitle("Sort", UIControlState.Normal);
            _SortButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 10);
			_SortButton.SetImage(UIImage.FromFile("Images/Sort.png"), UIControlState.Normal);
			_SortButton.TouchUpInside += HandleSortButtonTouchUpInside;
			AddSubview(_SortButton);

            
            _ResetButton = new GradientButton();
            StyleButton(_ResetButton);
            _ResetButton.SetTitle("Reset", UIControlState.Normal);
            _ResetButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 10);
            _ResetButton.SetImage(UIImage.FromFile("Images/Refresh.png"), UIControlState.Normal);
            _ResetButton.TouchUpInside += HandleResetButtonTouchUpInside;
            AddSubview(_ResetButton);

		}

        void HandleResetButtonTouchUpInside (object sender, EventArgs e)
        {
            foreach (Character c in _CombatState.Characters)
            {
                c.CurrentInitiative = 0;
                _CombatState.Round = null;

            }
            //_ListView.ReloadData();
        }

		void HandleMoveDownButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_SelectedCharacter != null)
			{
				_CombatState.MoveDownCharacter(_SelectedCharacter);	
			}
		}

		void HandleMoveUpButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_SelectedCharacter != null)
			{
				_CombatState.MoveUpCharacter(_SelectedCharacter);
			}
		}
		
		
		private void StyleButton(GradientButton b)
		{
			b.CornerRadius = 0;
			b.TitleLabel.Font = UIFont.SystemFontOfSize(14);
		}
		
		
		void HandleSortButtonTouchUpInside (object sender, EventArgs e)
		{
            ObservableCollection<Character> oldList = new ObservableCollection<Character>(CombatState.CombatList);
			CombatState.SortCombatList(true, true);
            ReloadList(oldList);
			MainUI.SaveCombatState();
		}

        int ActiveCharacterIndex
        {
            get
            {
                if (CombatState.CurrentCharacter == null)
                {
                    return -1;
                }

                return _CombatState.CombatList.IndexOf(CombatState.CurrentCharacter);
            }
        }

		void HandleNextButtonTouchUpInside (object sender, EventArgs e)
		{
            
            int index1 = ActiveCharacterIndex;
            CombatState.MoveNext();
            int index2 = ActiveCharacterIndex;

            ReloadListRows(new List<int>{index1, index2});
			MainUI.SaveCombatState();
		}

		void HandlePrevButtonTouchUpInside (object sender, EventArgs e)
		{
            int index1 = ActiveCharacterIndex;
			CombatState.MovePrevious();
            int index2 = ActiveCharacterIndex;

            ReloadListRows(new List<int>{index1, index2});
			MainUI.SaveCombatState();
		}


		void HandleRollButtonTouchUpInside (object sender, EventArgs e)
		{
            ObservableCollection<Character> oldList = new ObservableCollection<Character>(CombatState.CombatList);
			CombatState.RollInitiative();
			CombatState.SortCombatList();
			ReloadList(oldList);
			MainUI.SaveCombatState();

		}
		
        void ReloadList()
        {
            ReloadList(null);
        }

		void ReloadList(ObservableCollection<Character> oldList)
		{
			Character ch = _SelectedCharacter;
			
            if (oldList == null)
            {
                _ListView.ReloadData();
            }
            else
            {
                List<int> indexes = new List<int>();

                for (int i=0; i<_CombatState.CombatList.Count; i++)
                {

                    if (i>= oldList.Count || _CombatState.CombatList[i] != oldList[i])
                    {
                        indexes.Add(i);
                    }
                }

                ReloadListRows(indexes);
                if (oldList.Count > _CombatState.CombatList.Count)
                {
                    indexes = new List<int>();
                    for (int i=_CombatState.CombatList.Count; i<oldList.Count; i++)
                    {
                        indexes.Add(i);
                    }
                    DeleteListRows(indexes);
                }
            }

			int index = _CombatState.CombatList.IndexOf(ch);
			
			if (index >= 0)
			{
				
				_ListView.SelectRow(NSIndexPath.FromRowSection(index, 0), false, UITableViewScrollPosition.Middle);	
			}
			
		}

        void ReloadListRows(IEnumerable<int> indexes)
        {

            _ListView.ReloadRows((from a in indexes where a >= 0 select NSIndexPath.FromRowSection(a, 0)).ToArray(),
                                 UITableViewRowAnimation.None);
           
        }

        void DeleteListRows(IEnumerable<int> indexes)
        {

            _ListView.DeleteRows((from a in indexes where a >= 0 select NSIndexPath.FromRowSection(a, 0)).ToArray(),
                                 UITableViewRowAnimation.None);
           
        }
		
		public CombatState CombatState
		{
			get
			{
				return _CombatState;
			}
			set
			{
				if (_CombatState != value)
				{
					if (_CombatState != null)
					{
						
						_CombatState.CombatList.CollectionChanged -= CombatListChanged;
						_CombatState.PropertyChanged -= Handle_combatStatePropertyChanged;
					}
					
					_CombatState = value;
					viewDelegate = new ViewDelegate(this);
					viewDataSource = new ViewDataSource(this);
					_ListView.Delegate = viewDelegate;
					_ListView.DataSource = viewDataSource;
					
					ReloadList();
					
					if (_CombatState != null)
					{
						_CombatState.CombatList.CollectionChanged += CombatListChanged;
						_CombatState.PropertyChanged += Handle_combatStatePropertyChanged;
						
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
			if (e.PropertyName == "Name" || e.PropertyName == "IsReadying" || e.PropertyName == "IsDelaying")
			{
                UpdateCharacter(sender as Character);	
			}		
        }

        void UpdateCharacter(Character ch)
        {
            int index = CombatState.CombatList.IndexOf(ch);
            if (index != -1)
            {
                _ListView.ReloadRows(new NSIndexPath[] {NSIndexPath.FromRowSection(index, 0)}, UITableViewRowAnimation.None);
            }
        }
		
		

		
		void UpdateCell(Character ch, DataListViewCell cell)
		{
			cell.Data = ch;
			
			cell.TextLabel.Text = ch.Name;
			if (ch == _CombatState.CurrentCharacter)
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
			CGRect rect = ConvertRectFromView(Frame, Superview);

            double xColumnWidth = (rect.Width/3.0d);		
            double yRowHeight = buttonHeight;
			
            CGRect statusRect = new CGRect(0, 0, new nfloat(xColumnWidth*2.0d + 1.0d), new nfloat(buttonHeight));
			_CurrentCharacterView.Frame = statusRect;

			CGRect labelRect  = _CurrentCharacterView.Bounds;
			labelRect.X += labelMarginX;
			labelRect.Width -= 2 * labelMarginX;			
			_CurrentCharacterLabel.Frame = labelRect;
			
            statusRect.Y += new nfloat(yRowHeight);
			_RoundView.Frame = statusRect;

			labelRect  = _RoundView.Bounds;
			labelRect.X += labelMarginX;
			labelRect.Width -= 2 * labelMarginX;
			_RoundLabel.Frame = labelRect;
			
			
            CGRect button = new CGRect(new nfloat(xColumnWidth * 2.0f), new nfloat(0), new nfloat(xColumnWidth + 1.0d), new nfloat(yRowHeight * 2.0d));
            button.Width -= 1;
            _NextButton.Frame = button;
            button.Width += 1;
			
            button.X = 0;
            button.Y = button.Bottom;
            button.Height = button.Height /2.0f;
            _MoveUpButton.Frame = button;


            button.X += new nfloat(xColumnWidth);
            _MoveDownButton.Frame = button;	

            button.X += new nfloat(xColumnWidth);
            button.Width -= 1;
            _PrevButton.Frame = button;
            button.Width += 1;
			
			CGRect list = rect;
			list.Y += button.Bottom;
            list.Height -= list.Y + buttonHeight*2;
			_ListView.Frame = list;
			
			button.X = 0;
			button.Y = list.Bottom;
            button.Width = (rect.Width/2.0f) + 1;
            button.Height = Bounds.Height - list.Bottom;
			_RollButton.Frame = button;
			
            button.X += (rect.Width/2.0f);
            button.Width -=1;
            button.Height = button.Height/2.0f;
			_SortButton.Frame = button;

            button.Y += button.Height;
            _ResetButton.Frame = button;

			
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
			
            List<CharacterActionItem> actions = CharacterActions.GetInitiativeItems(ch, _SelectedCharacter, null);
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
			_CurrentCharacterLabel.Text = (_CombatState.CurrentCharacter != null)?
				_CombatState.CurrentCharacter.Name:"";
			_RoundLabel.Text = "Round " + ((_CombatState.Round != null)?
				_CombatState.Round.ToString():"");
		}
					                                                                                                                    
		
		private class ViewDataSource : UITableViewDataSource
		{
			CombatListView state;
			public ViewDataSource(CombatListView state)	
			{
				this.state = state;
				
				
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
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
				ch.Monster.ActiveConditions.CollectionChanged += delegate
                    {
                        state.UpdateCharacter(ch); 
                    };
                    
				
				
				state.UpdateCell(ch, cell);

                float accHeight = 30f;
				
				
				UIView buttonView = new UIView(new CGRect(0, 0, 79, accHeight));
				
                float xPos = 0;
				GradientButton b = new GradientButton();

                if (ch.IsReadying || ch.IsDelaying)
                {
                    UIImageView view = new UIImageView();
                    view.Image = ch.IsReadying?state._ReadyingImage:state._DelayingImage;
                    view.Frame = new CGRect(new CGPoint(xPos, (accHeight - 16f)/2.0f), new CGSize(16, 16));
                    buttonView.Add (view);
                    xPos += 18;
                }

                if (ch.HasFollowers)
                {
                    
                    UIImageView view = new UIImageView();
                    view.Image = state._LinkImage;
                    view.Frame = new CGRect(new CGPoint(xPos, (accHeight - 16f)/2.0f), new CGSize(16, 16));
                    buttonView.Add (view);
                    xPos += 18;
                }


				b.SetTitle(ch.CurrentInitiative.ToString(), UIControlState.Normal);
				b.CornerRadius = 0;
				b.Frame = new CGRect(xPos, 0, 40, accHeight);
				b.TouchUpInside += state.InitButtonTouchUpInside;
				b.Data = ch;
				buttonView.AddSubview(b);

                xPos += (float)(b.Frame.Width - 1);

				b = new GradientButton();
				b.SetImage(UIExtensions.GetSmallIcon("lightning"), UIControlState.Normal);
				b.CornerRadius = 0;
				b.Frame = new CGRect(xPos, 0, 40, accHeight);
				
				buttonView.AddSubview(b);
				b.Data = ch;

                xPos += (float)(b.Frame.Width -1);

                buttonView.SetWidth(xPos);
				
				
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
					CharacterActions.TakeAction(state._CombatState, item.Action, ch, new List<Character>() {ch}, item.Tag);
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
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
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


