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
using Foundation;
using UIKit;
using CoreGraphics;
using CombatManager;
using System.ComponentModel;


namespace CombatManagerMono
{
    
    public class LookupSideTabItem
    {
        public string Name {get; set;}
        public UIImage Icon {get; set;}
        public UIView View {get; set;}
    }


    public abstract class LookupTab<T> : CMTab
	{
		UITableView listTable;
		protected UITextField filterField;
		UIWebView webView;
		UIView _FilterView;
		UIView _BottomView;
		GradientButton _ResetButton;

        SideTabBar _SideTabBar;
		
		float _BottomViewHeight;
		
		List<T> sortedItems;
		List<T> currentViewItems;
		string filterText;
		
		T _selectedItem;
		T _displayItem;

        UIView _VisibleTabView;
		
		
		public LookupTab (CombatState state) : base(state)
		{			
			BackgroundColor = CMUIColors.PrimaryColorDark;
			
			listTable = new UITableView();
			listTable.Delegate = new TableDelegate(this);
			listTable.DataSource = new TableSource(this);
			AddSubview(listTable);
			filterField = new UITextField(new CGRect(0, 0, 100, 23));
			filterField.BorderStyle = UITextBorderStyle.RoundedRect;
			filterField.EditingChanged += HandleFilterFieldEditingChanged;
			filterField.AllEditingEvents += HandleFilterFieldAllEditingEvents;
			filterField.ClearButtonMode = UITextFieldViewMode.Always;
			AddSubview(filterField);
			filterText = "";
			_FilterView = new UIView(new CGRect(0, 0, 100, 0));
			_FilterView.BackgroundColor = CMUIColors.PrimaryColorDark;
			AddSubview(_FilterView);
			_ResetButton = new GradientButton();
			_ResetButton.TouchUpInside += Handle_ResetButtonTouchUpInside;
			_ResetButton.SetImage(UIExtensions.GetSmallIcon("reset"), UIControlState.Normal);
			_FilterView.AddSubview(_ResetButton);

            if (ShowSideBar)
            {
                _SideTabBar = new SideTabBar();
                _SideTabBar.TabSelected += HandleTabSelected;
                _SideTabBar.Hidden = true;
                _SideTabBar.AddTab(new SideTab {Name=DefaultTabName, Tag=null, Icon=DefaultTabImage});
                Add (_SideTabBar);

                List<LookupSideTabItem> tabItems = LoadTabItems();

                if (tabItems != null)
                {
                    foreach (LookupSideTabItem item in tabItems)
                    {
                        _SideTabBar.AddTab(new SideTab {Name=item.Name, Tag=item, Icon = item.Icon});
                    }
                }
            }
			
			
			
			Filter();
			webView = new UIWebView(new CGRect(100, 0, 100, 100));
			AddSubview(webView);
			
			if (currentViewItems.Count > 0)
			{
				SetSelectedItem(0);
			}
			
			
				
		}

        void HandleTabSelected (object sender, SideTabEventArgs e)
        {
            if (e.Tab.Tag != _VisibleTabView)
            {
                if (_VisibleTabView != null)
                {
                    _VisibleTabView.RemoveFromSuperview();
                    _VisibleTabView = null;
                }

                var v = ((LookupSideTabItem)e.Tab.Tag);

                if (v != null)
                {
                    _VisibleTabView = v.View;
                }

                if (_VisibleTabView != null)
                {
                    Add (_VisibleTabView);
                }



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
		
        protected virtual void ShowItem(T item)
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
            Filter(false);
        }
		
        protected void Filter(bool reload)
		{
            if (sortedItems == null || reload)
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

        protected virtual bool ShowSideBar
        {
            get
            {
                return false;
            }
        }

        protected virtual string DefaultTabName
        {
            get
            {
                return "";
            }
        }
        protected virtual UIImage DefaultTabImage
        {
            get
            {
                return null;
            }
        }


        protected virtual float SideTabWidth
        {
            get
            {
                return 60;
            }
        }

        protected virtual List<LookupSideTabItem> LoadTabItems()
        {
            return null;
        }

		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			CGRect rect = ConvertRectFromView(Frame, Superview);
			
            float sideTabSize = 0;
            if (ShowSideBar)
            {
                sideTabSize = SideTabWidth;
                _SideTabBar.Frame = new CGRect(0, 0,sideTabSize, rect.Height);
                _SideTabBar.Hidden = false;
            }

			filterField.SetLocation(sideTabSize + 5, 5);
			filterField.SetWidth(SideWidth -10);
			filterField.SetHeight(30);
			
			listTable.SetLocation(sideTabSize, 40);
			listTable.SetWidth(SideWidth-5);
            listTable.SetHeight((float)rect.Height-25);
			

            float xLoc = (float)listTable.Frame.Right + 0.5f;

			_FilterView.Frame = new CGRect(xLoc + 10.0f, 0,  rect.Width-xLoc-10.0f, FilterHeight);
			
            float webViewHeight = (float)rect.Height-FilterHeight;
			
			if (_BottomViewHeight > 0 && BottomView != null)
			{
				webViewHeight -= _BottomViewHeight;	
			}
			
			 webView.Frame = new CGRect(xLoc, FilterHeight, rect.Width-xLoc, webViewHeight);
			
			if (BottomView != null)
			{
				CGRect bottomFrame = webView.Frame;
				bottomFrame.Y = bottomFrame.Y + bottomFrame.Height;
				bottomFrame.Height = _BottomViewHeight;
				_BottomView.Frame = bottomFrame;
			}
			
            _ResetButton.SetLocation((float)FilterView.Frame.Width - 40 , 5);
			_ResetButton.SetWidth(30);
			_ResetButton.SetHeight(30);

            if (ShowSideBar)
            {

                CGRect rectTabView = new CGRect(_SideTabBar.Frame.Right, _SideTabBar.Frame.Top, Bounds.Width - _SideTabBar.Frame.Width, Bounds.Height);

                foreach (var view in from x in _SideTabBar.Tabs where ((LookupSideTabItem)x.Tag) != null select ((LookupSideTabItem)x.Tag).View)
                {
                    if (view != null)
                    {
                        view.Frame = rectTabView;
                    }
                }
            }
		}
		
		public override void Draw (CGRect rect)
		{
			base.Draw (rect);
		
			
			
		}
		
		protected void StyleFilterButton(GradientButton b)
		{
			b.Font = UIFont.SystemFontOfSize(14);
		}


        protected void StyleDBButton(GradientButton b)
        {
            b.Font = UIFont.SystemFontOfSize(14);
            b.Gradient = new GradientHelper(CMUIColors.SecondaryColorBMedium, CMUIColors.SecondaryColorBDarker);
        }
		
		private class TableDelegate : UITableViewDelegate
		{
			LookupTab<T> tab;
			
			public TableDelegate(LookupTab<T> tab)
			{
				this.tab = tab;
			}
			
			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
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
			
			public override nint RowsInSection (UITableView tableView, nint section)
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

