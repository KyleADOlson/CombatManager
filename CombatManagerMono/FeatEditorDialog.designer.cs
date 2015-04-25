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
	[Register ("FeatEditorDialog")]
	partial class FeatEditorDialog
	{
		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton BenefitButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NameButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NormalButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton PrerequisitesButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpecialButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton TypesButton { get; set; }

		[Action ("CancelButtonClicked:")]
		partial void CancelButtonClicked (Foundation.NSObject sender);

		[Action ("OKButtonClicked:")]
		partial void OKButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BenefitButton != null) {
				BenefitButton.Dispose ();
				BenefitButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (NameButton != null) {
				NameButton.Dispose ();
				NameButton = null;
			}

			if (NormalButton != null) {
				NormalButton.Dispose ();
				NormalButton = null;
			}

			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}

			if (PrerequisitesButton != null) {
				PrerequisitesButton.Dispose ();
				PrerequisitesButton = null;
			}

			if (SpecialButton != null) {
				SpecialButton.Dispose ();
				SpecialButton = null;
			}

			if (TypesButton != null) {
				TypesButton.Dispose ();
				TypesButton = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}
		}
	}
}
