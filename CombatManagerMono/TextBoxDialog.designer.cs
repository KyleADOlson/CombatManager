// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("TextBoxDialog")]
	partial class TextBoxDialog
	{
		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView HeaderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView TextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField TextField { get; set; }

		[Action ("OKButtonTouchUpInside:")]
		partial void OKButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("CancelButtonTouchUpInside:")]
		partial void CancelButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
	}
}
