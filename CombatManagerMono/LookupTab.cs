/*
 *  LookupTab.cs
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
using System.Drawing;
using CombatManager;


namespace CombatManagerMono
{
	public abstract class LookupTab<T> : CMTab
	{
		UITableView listTable;
		protected UITextField filterField;
		UIWebView webView;
		UIView _FilterView;
		UIView _BottomView;
		GradientButton _ResetButton;
		
		float _BottomViewHeight;
		
		List<T> sortedItems;
		List<T> currentViewItems;
		string filterText;
		
		T _selectedItem;
		T _displayItem;
		
		
		public LookupTab (CombatState state) : base(state)
		{			
			BackgroundColor = CMUIColors.PrimaryColorDark;
			
			listTable = new UITableView();
			listTable.Delegate = new TableDelegate(this);
			listTable.DataSource = new TableSource(this);
			AddSubview(listTable);
			filterField = new UITextField(new Rectangle(0, 0, 100, 23));
			filterField.BorderStyle = UITextBorderStyle.RoundedRect;
			filterField.EditingChanged += HandleFilterFieldEditingChanged;
			filterField.AllEditingEvents += HandleFilterFieldAllEditingEvents;
			filterField.ClearButtonMode = UITextFieldViewMode.Always;
			AddSubview(filterField);
			filterText = "";
			_FilterView = new UIView(new RectangleF(0, 0, 100, 0));
			_FilterView.BackgroundColor = CMUIColors.PrimaryColorDark;
			AddSubview(_FilterView);
			_ResetButton = new GradientButton();
			_ResetButton.TouchUpInside += Handle_ResetButtonTouchUpInside;
			_ResetButton.SetImage(UIExtensions.GetSmallIcon("reset"), UIControlState.Normal);
			_FilterView.AddSubview(_ResetButton);
			
			
			
			
			Filter();
			webView = new UIWebView(new RectangleF(100, 0, 100, 100));
			AddSubview(webView);
			
			if (currentViewItems.Count > 0)
			{
				SetSelectedItem(0);
			}
			
			
				
		}

		void Handle_ResetButtonTouchUpInside (object sender, EventArgs e)
		{
			ResetButtonClicked();
		}
		
		void HandleFilterFieldAllEditingEvents (object sender, EventArgs e)
		{
			
			if (filterText != filterField.Text.Trim())
			{
				filterText = filterField.Text.Trim();
				
				Filter();
			}
		}

		void HandleFilterFieldEditingChanged (object sender, EventArgs e)
		{
		}
		
		void ShowItem(T item)
		{
			if (CompareItems(item, default(T)))
			{
				webView.LoadHtmlString("", new NSUrl("http://localhost/"));
				_displayItem = item;
			}
			else
			{
				_displayItem = ModifiedItem(item);
				webView.LoadHtmlString(ItemHtml(_displayItem), new NSUrl("http://localhost/"));
			}
		}
		
		protected virtual T ModifiedItem(T item)
		{
			return item;
		}
		
		protected void ModifiedItemChanged()
		{
			ShowItem (_selectedItem);
		}

		
		protected void Filter()
		{
			if (sortedItems == null)
			{
				CreateSortedItems();
			}
			
			currentViewItems = new List<T>();
			
			foreach (T m in sortedItems)
			{
				if ((filterText == "" || ItemFilterText(m).ToLower().Contains(filterText.ToLower())) && ItemFilter(m))
				{
					currentViewItems.Add(m);	
				}
			}
			
			listTable.ReloadData();
		}
		
		private void CreateSortedItems()
		{
			
			sortedItems = new List<T>();
			sortedItems.AddRange(ItemsSource);
			sortedItems.Sort((a, b) => SortText(a).CompareTo(SortText(b)));
		}
		
		protected abstract bool ItemFilter(T item);
		
		protected abstract string ItemFilterText(T item);
		
		protected abstract string SortText(T item);
		protected abstract string DisplayText(T item);
		
		protected abstract string ItemHtml(T item);
		
		protected abstract bool CompareItems(T item1, T item2);
		
		protected virtual void ResetButtonClicked()
		{
			
		}
		
		protected abstract IEnumerable<T> ItemsSource
		{
			get;
		}
		
		public override void DidChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey)
		{
			base.DidChange (changeKind, indexes, forKey);
			
		}
		
		protected virtual int SideWidth
		{
			get
			{
				return 275;
			}
		}
		
		protected virtual int FilterHeight
		{
			get
			{
				return 40;
			}
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			RectangleF rect = ConvertRectFromView(Frame, Superview);
			
			filterField.SetLocation(5, 5);
			filterField.SetWidth(SideWidth -10);
			filterField.SetHeight(30);
			
			listTable.SetLocation(0, 40);
			listTable.SetWidth(SideWidth-5);
			listTable.SetHeight(rect.Height-25);
			
			
			
			_FilterView.Frame = new RectangleF(SideWidth, 0,  rect.Width-SideWidth, FilterHeight);
			
			float webViewHeight = rect.Height-FilterHeight;
			
			if (_BottomViewHeight > 0 && BottomView != null)
			{
				webViewHeight -= _BottomViewHeight;	
			}
			
			 webView.Frame = new RectangleF(SideWidth, FilterHeight, rect.Width-SideWidth, webViewHeight);
			
			if (BottomView != null)
			{
				RectangleF bottomFrame = webView.Frame;
				bottomFrame.Y = bottomFrame.Y + bottomFrame.Height;
				bottomFrame.Height = _BottomViewHeight;
				_BottomView.Frame = bottomFrame;
			}
			
			_ResetButton.SetLocation(FilterView.Frame.Width - 40 , 5);
			_ResetButton.SetWidth(30);
			_ResetButton.SetHeight(30);
			
			
			
		}
		
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);
		
			
			
		}
		
		protected void StyleFilterButton(GradientButton b)
		{
			b.Font = UIFont.SystemFontOfSize(14);
		}
		
		private class TableDelegate : UITableViewDelegate
		{
			LookupTab<T> tab;
			
			public TableDelegate(LookupTab<T> tab)
			{
				this.tab = tab;
			}
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (tab!= null)
				{
					return 26;
				}
				return 26;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				tab.SetSelectedItem(indexPath.Row);
			}
			
		}
		
		private void SetSelectedItem(int row)
		{
			T m = currentViewItems[row];
			
			if (!CompareItems(m, _selectedItem))
			{
				_selectedItem = m;
			
				ShowItem(_selectedItem);
			}
		}
		
		private class TableSource : UITableViewDataSource
		{
			LookupTab<T> tab;
			
			public TableSource(LookupTab<T> tab)
			{
				this.tab = tab;
			}
			
			public override int RowsInSection (UITableView tableView, int section)
			{
				return tab.currentViewItems.Count;
			}
		
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				int row = indexPath.Row;
				
				UITableViewCell cell = tableView.DequeueReusableCell("UICell");
				
				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, "UICell");
				}
				
				cell.TextLabel.Text = tab.DisplayText(tab.currentViewItems[row]);
				cell.TextLabel.Font = UIFont.SystemFontOfSize(15);
				
				return cell;
			}
			
		}
		
		public UIView FilterView
		{
			get
			{
				return _FilterView;
			}
		}
		
		public UIView BottomView
		{
			get
			{
				return _BottomView;
			}
			set
			{
				if (_BottomView != value)
				{
					if (_BottomView != null)
					{
						_BottomView.RemoveFromSuperview();
					}
					
					_BottomView = value;
					
					if (_BottomView != null)
					{
					
						AddSubview(_BottomView);
					}
				}
				
			}
		}
		
		public float BottomViewHeight
		{
			get
			{
				return _BottomViewHeight;
			}
			set
			{
				_BottomViewHeight = value;
			}
			
		}
		
		public T DisplayItem
		{
			get
			{
				return _displayItem;
			}
		}
	}
}

