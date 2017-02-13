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
    [Register ("ConditionViewController")]
    partial class ConditionViewController
    {
        [Outlet]
        CombatManagerMono.GradientView TopView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView BottomView { get; set; }


        [Outlet]
        UIKit.UITableView SelectionTable { get; set; }


        [Outlet]
        UIKit.UITextField FilterText { get; set; }


        [Outlet]
        CombatManagerMono.GradientView TitleView { get; set; }


        [Outlet]
        UIKit.UILabel DurationLabel { get; set; }


        [Outlet]
        UIKit.UIStepper DurationStepper { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton ConditionsTab { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton SpellsTab { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton AfflictionsTab { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton CustomTab { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton FavoritesTab { get; set; }


        [Outlet]
        UIKit.UIWebView ConditionDetailWebView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton ApplyButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton CloseButton { get; set; }


        [Action ("StepperValueChanged:")]
        partial void StepperValueChanged (UIKit.UIStepper sender);


        [Action ("TabButtonClicked:")]
        partial void TabButtonClicked (UIKit.UIButton sender);


        [Action ("ApplyButtonClicked:")]
        partial void ApplyButtonClicked (UIKit.UIButton sender);


        [Action ("CloseButtonClicked:")]
        partial void CloseButtonClicked (Foundation.NSObject sender);

        [Action ("CloseButtonClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonClicked (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AfflictionsTab != null) {
                AfflictionsTab.Dispose ();
                AfflictionsTab = null;
            }

            if (ApplyButton != null) {
                ApplyButton.Dispose ();
                ApplyButton = null;
            }

            if (BottomView != null) {
                BottomView.Dispose ();
                BottomView = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (ConditionDetailWebView != null) {
                ConditionDetailWebView.Dispose ();
                ConditionDetailWebView = null;
            }

            if (ConditionsTab != null) {
                ConditionsTab.Dispose ();
                ConditionsTab = null;
            }

            if (CustomTab != null) {
                CustomTab.Dispose ();
                CustomTab = null;
            }

            if (DurationLabel != null) {
                DurationLabel.Dispose ();
                DurationLabel = null;
            }

            if (FavoritesTab != null) {
                FavoritesTab.Dispose ();
                FavoritesTab = null;
            }

            if (FilterText != null) {
                FilterText.Dispose ();
                FilterText = null;
            }

            if (SelectionTable != null) {
                SelectionTable.Dispose ();
                SelectionTable = null;
            }

            if (SpellsTab != null) {
                SpellsTab.Dispose ();
                SpellsTab = null;
            }

            if (TitleView != null) {
                TitleView.Dispose ();
                TitleView = null;
            }

            if (TopView != null) {
                TopView.Dispose ();
                TopView = null;
            }
        }
    }
}