// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CombatManagerMono
{
	[Register ("SpellEditorDialog")]
	partial class SpellEditorDialog
	{
		[Outlet]
		CombatManagerMono.GradientButton AreaButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AuraButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CastingTimeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DescriptionButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DescriptorButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DismissableButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DivineFocusButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DurationButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton FocusButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton FocusTextButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton LevelsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton MaterialButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton MaterialTextButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NameButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RangeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SavingThrowButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SchoolButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SomaticButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpellResistanceButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SubschoolButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton TargetsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton VerbalButton { get; set; }

		[Action ("OnCancelButtonClicked:")]
		partial void OnCancelButtonClicked (Foundation.NSObject sender);

		[Action ("OnOkButtonClicked:")]
		partial void OnOkButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AreaButton != null) {
				AreaButton.Dispose ();
				AreaButton = null;
			}

			if (AuraButton != null) {
				AuraButton.Dispose ();
				AuraButton = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (CastingTimeButton != null) {
				CastingTimeButton.Dispose ();
				CastingTimeButton = null;
			}

			if (DescriptorButton != null) {
				DescriptorButton.Dispose ();
				DescriptorButton = null;
			}

			if (DismissableButton != null) {
				DismissableButton.Dispose ();
				DismissableButton = null;
			}

			if (DurationButton != null) {
				DurationButton.Dispose ();
				DurationButton = null;
			}

			if (NameButton != null) {
				NameButton.Dispose ();
				NameButton = null;
			}

			if (RangeButton != null) {
				RangeButton.Dispose ();
				RangeButton = null;
			}

			if (SavingThrowButton != null) {
				SavingThrowButton.Dispose ();
				SavingThrowButton = null;
			}

			if (SchoolButton != null) {
				SchoolButton.Dispose ();
				SchoolButton = null;
			}

			if (SpellResistanceButton != null) {
				SpellResistanceButton.Dispose ();
				SpellResistanceButton = null;
			}

			if (SubschoolButton != null) {
				SubschoolButton.Dispose ();
				SubschoolButton = null;
			}

			if (MaterialButton != null) {
				MaterialButton.Dispose ();
				MaterialButton = null;
			}

			if (MaterialTextButton != null) {
				MaterialTextButton.Dispose ();
				MaterialTextButton = null;
			}

			if (DescriptionButton != null) {
				DescriptionButton.Dispose ();
				DescriptionButton = null;
			}

			if (FocusButton != null) {
				FocusButton.Dispose ();
				FocusButton = null;
			}

			if (DivineFocusButton != null) {
				DivineFocusButton.Dispose ();
				DivineFocusButton = null;
			}

			if (FocusTextButton != null) {
				FocusTextButton.Dispose ();
				FocusTextButton = null;
			}

			if (TargetsButton != null) {
				TargetsButton.Dispose ();
				TargetsButton = null;
			}

			if (LevelsButton != null) {
				LevelsButton.Dispose ();
				LevelsButton = null;
			}

			if (VerbalButton != null) {
				VerbalButton.Dispose ();
				VerbalButton = null;
			}

			if (SomaticButton != null) {
				SomaticButton.Dispose ();
				SomaticButton = null;
			}
		}
	}
}
