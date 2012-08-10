// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("MonsterEditorStatisticsPage")]
	partial class MonsterEditorStatisticsPage
	{
		[Outlet]
		CombatManagerMono.GradientView BaseView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView ModView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView SkillsView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView DescriptionView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CMBButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton BaseAtkButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CMDButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialModsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AuraButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton LanguagesButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SQButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton GearButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AddSkillButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView AvailableSkillsTable { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView KnownSkillsTable { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SkillDetailButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SkillDetailSelectButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BaseView != null) {
				BaseView.Dispose ();
				BaseView = null;
			}

			if (ModView != null) {
				ModView.Dispose ();
				ModView = null;
			}

			if (SkillsView != null) {
				SkillsView.Dispose ();
				SkillsView = null;
			}

			if (DescriptionView != null) {
				DescriptionView.Dispose ();
				DescriptionView = null;
			}

			if (CMBButton != null) {
				CMBButton.Dispose ();
				CMBButton = null;
			}

			if (BaseAtkButton != null) {
				BaseAtkButton.Dispose ();
				BaseAtkButton = null;
			}

			if (CMDButton != null) {
				CMDButton.Dispose ();
				CMDButton = null;
			}

			if (RacialModsButton != null) {
				RacialModsButton.Dispose ();
				RacialModsButton = null;
			}

			if (AuraButton != null) {
				AuraButton.Dispose ();
				AuraButton = null;
			}

			if (LanguagesButton != null) {
				LanguagesButton.Dispose ();
				LanguagesButton = null;
			}

			if (SQButton != null) {
				SQButton.Dispose ();
				SQButton = null;
			}

			if (GearButton != null) {
				GearButton.Dispose ();
				GearButton = null;
			}

			if (AddSkillButton != null) {
				AddSkillButton.Dispose ();
				AddSkillButton = null;
			}

			if (AvailableSkillsTable != null) {
				AvailableSkillsTable.Dispose ();
				AvailableSkillsTable = null;
			}

			if (KnownSkillsTable != null) {
				KnownSkillsTable.Dispose ();
				KnownSkillsTable = null;
			}

			if (SkillDetailButton != null) {
				SkillDetailButton.Dispose ();
				SkillDetailButton = null;
			}

			if (SkillDetailSelectButton != null) {
				SkillDetailSelectButton.Dispose ();
				SkillDetailSelectButton = null;
			}
		}
	}
}
