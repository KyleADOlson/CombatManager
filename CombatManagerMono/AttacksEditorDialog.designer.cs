// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

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

        [Action ("CancelButtonClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonClicked (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AttackTextView != null) {
                AttackTextView.Dispose ();
                AttackTextView = null;
            }

            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (EditingView != null) {
                EditingView.Dispose ();
                EditingView = null;
            }

            if (MeleeButton != null) {
                MeleeButton.Dispose ();
                MeleeButton = null;
            }

            if (MeleeView != null) {
                MeleeView.Dispose ();
                MeleeView = null;
            }

            if (NaturalButton != null) {
                NaturalButton.Dispose ();
                NaturalButton = null;
            }

            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }

            if (RangedButton != null) {
                RangedButton.Dispose ();
                RangedButton = null;
            }

            if (RangedView != null) {
                RangedView.Dispose ();
                RangedView = null;
            }
        }
    }
}