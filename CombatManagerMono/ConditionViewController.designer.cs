// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("ConditionViewController")]
	partial class ConditionViewController
	{
		[Outlet]
		CombatManagerMono.GradientView TopView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BottomView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView SelectionTable { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField FilterText { get; set; }

		[Outlet]
		CombatManagerMono.GradientView TitleView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIStepper DurationStepper { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ConditionsTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpellsTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AfflictionsTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CustomTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton FavoritesTab { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIWebView ConditionDetailWebView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ApplyButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CloseButton { get; set; }

		[Action ("StepperValueChanged:")]
		partial void StepperValueChanged (MonoTouch.UIKit.UIStepper sender);

		[Action ("TabButtonClicked:")]
		partial void TabButtonClicked (MonoTouch.UIKit.UIButton sender);

		[Action ("ApplyButtonClicked:")]
		partial void ApplyButtonClicked (MonoTouch.UIKit.UIButton sender);

		[Action ("CloseButtonClicked:")]
		partial void CloseButtonClicked (MonoTouch.Foundation.NSObject sender);
	}
}
