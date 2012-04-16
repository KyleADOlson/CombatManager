using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CombatManagerMono
{
	
	[Register ("CMDialogController")]
	public class CMDialogController : UIViewController
	{
		public CMDialogController() : base ()
		{
			Initialize();
		}
		
		public CMDialogController(NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		public CMDialogController(NSObjectFlag t) : base(t)
		{
			Initialize();
		}
		
		public CMDialogController(IntPtr handle) : base(handle)
		{
			Initialize();
		}
		
		
		
		public CMDialogController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Initialize();
		}
		
		private void Initialize()
		{
			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}
		
		
		[Outlet]
		public CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		public CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		public CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		public CombatManagerMono.GradientView TitleGradient { get; set; }

		[Action ("CancelButtonClicked:")]
		public void CancelButtonClicked (MonoTouch.Foundation.NSObject sender)
		{
			if (HandleCancel())
			{
				View.RemoveFromSuperview();
			}
		}

		[Action ("OKButtonClicked:")]
		public void OKButtonClicked (MonoTouch.Foundation.NSObject sender)
		{
			if (HandleOK())
			{
				View.RemoveFromSuperview();
			}
		}
		
		public virtual bool HandleOK()
		{
			return true;	
		}
		
		public virtual bool HandleCancel()
		{
			return true;	
		}
	}
}

