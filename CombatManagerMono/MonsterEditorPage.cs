using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CombatManager;
using System.ComponentModel;
using System.Drawing;


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
			panel.BackgroundColor = UIColor.Clear;
			panel.Gradient = new GradientHelper(CMUIColors.PrimaryColorDarker);
			panel.BorderColor = UIExtensions.RGBColor(0xFFFFFF);
			panel.Border = 2;
		}
		
		
		public Monster Monster
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

