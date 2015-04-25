/*
 *  MonsterAddView.xib.cs
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
using CoreGraphics;

namespace CombatManagerMono
{
	public partial class MonsterAddView : UIViewController
	{
		TableDataSource _tableSource;
		TableDelegate _tableDelegate;
		Monster _selectedMonster;
		List<Monster> _currentViewMonsters;
		List<Monster> _sortedMonsters ;
		string _filterText;
		
		public bool IsMonsters {get; set;}
		
		CombatState _CombatState;
		
		public event EventHandler ShouldClose;


		public MonsterAddView (IntPtr handle) : base(handle)
		{
			
			Initialize ();
		}
		
		private void FilterMonsters()
		{
			Monster oldM = _selectedMonster;
			
			if (_sortedMonsters == null)
			{
				_sortedMonsters = new List<Monster>();
				_sortedMonsters.AddRange(Monster.Monsters);
				_sortedMonsters.Sort((a, b) => a.Name.CompareTo(b.Name));
			}
			
			_currentViewMonsters = new List<Monster>();
			
			int index = 0;
			int selectedIndex = -1;
			foreach (Monster m in _sortedMonsters)
			{
				if (_filterText == "" || m.Name.ToLower().Contains(_filterText.ToLower()))
				{
					_currentViewMonsters.Add(m);	
					
					if (m == oldM)
					{
						selectedIndex = index;
					}
					index++;
				}
			}
			
			if (selectedIndex == -1 && _currentViewMonsters.Count > 0)
			{
				selectedIndex = 0;
			}
			
			monsterTable.ReloadData();
			if (selectedIndex != -1)
			{
				NSIndexPath path = NSIndexPath.FromRowSection(selectedIndex, 0);
				monsterTable.SelectRow(path, false, UITableViewScrollPosition.Top);
				_selectedMonster = _currentViewMonsters[selectedIndex];
		
			}
			else
			{
				_selectedMonster = null;
			}
		}

		[Export("initWithCoder:")]
		public MonsterAddView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MonsterAddView () : base("MonsterAddView", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			_filterText = "";
			_tableSource = new TableDataSource(this);
			_tableDelegate = new TableDelegate(this);

            ButtonAreaView.Gradient = new GradientHelper(CMUIColors.SecondaryColorADark);
            ButtonAreaView.Border = 0;
            ButtonAreaView.CornerRadius = 0;
			
			monsterTable.DataSource = _tableSource;
			monsterTable.Delegate = _tableDelegate;
			addButton.TouchUpInside += HandleAddButtonTouchUpInside;
			closeButton.TouchUpInside += HandleCloseButtonTouchUpInside;
			filterTextBox.ValueChanged += HandleFilterTextBoxValueChanged;
			filterTextBox.AllEditingEvents += HandleFilterTextBoxAllEditingEvents;
		}

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
        }

		void HandleFilterTextBoxAllEditingEvents (object sender, EventArgs e)
		{
			
			_filterText = filterTextBox.Text;
			FilterMonsters();
		}

		void HandleFilterTextBoxValueChanged (object sender, EventArgs e)
		{
			
			
		}

		void HandleCloseButtonTouchUpInside (object sender, EventArgs e)
		{
			if (ShouldClose != null)
			{
				ShouldClose(this, new EventArgs());
			}
		}

		void HandleAddButtonTouchUpInside (object sender, EventArgs e)
		{
			if (_selectedMonster != null && _CombatState != null)
			{
				_CombatState.AddMonster(_selectedMonster, true, IsMonsters);	
			}
		}
		
		public Monster SelectedMonster
		{
			get
			{
				return _selectedMonster;
			}
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
		
		public class TableDelegate : UITableViewDelegate
		{
			MonsterAddView parent;
			
			public TableDelegate(MonsterAddView parent)
			{
				this.parent = parent;
			}
			

			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if (parent!= null)
				{
					return 35;
				}
				return 35;
			}
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				parent._selectedMonster = parent._currentViewMonsters[indexPath.Row];
			
			}
		}
		
		public class TableDataSource : UITableViewDataSource
		{
			MonsterAddView parent;
			
			public TableDataSource(MonsterAddView parent)
			{
				this.parent = parent;
			}
		
			
			public override nint RowsInSection (UITableView tableView, nint section)
			{
				if (parent._currentViewMonsters == null)
				{
					parent.FilterMonsters();
				}
				return parent._currentViewMonsters.Count;
			}
		
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				int row = indexPath.Row;
				
				UITableViewCell cell = tableView.DequeueReusableCell("UICell");
				
				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, "UICell");
				}
				
				cell.TextLabel.Text = parent._currentViewMonsters[row].Name;
				cell.TextLabel.Font = UIFont.SystemFontOfSize(15);
				
				return cell;
			}
			
		}
		
	}
}

