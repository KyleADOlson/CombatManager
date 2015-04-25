// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CombatManagerMono
{
	[Register ("SpellLevelsDialog")]
	partial class SpellLevelsDialog
	{
		[Outlet]
		CombatManagerMono.GradientButton AddButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ClassButton { get; set; }

		[Outlet]
		UIKit.UITableView LevelsTable { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Action ("AddButtonClicked:")]
		partial void AddButtonClicked (Foundation.NSObject sender);

		[Action ("CancelButtonClicked:")]
		partial void CancelButtonClicked (Foundation.NSObject sender);

		[Action ("OkButtonClicked:")]
		partial void OkButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (ClassButton != null) {
				ClassButton.Dispose ();
				ClassButton = null;
			}

			if (LevelsTable != null) {
				LevelsTable.Dispose ();
				LevelsTable = null;
			}

			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}
		}
	}
}
