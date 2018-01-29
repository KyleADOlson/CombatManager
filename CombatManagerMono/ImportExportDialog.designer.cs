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
    [Register ("ImportExportDialog")]
    partial class ImportExportDialog
    {
        [Outlet]
        UIKit.UITableView FileTableView { get; set; }


        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }


        [Outlet]
        CombatManagerMono.GradientView TitleView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView BackgroundView { get; set; }


        [Outlet]
        CombatManagerMono.GradientView ButtonView { get; set; }


        [Outlet]
        UIKit.UIButton BackgroundButton { get; set; }


        [Outlet]
        UIKit.UITextField FileNameText { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OKButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton CancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl ListSelectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        CombatManagerMono.GradientButton SelectAllButton { get; set; }

        [Action ("CancelButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButton_TouchUpInside (CombatManagerMono.GradientButton sender);

        [Action ("ListSelectionViewChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ListSelectionViewChanged (UIKit.UISegmentedControl sender);

        [Action ("OKClicked:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OKClicked (CombatManagerMono.GradientButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BackgroundButton != null) {
                BackgroundButton.Dispose ();
                BackgroundButton = null;
            }

            if (BackgroundView != null) {
                BackgroundView.Dispose ();
                BackgroundView = null;
            }

            if (ButtonView != null) {
                ButtonView.Dispose ();
                ButtonView = null;
            }

            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (FileTableView != null) {
                FileTableView.Dispose ();
                FileTableView = null;
            }

            if (ListSelectionView != null) {
                ListSelectionView.Dispose ();
                ListSelectionView = null;
            }

            if (OKButton != null) {
                OKButton.Dispose ();
                OKButton = null;
            }

            if (SelectAllButton != null) {
                SelectAllButton.Dispose ();
                SelectAllButton = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (TitleView != null) {
                TitleView.Dispose ();
                TitleView = null;
            }
        }
    }
}