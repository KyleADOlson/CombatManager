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
    [Register ("NaturalAttackItemView")]
    partial class NaturalAttackItemView
    {
        [Outlet]
        UIKit.UIButton DeleteButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton TitleButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientView BackgroundView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (DeleteButton != null) {
                DeleteButton.Dispose ();
                DeleteButton = null;
            }

            if (TitleButton != null) {
                TitleButton.Dispose ();
                TitleButton = null;
            }
        }
    }
}