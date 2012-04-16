// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("AdvancerPanel")]
	partial class AdvancerPanel
	{
		[Outlet]
		CombatManagerMono.GradientButton AddMonsterButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView RacialView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView SimpleView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView OtherView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView SimpleHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView RacialHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView OtherHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AdvancedButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SimpleSizeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OutsiderButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AugmentSummoningButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialHitDieButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialBonusButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialSizeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OtherTemplateButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView AdvancerHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ResetButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OtherTemplateOptionView { get; set; }

		[Action ("AddMonsterButtonTouchUpInside:")]
		partial void AddMonsterButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("AdvancerHeaderButtonTouchUpInside:")]
		partial void AdvancerHeaderButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("ResetButtonTouchUpInside:")]
		partial void ResetButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
	}
}
