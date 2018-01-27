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
    [Register ("SpellLevelsDialog")]
    partial class SpellLevelsDialog
    {
        [Outlet]
        CombatManagerMono.GradientButton AddButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientView BackgroundView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton CancelButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton ClassButton { get; set; }


        [Outlet]
        UIKit.UITableView LevelsTable { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OKButton { get; set; }


        [Action ("AddButtonClicked:")]
        partial void AddButtonClicked (Foundation.NSObject sender);


        [Action ("CancelButtonClicked:")]
        partial void CancelButtonClicked (Foundation.NSObject sender);


        [Action ("OkButtonClicked:")]
        partial void OkButtonClicked (Foundation.NSObject sender);

        [Action ("CancelButtonClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonClicked (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AddButton != null) {
                AddButton.Dispose ();
                AddButton = null;
            }

            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (ClassButton != null) {
                ClassButton.Dispose ();
                ClassButton = null;
            }

            if (LevelsTable != null) {
                LevelsTable.Dispose ();
                LevelsTable = null;
            }

            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }
        }
    }
}