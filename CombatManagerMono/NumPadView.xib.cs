
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CombatManagerMono
{
	
	public class NumPadEventArgs
	{
		public int Key {get; set; }
	}
	
	public delegate void NumPadEventHandler(object sender, NumPadEventArgs e);
	
	public partial class NumPadView : UIViewController
	{
		public event NumPadEventHandler NumberPressed;
		public event EventHandler EnterPressed;
		public event EventHandler DeletePressed;
		
		public NumPadView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public NumPadView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public NumPadView () : base("NumPadView", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		partial void numberPressed (MonoTouch.Foundation.NSObject sender)
		{
			if (NumberPressed != null)
			{
				NumberPressed(this, new NumPadEventArgs() {Key = ((UIButton)sender).Tag});
			}
		}

		partial void delPressed (MonoTouch.Foundation.NSObject sender)
		{
			if (DeletePressed != null)
			{
				DeletePressed(this, new EventArgs());
			}
		}

		partial void enterPressed (MonoTouch.Foundation.NSObject sender)
		{
			if (EnterPressed != null)
			{
				EnterPressed(this, new EventArgs());
			}
		}
		
	}
}

