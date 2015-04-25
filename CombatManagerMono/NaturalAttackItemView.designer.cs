// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("NaturalAttackItemView")]
	partial class NaturalAttackItemView
	{
		[Outlet]
		UIKit.UIButton DeleteButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton TitleButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}

			if (TitleButton != null) {
				TitleButton.Dispose ();
				TitleButton = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}
		}
	}
}
