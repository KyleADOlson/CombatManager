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
    [Register ("HDEditorDialog")]
    partial class HDEditorDialog
    {
        [Outlet]
        CombatManagerMono.GradientView BackgroundView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView HeaderView { get; set; }


        [Outlet]
        UIKit.UILabel HeaderLabel { get; set; }


        [Outlet]
        CombatManagerMono.GradientView ModView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OKButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton CancelButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton ModButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton AddDieButton { get; set; }


        [Outlet]
        UIKit.UITableView DieTableView { get; set; }


        [Action ("AddDieButtonTouchUpInside:")]
        partial void AddDieButtonTouchUpInside (Foundation.NSObject sender);


        [Action ("ModButtonTouchUpInside:")]
        partial void ModButtonTouchUpInside (Foundation.NSObject sender);


        [Action ("OKButtonTouchUpInside:")]
        partial void OKButtonTouchUpInside (Foundation.NSObject sender);


        [Action ("CancelButtonTouchUpInside:")]
        partial void CancelButtonTouchUpInside (Foundation.NSObject sender);

        [Action ("CancelButtonTouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonTouchUpInside (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AddDieButton != null) {
                AddDieButton.Dispose ();
                AddDieButton = null;
            }

            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (DieTableView != null) {
                DieTableView.Dispose ();
                DieTableView = null;
            }

            if (HeaderLabel != null) {
                HeaderLabel.Dispose ();
                HeaderLabel = null;
            }

            if (HeaderView != null) {
                HeaderView.Dispose ();
                HeaderView = null;
            }

            if (ModButton != null) {
                ModButton.Dispose ();
                ModButton = null;
            }

            if (ModView != null) {
                ModView.Dispose ();
                ModView = null;
            }

            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }
        }
    }
}