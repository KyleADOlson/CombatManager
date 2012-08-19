// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("MonsterEditorFeatsPage")]
	partial class MonsterEditorFeatsPage
	{
		[Outlet]
		MonoTouch.UIKit.UITableView CurrentFeatsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView AvailableFeatsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField FilterTextView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CurrentFeatsView != null) {
				CurrentFeatsView.Dispose ();
				CurrentFeatsView = null;
			}

			if (AvailableFeatsView != null) {
				AvailableFeatsView.Dispose ();
				AvailableFeatsView = null;
			}

			if (FilterTextView != null) {
				FilterTextView.Dispose ();
				FilterTextView = null;
			}
		}
	}
}
