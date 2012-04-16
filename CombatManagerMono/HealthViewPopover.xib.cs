
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CombatManagerMono
{
	public partial class HealthViewPopover : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public HealthViewPopover (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public HealthViewPopover (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public HealthViewPopover () : base("HealthViewPopover", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		
		
	}
}

