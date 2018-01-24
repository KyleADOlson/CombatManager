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
    [Register ("MonsterEditorDialog")]
    partial class MonsterEditorDialog
    {
        [Outlet]
        CombatManagerMono.GradientButton CancelButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OKButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientView BackgroundView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView HeaderView { get; set; }


        [Outlet]
        UIKit.UILabel HeaderLabel { get; set; }


        [Outlet]
        UIKit.UIView PageView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView PageBorderView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton BasicButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton DefenseButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OffenseButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton StatisticsButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton FeatsButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton SpecialButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton DescriptionButton { get; set; }


        [Action ("TabButtonTouchUpInside:")]
        partial void TabButtonTouchUpInside (Foundation.NSObject sender);


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

            if (BasicButton != null) {
                BasicButton.Dispose ();
                BasicButton = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (DefenseButton != null) {
                DefenseButton.Dispose ();
                DefenseButton = null;
            }

            if (DescriptionButton != null) {
                DescriptionButton.Dispose ();
                DescriptionButton = null;
            }

            if (FeatsButton != null) {
                FeatsButton.Dispose ();
                FeatsButton = null;
            }

            if (HeaderLabel != null) {
                HeaderLabel.Dispose ();
                HeaderLabel = null;
            }

            if (HeaderView != null) {
                HeaderView.Dispose ();
                HeaderView = null;
            }

            if (OffenseButton != null) {
                OffenseButton.Dispose ();
                OffenseButton = null;
            }

            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }

            if (PageBorderView != null) {
                PageBorderView.Dispose ();
                PageBorderView = null;
            }

            if (PageView != null) {
                PageView.Dispose ();
                PageView = null;
            }

            if (SpecialButton != null) {
                SpecialButton.Dispose ();
                SpecialButton = null;
            }

            if (StatisticsButton != null) {
                StatisticsButton.Dispose ();
                StatisticsButton = null;
            }
        }
    }
}