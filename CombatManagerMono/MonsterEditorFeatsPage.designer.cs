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
    [Register ("MonsterEditorFeatsPage")]
    partial class MonsterEditorFeatsPage
    {
        [Outlet]
        UIKit.UITableView CurrentFeatsView { get; set; }


        [Outlet]
        UIKit.UITableView AvailableFeatsView { get; set; }


        [Outlet]
        UIKit.UITextField FilterTextView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AvailableFeatsView != null) {
                AvailableFeatsView.Dispose ();
                AvailableFeatsView = null;
            }

            if (CurrentFeatsView != null) {
                CurrentFeatsView.Dispose ();
                CurrentFeatsView = null;
            }

            if (FilterTextView != null) {
                FilterTextView.Dispose ();
                FilterTextView = null;
            }
        }
    }
}