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
    [Register ("MonsterEditorDescriptionPage")]
    partial class MonsterEditorDescriptionPage
    {
        [Outlet]
        CombatManagerMono.GradientButton EnvironmentButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton OrganizationButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton TreasureButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton BeforeCombatButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton DuringCombatButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton MoraleButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton VisualDescriptionButton { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton DescriptionButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BeforeCombatButton != null) {
                BeforeCombatButton.Dispose ();
                BeforeCombatButton = null;
            }

            if (DescriptionButton != null) {
                DescriptionButton.Dispose ();
                DescriptionButton = null;
            }

            if (DuringCombatButton != null) {
                DuringCombatButton.Dispose ();
                DuringCombatButton = null;
            }

            if (EnvironmentButton != null) {
                EnvironmentButton.Dispose ();
                EnvironmentButton = null;
            }

            if (MoraleButton != null) {
                MoraleButton.Dispose ();
                MoraleButton = null;
            }

            if (OrganizationButton != null) {
                OrganizationButton.Dispose ();
                OrganizationButton = null;
            }

            if (TreasureButton != null) {
                TreasureButton.Dispose ();
                TreasureButton = null;
            }

            if (VisualDescriptionButton != null) {
                VisualDescriptionButton.Dispose ();
                VisualDescriptionButton = null;
            }
        }
    }
}