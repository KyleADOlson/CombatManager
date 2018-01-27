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

        [Action ("CancelButtonClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonClicked (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AttackButton != null) {
                AttackButton.Dispose ();
                AttackButton = null;
            }

            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (CountButton != null) {
                CountButton.Dispose ();
                CountButton = null;
            }

            if (DamageButton != null) {
                DamageButton.Dispose ();
                DamageButton = null;
            }

            if (HeaderLabel != null) {
                HeaderLabel.Dispose ();
                HeaderLabel = null;
            }

            if (HeaderView != null) {
                HeaderView.Dispose ();
                HeaderView = null;
            }

            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }

            if (PlusButton != null) {
                PlusButton.Dispose ();
                PlusButton = null;
            }
        }
    }
}