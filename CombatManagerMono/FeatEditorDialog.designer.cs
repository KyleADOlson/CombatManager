// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
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

        [Action ("CancelButtonClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonClicked (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

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
        }
    }
}