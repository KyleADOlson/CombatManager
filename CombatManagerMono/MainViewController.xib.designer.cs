// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIWindow window { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView LoadingView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (LoadingView != null) {
				LoadingView.Dispose ();
				LoadingView = null;
			}
		}
	}
}
