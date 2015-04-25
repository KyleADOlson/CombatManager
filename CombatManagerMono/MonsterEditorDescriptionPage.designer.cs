// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("MonsterEditorDescriptionPage")]
	partial class MonsterEditorDescriptionPage
	{
		[Outlet]
		CombatManagerMono.GradientButton EnvironmentButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OrganizationButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton TreasureButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton BeforeCombatButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DuringCombatButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton MoraleButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton VisualDescriptionButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DescriptionButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (EnvironmentButton != null) {
				EnvironmentButton.Dispose ();
				EnvironmentButton = null;
			}

			if (OrganizationButton != null) {
				OrganizationButton.Dispose ();
				OrganizationButton = null;
			}

			if (TreasureButton != null) {
				TreasureButton.Dispose ();
				TreasureButton = null;
			}

			if (BeforeCombatButton != null) {
				BeforeCombatButton.Dispose ();
				BeforeCombatButton = null;
			}

			if (DuringCombatButton != null) {
				DuringCombatButton.Dispose ();
				DuringCombatButton = null;
			}

			if (MoraleButton != null) {
				MoraleButton.Dispose ();
				MoraleButton = null;
			}

			if (VisualDescriptionButton != null) {
				VisualDescriptionButton.Dispose ();
				VisualDescriptionButton = null;
			}

			if (DescriptionButton != null) {
				DescriptionButton.Dispose ();
				DescriptionButton = null;
			}
		}
	}
}
