// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("AboutView")]
	partial class AboutView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton OKButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIWebView WebView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}

			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}
		}
	}
}
