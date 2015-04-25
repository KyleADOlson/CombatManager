// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

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
			if (ButtonAreaView != null) {
				ButtonAreaView.Dispose ();
				ButtonAreaView = null;
			}

			if (monsterTable != null) {
				monsterTable.Dispose ();
				monsterTable = null;
			}

			if (filterTextBox != null) {
				filterTextBox.Dispose ();
				filterTextBox = null;
			}

			if (addButton != null) {
				addButton.Dispose ();
				addButton = null;
			}

			if (closeButton != null) {
				closeButton.Dispose ();
				closeButton = null;
			}
		}
	}
}
