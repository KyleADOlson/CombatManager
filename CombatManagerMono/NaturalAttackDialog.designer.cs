// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("NaturalAttackDialog")]
	partial class NaturalAttackDialog
	{
		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView HeaderView { get; set; }

		[Outlet]
		UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AttackButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DamageButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CountButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton PlusButton { get; set; }

		[Action ("OKButtonClicked:")]
		partial void OKButtonClicked (Foundation.NSObject sender);

		[Action ("CancelButtonClicked:")]
		partial void CancelButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (HeaderView != null) {
				HeaderView.Dispose ();
				HeaderView = null;
			}

			if (HeaderLabel != null) {
				HeaderLabel.Dispose ();
				HeaderLabel = null;
			}

			if (AttackButton != null) {
				AttackButton.Dispose ();
				AttackButton = null;
			}

			if (DamageButton != null) {
				DamageButton.Dispose ();
				DamageButton = null;
			}

			if (CountButton != null) {
				CountButton.Dispose ();
				CountButton = null;
			}

			if (PlusButton != null) {
				PlusButton.Dispose ();
				PlusButton = null;
			}
		}
	}
}
