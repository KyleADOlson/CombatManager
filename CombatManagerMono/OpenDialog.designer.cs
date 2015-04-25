// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("OpenDialog")]
	partial class OpenDialog
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
		
		void ReleaseDesignerOutlets ()
		{
			if (FileTableView != null) {
				FileTableView.Dispose ();
				FileTableView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleView != null) {
				TitleView.Dispose ();
				TitleView = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (ButtonView != null) {
				ButtonView.Dispose ();
				ButtonView = null;
			}

			if (BackgroundButton != null) {
				BackgroundButton.Dispose ();
				BackgroundButton = null;
			}

			if (FileNameText != null) {
				FileNameText.Dispose ();
				FileNameText = null;
			}

			if (OKButton != null) {
				OKButton.Dispose ();
				OKButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}
		}
	}
}
