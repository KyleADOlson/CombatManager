// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("NumberModifyPopover")]
	partial class NumberModifyPopover
	{
		[Outlet]
		CombatManagerMono.GradientButton subtractButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton setButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton addButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel numberLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPickerView pickerView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NullButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NumPadLabel { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton1 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton2 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton3 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton4 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton5 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton6 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton7 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton8 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton9 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton0 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ClearButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView NumPadView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SwapButton { get; set; }

		[Action ("SwapButtonTouchUpInside:")]
		partial void SwapButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("NumberTouchUpInside:")]
		partial void NumberTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("ClearButtonTouchUpInside:")]
		partial void ClearButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("SubtractClicked:")]
		partial void SubtractClicked (MonoTouch.Foundation.NSObject sender);

		[Action ("SetClicked:")]
		partial void SetClicked (MonoTouch.Foundation.NSObject sender);

		[Action ("AddClicked:")]
		partial void AddClicked (MonoTouch.Foundation.NSObject sender);

		[Action ("NullButtonTouchUpInside:")]
		partial void NullButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
	}
}
