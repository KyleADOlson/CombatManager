// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("AttacksEditorDialog")]
	partial class AttacksEditorDialog
	{
		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView AttackTextView { get; set; }

		[Outlet]
		UIKit.UITextView MeleeView { get; set; }

		[Outlet]
		UIKit.UITextView RangedView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView EditingView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton MeleeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RangedButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NaturalButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Action ("OKButtonClicked:")]
		partial void OKButtonClicked (Foundation.NSObject sender);

		[Action ("CancelButtonClicked:")]
		partial void CancelButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (AttackTextView != null) {
				AttackTextView.Dispose ();
				AttackTextView = null;
			}

			if (MeleeView != null) {
				MeleeView.Dispose ();
				MeleeView = null;
			}

			if (RangedView != null) {
				RangedView.Dispose ();
				RangedView = null;
			}

			if (EditingView != null) {
				EditingView.Dispose ();
				EditingView = null;
			}

			if (MeleeButton != null) {
				MeleeButton.Dispose ();
				MeleeButton = null;
			}

			if (RangedButton != null) {
				RangedButton.Dispose ();
				RangedButton = null;
			}

			if (NaturalButton != null) {
				NaturalButton.Dispose ();
				NaturalButton = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}
		}
	}
}
