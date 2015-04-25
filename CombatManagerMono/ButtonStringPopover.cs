/*
 *  ButtonStringPopover.cs
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
	
    public class WillShowPopoverEventArgs : EventArgs
    {
        public bool Cancel {get; set;}
    }

	public class ButtonStringPopoverItem
	{
		public string Text {get; set;}
		public string Icon {get; set;}
		public object Tag {get; set;}
		public bool Disabled{get; set;}
        public bool Selected{ get; set;}
		public List<ButtonStringPopoverItem> Subitems {get;set;}
		
		public ButtonStringPopoverItem()
		{
			Subitems  = new List<ButtonStringPopoverItem>();
		}
		
		
		
	}
	
	public class ButtonStringPopover : UIViewController
	{
		UIButton _button;
		List<ButtonStringPopoverItem> _Items = new List<ButtonStringPopoverItem>();
		List<ButtonStringPopoverItem> _CurrentItems;
		
		UIPopoverController _controller;
		
		bool _SetButtonText;
		
		public event EventHandler<WillShowPopoverEventArgs> WillShowPopover;
		
		float _separatorHeight = 10;
		float _rowHeight = 28;
		
		UITableView _TableView;
		UIView _AccessoryView;
		
		
		public class PopoverEventArgs
		{
			
			public PopoverEventArgs()
			{
				
			}
			
			public PopoverEventArgs(string text, int index, object tag)
			{
				Text = text;
				Tag = tag;
				Index = index;
			}
			
			public string Text {get; set;}
			public int Index{get; set;}
			public object Tag {get; set;}
		}
		
		public delegate void PopoverEventHandler (object sender, PopoverEventArgs e);
		
		public event PopoverEventHandler ItemClicked;
		
		public ButtonStringPopover (UIButton b)
		{
			Button = b;
			
			_TableView = new UITableView();
			View.AddSubview(_TableView);
			
			this.TableView.Delegate = new ViewDelegate(this);
			this.TableView.DataSource = new ViewDataSource(this);
			_controller = new UIPopoverController(this);
		}
		
		public UITableView TableView
		{
			get
			{
				return _TableView;
			}
		}
		
		public UIView AccessoryView
		{
			get
			{
				return _AccessoryView;
			}
			set
			{
				if (_AccessoryView != null)
				{
					_AccessoryView.RemoveFromSuperview();
				}
				
				_AccessoryView = value;
				if (_AccessoryView != null && View != null)
				{
					View.AddSubview(_AccessoryView);
				}
			}
		}
		
		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
		}
		
		public UIButton Button
		{
			get
			{
				return _button;
			}
			set
			{
				if (_button != null)
				{
					
					_button.TouchUpInside -= HandleBTouchUpInside;
				}
				_button = value;
				if (_button != null)
				{
					
					_button.TouchUpInside += HandleBTouchUpInside;
				}
			}
		}

		void HandleBTouchUpInside (object sender, EventArgs e)
		{
            WillShowPopoverEventArgs ea = new WillShowPopoverEventArgs();
			if (WillShowPopover != null)
			{
				WillShowPopover(this, ea);
			}
			
			_CurrentItems = _Items;
			
			RecalcHeight();

            if (!ea.Cancel)
            {
			    _controller.PresentFromRect(_button.Frame, _button.Superview, UIPopoverArrowDirection.Any, true);
			    TableView.ReloadData();
            }
		}
		
		public void RecalcHeight()
		{
			nfloat height = 0;
			foreach (ButtonStringPopoverItem  item in _CurrentItems)
			{
				if (item.Text.IsEmptyOrNull())
				{
					height += _separatorHeight;
				}
				else
				{
					height += _rowHeight;
				}
			}
			
			nfloat width = 200;
			
			if (_AccessoryView != null)
			{
                width += new nfloat(Math.Min((double)_AccessoryView.Frame.Width, 400d));
                height = new nfloat(Math.Max(height, (double)_AccessoryView.Frame.Height));
			}
			
            height = new nfloat(Math.Min(height, 400));
			
			_controller.SetPopoverContentSize(new CGSize(width, height), true);
			
			if (_AccessoryView != null)
			{
				CGRect rect = _AccessoryView.Frame;
				rect.X = 0;
				rect.Y = 0;
				rect.Width = width-200;
				rect.Height = height;
				_AccessoryView.Frame = rect;
				
			
			}
			
			CGRect tbRect = new CGRect();
			tbRect.X = width-200;
			tbRect.Y = 0;
			tbRect.Width = 200;
			tbRect.Height = height;
			_TableView.Frame = tbRect;
			
		}
	
		
		
		public UIImage GetCellIcon(int row)
		{
			if (row < _CurrentItems.Count &&
			    _CurrentItems[row].Icon != null && _CurrentItems[row].Icon.Length > 0)
			{
				return UIExtensions.GetSmallIcon(_CurrentItems[row].Icon);
			}
			
			return null;
		}
		
		
		public object Data {get; set;}
		
		public List<ButtonStringPopoverItem> Items
		{
			get
			{
				return _Items;
			}
		}
		
		public bool SetButtonText
		{
			get
			{
				return _SetButtonText;
			}
			set
			{
				_SetButtonText = value;
			
			}
		}
		
			
		
		private class ViewDataSource : UITableViewDataSource
		{
			ButtonStringPopover state;
			public ViewDataSource(ButtonStringPopover state)	
			{
				this.state = state;
				
			}
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return state._CurrentItems.Count;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = new UITableViewCell (UITableViewCellStyle.Default, "ButtonStringPopover");
				
				ButtonStringPopoverItem item = state._CurrentItems[indexPath.Row];
				
				if (item.Text.IsEmptyOrNull())
				{
					UIView bview = new UIView();
					bview.BackgroundColor = UIExtensions.RGBColor(0xCCCCCC);
					cell.BackgroundView = bview;
					bview = new UIView();
					bview.BackgroundColor = UIExtensions.RGBColor(0xCCCCCC);
					cell.SelectedBackgroundView = bview;
					cell.Tag = 1;
				}
				else
				{
					cell.TextLabel.Text = item.Text;
					cell.TextLabel.Font = UIFont.SystemFontOfSize(14);
					if (item.Disabled)
					{
						cell.TextLabel.TextColor = UIColor.LightGray;	
					}
					
					UIImage image = state.GetCellIcon(indexPath.Row);
                    if (item.Selected)
                    {
                        image = UIExtensions.GetSmallIcon("check");
                    }
					cell.ImageView.Image = image;
					cell.ImageView.Frame = new CGRect(0, 0, 16, 16);

				}
				
	
				return cell;			
			}
			
		}
		
		private class ViewDelegate : UITableViewDelegate
		{
			ButtonStringPopover state;
			public ViewDelegate(ButtonStringPopover state)	
			{
				this.state = state;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				ButtonStringPopoverItem clickedItem = state._CurrentItems[indexPath.Row];
				
				string text = clickedItem.Text;
				
				//if not a separator
				if (!text.IsEmptyOrNull() && !clickedItem.Disabled)
				{
					if (clickedItem.Subitems != null && clickedItem.Subitems.Count > 0)
					{
						state._CurrentItems = clickedItem.Subitems;
						state.RecalcHeight();
						state.TableView.ReloadData();
					}
					else
					{
						
						if (state != null)
						{
							if (state.ItemClicked != null)
							{
								int index = indexPath.Row;
								object tag = null;
								if (index < state._CurrentItems.Count)
								{
									tag = state._CurrentItems[indexPath.Row].Tag;
								}
								
								state.ItemClicked(state, new PopoverEventArgs(text, index, tag));
							}
							
							if (state.SetButtonText)
							{
								state.Button.SetText(text);
							}
						}
						state._controller.Dismiss(true);
					}
				}
			}
			
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{				
				if (state._CurrentItems[indexPath.Row].Text.IsEmptyOrNull())
				{
					return state._separatorHeight;
				}
				return state._rowHeight;
			}
			
		}
		
		
	}
}

