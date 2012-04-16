// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("MonsterEditorDialog")]
	partial class MonsterEditorDialog
	{
		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView HeaderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PageView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView PageBorderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton BasicButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DefenseButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OffenseButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton StatisticsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton FeatsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpecialButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DescriptionButton { get; set; }

		[Action ("TabButtonTouchUpInside:")]
		partial void TabButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("OKButtonTouchUpInside:")]
		partial void OKButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("CancelButtonTouchUpInside:")]
		partial void CancelButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
	}
}
