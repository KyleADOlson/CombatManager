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
    [Register ("TextBoxDialog")]
    partial class TextBoxDialog
    {
        [Outlet]
        CombatManagerMono.GradientView BackgroundView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView HeaderView { get; set; }


        [Outlet]
        UIKit.UILabel HeaderLabel { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton CancelButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OKButton { get; set; }


        [Outlet]
        UIKit.UITextView TextView { get; set; }


        [Outlet]
        UIKit.UITextField TextField { get; set; }


        [Action ("OKButtonTouchUpInside:")]
        partial void OKButtonTouchUpInside (Foundation.NSObject sender);


        [Action ("CancelButtonTouchUpInside:")]
        partial void CancelButtonTouchUpInside (Foundation.NSObject sender);

        [Action ("CancelButtonTouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonTouchUpInside (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
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

            if (TextField != null) {
                TextField.Dispose ();
                TextField = null;
            }

            if (TextView != null) {
                TextView.Dispose ();
                TextView = null;
            }
        }
    }
}