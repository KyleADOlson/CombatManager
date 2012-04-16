using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace CombatManagerMono
{
	[Register ("DialogPopoverViewController")]
	public class DialogPopoverViewController : UIViewController
	{
		UIPopoverController _PopoverController;
		
		public DialogPopoverViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public DialogPopoverViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public DialogPopoverViewController (string name, NSBundle bundle) : base(name, bundle)
		{
			Initialize ();
		}

		void Initialize ()
		{
			_PopoverController = new UIPopoverController(this);
		}
		
		public void ShowOnView(UIView view)
		{
			ParentView = view;
			Show();
		}
		
		public void Show()
		{
			_PopoverController.SetPopoverContentSize(PresentationSize, true);
			_PopoverController.PresentFromRect(ParentView.Frame, ParentView.Superview, UIPopoverArrowDirection.Any, true);
		}
		
		
		[MonoTouch.Foundation.Export("Close:")]
		public void Close (MonoTouch.UIKit.UIButton sender)
		{
			_PopoverController.Dismiss(true);	
		}
		
		public virtual SizeF PresentationSize
		{
			get
			{
				return View.Frame.Size;
			}
		}
		
		public object Data {get; set;}
		public UIView ParentView {get; set;}
		
		
	}
}

