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
    [Register ("CharacterListCellView")]
    partial class CharacterListCellView
    {
        [Outlet]
        UIKit.UIImageView IndicatorView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton hpButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton maxHPButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton nonlethalButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton actionsButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton modButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton tempHPButton { get; set; }


        [Outlet]
        UIKit.UITableViewCell cellmain { get; set; }


        [Outlet]
        UIKit.UIView nameContainer { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton nameField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (actionsButton != null) {
                actionsButton.Dispose ();
                actionsButton = null;
            }

            if (cellmain != null) {
                cellmain.Dispose ();
                cellmain = null;
            }

            if (hpButton != null) {
                hpButton.Dispose ();
                hpButton = null;
            }

            if (IndicatorView != null) {
                IndicatorView.Dispose ();
                IndicatorView = null;
            }

            if (maxHPButton != null) {
                maxHPButton.Dispose ();
                maxHPButton = null;
            }

            if (modButton != null) {
                modButton.Dispose ();
                modButton = null;
            }

            if (nameContainer != null) {
                nameContainer.Dispose ();
                nameContainer = null;
            }

            if (nameField != null) {
                nameField.Dispose ();
                nameField = null;
            }

            if (nonlethalButton != null) {
                nonlethalButton.Dispose ();
                nonlethalButton = null;
            }

            if (tempHPButton != null) {
                tempHPButton.Dispose ();
                tempHPButton = null;
            }
        }
    }
}