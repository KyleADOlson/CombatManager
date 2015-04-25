// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

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
			if (IndicatorView != null) {
				IndicatorView.Dispose ();
				IndicatorView = null;
			}

			if (hpButton != null) {
				hpButton.Dispose ();
				hpButton = null;
			}

			if (maxHPButton != null) {
				maxHPButton.Dispose ();
				maxHPButton = null;
			}

			if (nonlethalButton != null) {
				nonlethalButton.Dispose ();
				nonlethalButton = null;
			}

			if (actionsButton != null) {
				actionsButton.Dispose ();
				actionsButton = null;
			}

			if (modButton != null) {
				modButton.Dispose ();
				modButton = null;
			}

			if (tempHPButton != null) {
				tempHPButton.Dispose ();
				tempHPButton = null;
			}

			if (cellmain != null) {
				cellmain.Dispose ();
				cellmain = null;
			}

			if (nameContainer != null) {
				nameContainer.Dispose ();
				nameContainer = null;
			}

			if (nameField != null) {
				nameField.Dispose ();
				nameField = null;
			}
		}
	}
}
