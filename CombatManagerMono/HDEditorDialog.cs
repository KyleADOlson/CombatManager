/*
 *  HDEditorDialog.cs
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
	public partial class HDEditorDialog : StandardDialogView
	{
		
		List<DieStep> _DieSteps = new List<DieStep>();
		int _Mod = 0;
		
		NumberModifyPopover _NumberPop;
		
		ButtonStringPopover _AddDiePopover;
		
		ViewDataSource _DataSource;
		ViewDelegate _Delegate;
		
		
		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public HDEditorDialog ()
			: base (UserInterfaceIdiomIsPhone ? "HDEditorDialog_iPhone" : "HDEditorDialog_iPad", null)
		{
		}
		
		protected override void Initialize ()
		{
			base.Initialize ();
			
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
			StyleButton(AddDieButton);
			CreateAddDiePopover();
			StyleHeader(HeaderView, HeaderLabel);
			StyleBackground(BackgroundView);
			StylePanel1(ModView);
			StyleButton(ModButton);
			UpdateDieRoll();
			
			SetupTable();
		}
		
		private void SetupTable()
		{
			
			_DataSource = new ViewDataSource(this);
			_Delegate = new ViewDelegate(this);
		
			
			DieTableView.DataSource = _DataSource;
			DieTableView.Delegate = _Delegate;
			DieTableView.ReloadData();
			
		}
		
		private void CreateAddDiePopover()
		{
			_AddDiePopover = new ButtonStringPopover(AddDieButton);
			foreach (int die in new int[] {4, 6, 8, 10, 12})
			{
				ButtonStringPopoverItem item = new ButtonStringPopoverItem();
				item.Text = die.ToString();
				item.Tag = die;
				_AddDiePopover.Items.Add(item);
			}
			_AddDiePopover.ItemClicked += Handle_AddDiePopoverItemClicked;
		}

		void Handle_AddDiePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			int die = (int)e.Tag;
			
			bool found = false;
			
			foreach (DieStep step in _DieSteps)
			{
				if (step.Die == die)
				{
					step.Count++;
					found = true;
					break;
				}
			}
			
			if (!found)
			{
				DieStep step = new DieStep(1, die);
				_DieSteps.Add(step);
			}
			
			UpdateDieRoll();
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
		
		private void UpdateDieRoll()
		{
			if (DieTableView != null)
			{
				DieTableView.ReloadData();
			}
			UpdateModButton();
			UpdateOK();
		}
		
		private void UpdateOK()
		{
			if (OKButton != null)
			{
				OKButton.Enabled = _DieSteps.Count > 0;	
			}
		}
		
		private void UpdateModButton()
		{
			if (ModButton != null)
			{
				ModButton.SetText(_Mod.PlusFormat());	
			}
		}
		
		public DieRoll DieRoll
		{
			get
			{
				DieRoll roll = new DieRoll();
				if (_DieSteps.Count > 0)
				{
					roll.AllRolls = _DieSteps;
				}
				roll.mod = _Mod;
				
				return roll;
			}
			set
			{
				_DieSteps.Clear();
				if (value == null)
				{
					_Mod = 0;
				}
				else
				{
					_DieSteps.AddRange(value.AllRolls);
					_Mod = value.mod;
				}
				
				UpdateDieRoll();
				
			}
		}
		
		partial void OKButtonTouchUpInside (Foundation.NSObject sender)
		{
			if (_DieSteps.Count > 0)
			{
				HandleOK();
			}
		}
		
		partial void CancelButtonTouchUpInside (Foundation.NSObject sender)
		{
			HandleCancel();
		}
		
		partial void AddDieButtonTouchUpInside (Foundation.NSObject sender)
		{
			
		}
		
		partial void ModButtonTouchUpInside (Foundation.NSObject sender)
		{
			
			_NumberPop = new NumberModifyPopover();
			_NumberPop.Title = "Die Modifier";
			_NumberPop.Data = "Die Modifier";
			_NumberPop.Value = _Mod;
			_NumberPop.ShowOnView(ModButton);
			_NumberPop.NumberModified += Handle_NumberPopNumberModified;
		}

		void Handle_NumberPopNumberModified (object sender, NumberModifyEventArgs args)
		{
			if (args.Set)
			{
				_Mod = (int)args.Value;
			}
			else
			{
				_Mod += (int)args.Value;
			}
			
			_NumberPop.Value = _Mod;
			UpdateModButton();
		}
		
		
		private static float bWidth = 49;
		private static float bHeight = 28;
		
		private class ViewDataSource : UITableViewDataSource
		{
			int lastSelectedIndex;
			
			HDEditorDialog state;
			public ViewDataSource(HDEditorDialog state)	
			{
				this.state = state;
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return state._DieSteps.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				DataListViewCell cell = (DataListViewCell)tableView.DequeueReusableCell ("HDEditorDialogCell");
				
				if (cell == null)
				{
					cell = new DataListViewCell (UITableViewCellStyle.Default, "HDEditorDialogCell");
				}
				
				DieStep step = state._DieSteps[indexPath.Row];
				
				
				
				UIView buttonView = new UIView(new CGRect(0, 0, bWidth*4 + 1, bHeight));
				GradientButton b = new GradientButton();
				b.SetText(step.Count.ToString());
				StyleButton(b);
				b.Frame = new CGRect(0, 0, bWidth, bHeight);
				b.TouchUpInside += HandleCountTouchUpInside;
				b.Tag = indexPath.Row;
				buttonView.AddSubview(b);
				
				UILabel l = new UILabel(new CGRect(bWidth, 0, bWidth, bHeight));
				l.Text = "d";
				l.TextAlignment = UITextAlignment.Center;
				l.BackgroundColor = UIColor.Clear;
				buttonView.AddSubview(l);
				
				b = new GradientButton();
				b.SetText(step.Die.ToString());
				StyleButton(b);
				b.Frame = new CGRect(bWidth*2, 0, bWidth, bHeight);
				b.TouchUpInside += HandleDieTouchUpInside;
				b.Tag = indexPath.Row;
				buttonView.AddSubview(b);
				
				b = new GradientButton();
				b.SetImage(UIExtensions.GetSmallIcon("delete"), UIControlState.Normal);
				StyleButton(b);
				b.Frame = new CGRect(bWidth*3, 0, bWidth + 1, bHeight);
				buttonView.AddSubview(b);
				b.Tag = indexPath.Row;
				b.TouchUpInside += HandleDeleteTouchUpInside;
				
				
				cell.AccessoryView = buttonView;

				cell.Data = state._DieSteps[indexPath.Row];
				//cell.TextLabel.Text = state._DieSteps.ToString();
				
				return cell;			
			}

			void HandleCountTouchUpInside (object sender, EventArgs e)
			{
				
                int index = (int)((UIButton)sender).Tag;
				lastSelectedIndex = index;
				DieStep step = state._DieSteps[index];
				state._NumberPop = new NumberModifyPopover();
				state._NumberPop.Title = "Die Count";
				state._NumberPop.Data = "Die Count";
				state._NumberPop.Value = step.Count;
				state._NumberPop.ShowOnView((UIButton)sender);
				state._NumberPop.NumberModified += Handle_CountPopModified;
			}
			
			void Handle_CountPopModified (object sender, NumberModifyEventArgs args)
			{
				
				DieStep step = state._DieSteps[lastSelectedIndex];
				step.Count = args.ModifyValue(step.Count, 0, int.MaxValue).Value;
				state.UpdateDieRoll();
			}
			void HandleDieTouchUpInside (object sender, EventArgs e)
			{
                int index = (int)((UIButton)sender).Tag;
				lastSelectedIndex = index;
				DieStep step = state._DieSteps[index];
				state._NumberPop = new NumberModifyPopover();
				state._NumberPop.Title = "Die";
				state._NumberPop.Data = "Die";
				state._NumberPop.Value = step.Die;
				state._NumberPop.ShowOnView((UIButton)sender);
				state._NumberPop.NumberModified += Handle_DiePopModified;
				
			}
			void Handle_DiePopModified (object sender, NumberModifyEventArgs args)
			{
				
				DieStep step = state._DieSteps[lastSelectedIndex];
				step.Die = args.ModifyValue(step.Die, 0, int.MaxValue).Value;
				state.UpdateDieRoll();
			}
			

			void HandleDeleteTouchUpInside (object sender, EventArgs e)
			{
                int index = (int)((UIButton)sender).Tag;
				state._DieSteps.RemoveAt(index);
				state.UpdateDieRoll();
			}
			
			
			private void StyleButton(GradientButton b)
			{
				b.CornerRadius = 0;
			}
			
		}
		
		
		private class ViewDelegate : UITableViewDelegate
		{
			HDEditorDialog state;
			public ViewDelegate(HDEditorDialog state)	
			{
				this.state = state;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
			}
			
			public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
			{
				
			}
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 28;
			}

            public HDEditorDialog State
            {
                get
                {
                    return state;
                }
            }
		}
		
		
	
	}
}

