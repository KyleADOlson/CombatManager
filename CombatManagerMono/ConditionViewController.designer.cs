/*
 *  ConditionViewController.designer.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace CombatManagerMono
{
	[Register ("ConditionViewController")]
	partial class ConditionViewController
	{
		[Outlet]
		CombatManagerMono.GradientView TopView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BottomView { get; set; }

		[Outlet]
		UIKit.UITableView SelectionTable { get; set; }

		[Outlet]
		UIKit.UITextField FilterText { get; set; }

		[Outlet]
		CombatManagerMono.GradientView TitleView { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		UIKit.UIStepper DurationStepper { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ConditionsTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpellsTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AfflictionsTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CustomTab { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton FavoritesTab { get; set; }

		[Outlet]
		UIKit.UIWebView ConditionDetailWebView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ApplyButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton CloseButton { get; set; }

		[Action ("StepperValueChanged:")]
		partial void StepperValueChanged (UIKit.UIStepper sender);

		[Action ("TabButtonClicked:")]
		partial void TabButtonClicked (UIKit.UIButton sender);

		[Action ("ApplyButtonClicked:")]
		partial void ApplyButtonClicked (UIKit.UIButton sender);

		[Action ("CloseButtonClicked:")]
		partial void CloseButtonClicked (Foundation.NSObject sender);
	}
}
