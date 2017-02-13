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
    [Register ("AttackItemView")]
    partial class AttackItemView
    {
        [Outlet]
        CombatManagerMono.GradientView HandView { get; set; }


        [Outlet]
        UIKit.UILabel HandLabel { get; set; }


        [Outlet]
        CombatManagerMono.GradientView NameView { get; set; }


        [Outlet]
        UIKit.UILabel NameLabel { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton DeleteButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton BonusButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton SpecialButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BonusButton != null) {
                BonusButton.Dispose ();
                BonusButton = null;
            }

            if (DeleteButton != null) {
                DeleteButton.Dispose ();
                DeleteButton = null;
            }

            if (HandLabel != null) {
                HandLabel.Dispose ();
                HandLabel = null;
            }

            if (HandView != null) {
                HandView.Dispose ();
                HandView = null;
            }

            if (NameLabel != null) {
                NameLabel.Dispose ();
                NameLabel = null;
            }

            if (NameView != null) {
                NameView.Dispose ();
                NameView = null;
            }

            if (SpecialButton != null) {
                SpecialButton.Dispose ();
                SpecialButton = null;
            }
        }
    }
}