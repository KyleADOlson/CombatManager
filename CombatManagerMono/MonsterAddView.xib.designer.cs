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
    [Register ("MonsterAddView")]
    partial class MonsterAddView
    {
        [Outlet]
        CombatManagerMono.GradientView ButtonAreaView { get; set; }


        [Outlet]
        UIKit.UITableView monsterTable { get; set; }


        [Outlet]
        UIKit.UITextField filterTextBox { get; set; }


        [Outlet]
        UIKit.UIButton addButton { get; set; }


        [Outlet]
        UIKit.UIButton closeButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (addButton != null) {
                addButton.Dispose ();
                addButton = null;
            }

            if (ButtonAreaView != null) {
                ButtonAreaView.Dispose ();
                ButtonAreaView = null;
            }

            if (closeButton != null) {
                closeButton.Dispose ();
                closeButton = null;
            }

            if (filterTextBox != null) {
                filterTextBox.Dispose ();
                filterTextBox = null;
            }

            if (monsterTable != null) {
                monsterTable.Dispose ();
                monsterTable = null;
            }
        }
    }
}