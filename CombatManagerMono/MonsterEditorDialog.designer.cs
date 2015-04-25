/*
 *  MonsterEditorDialog.designer.cs
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
	[Register ("MonsterEditorDialog")]
	partial class MonsterEditorDialog
	{
		[Outlet]
		CombatManagerMono.GradientButton CancelButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OKButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView BackgroundView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView HeaderView { get; set; }

		[Outlet]
		UIKit.UILabel HeaderLabel { get; set; }

		[Outlet]
		UIKit.UIView PageView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView PageBorderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton BasicButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DefenseButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OffenseButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton StatisticsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton FeatsButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SpecialButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton DescriptionButton { get; set; }

		[Action ("TabButtonTouchUpInside:")]
		partial void TabButtonTouchUpInside (Foundation.NSObject sender);

		[Action ("OKButtonTouchUpInside:")]
		partial void OKButtonTouchUpInside (Foundation.NSObject sender);

		[Action ("CancelButtonTouchUpInside:")]
		partial void CancelButtonTouchUpInside (Foundation.NSObject sender);
	}
}
