// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("MonsterEditorSpecialPage")]
	partial class MonsterEditorSpecialPage
	{
		[Outlet]
		UIKit.UIScrollView SpecialScrollView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AddAbilityButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SpecialScrollView != null) {
				SpecialScrollView.Dispose ();
				SpecialScrollView = null;
			}

			if (AddAbilityButton != null) {
				AddAbilityButton.Dispose ();
				AddAbilityButton = null;
			}
		}
	}
}
