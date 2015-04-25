/*
 *  MonsterEditorPage.cs
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
	[Register("MonsterEditorPage")]
	public class MonsterEditorPage : UIViewController
	{
		
		private Monster _Monster;
		private UIView _DialogParent;
		
		private List<ButtonPropertyManager> _PropertyManagers = new List<ButtonPropertyManager>();
		
		public MonsterEditorPage() : base ()
		{
			Initialize();
		}
		
		public MonsterEditorPage(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		public MonsterEditorPage(NSObjectFlag t) : base(t)
		{
			Initialize();
		}
		
		public MonsterEditorPage(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		
		
		public MonsterEditorPage (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			
		}
		
		protected void StylePanel(GradientView panel)
		{
            CMStyles.StyleBasicPanel(panel);
		}
		
		
		public Monster CurrentMonster
		{
			get
			{
				return _Monster;
			}
			set
			{
				if (_Monster != value)
				{
					Monster oldMonster = _Monster;
					_Monster = value;
					
					MonsterUpdated(oldMonster, _Monster);
				}
			}
			
		}
		
		public virtual void MonsterUpdated(Monster oldMonster, Monster newMonster)
		{
			
		}	
		
		
			
		public UIView DialogParent
		{
			get
			{
				return _DialogParent;
			}
			set
			{
				_DialogParent = value;
			}
		}
		
		public List<ButtonPropertyManager> PropertyManagers
		{
			get
			{
				return _PropertyManagers;
			}
		}
		
	}
}

