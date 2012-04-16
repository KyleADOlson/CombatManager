using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CombatManager;
using System.ComponentModel;

namespace CombatManagerMono
{
	public class DataListViewCell : UITableViewCell
	{
		public object Data {get; set;}
		
		public DataListViewCell (UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
		{
		}
	}
}

