using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using CombatManager;

namespace CombatManagerMono
{
	public abstract class CMTab : UIView
	{
		
		CombatState _CombatState;
		
		public CMTab (CombatState state)
		{
			CombatState = state;
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

	}
}

