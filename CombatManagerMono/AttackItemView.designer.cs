// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("AttackItemView")]
	partial class AttackItemView
	{
		[Outlet]
		CombatManagerMono.GradientView HandView { get; set; }

		[Outlet]
		UIKit.UILabel HandLabel { get; set; }

		[Outlet]
		CombatManagerMono.GradientView NameView { get; set; }

		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DeleteButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton BonusButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpecialButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HandView != null) {
				HandView.Dispose ();
				HandView = null;
			}

			if (HandLabel != null) {
				HandLabel.Dispose ();
				HandLabel = null;
			}

			if (NameView != null) {
				NameView.Dispose ();
				NameView = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}

			if (BonusButton != null) {
				BonusButton.Dispose ();
				BonusButton = null;
			}

			if (SpecialButton != null) {
				SpecialButton.Dispose ();
				SpecialButton = null;
			}
		}
	}
}
