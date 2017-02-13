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
    [Register ("MonsterEditorSpecialPage")]
    partial class MonsterEditorSpecialPage
    {
        [Outlet]
        UIKit.UIScrollView SpecialScrollView { get; set; }


        [Outlet]
        CombatManagerMono.GradientButton AddAbilityButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AddAbilityButton != null) {
                AddAbilityButton.Dispose ();
                AddAbilityButton = null;
            }

            if (SpecialScrollView != null) {
                SpecialScrollView.Dispose ();
                SpecialScrollView = null;
            }
        }
    }
}