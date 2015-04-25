/*
 *  NumberModifyPopover.xib.designer.cs
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
	[Register ("NumberModifyPopover")]
	partial class NumberModifyPopover
	{
		[Outlet]
		CombatManagerMono.GradientButton subtractButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton setButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton addButton { get; set; }

		[Outlet]
		UIKit.UILabel numberLabel { get; set; }

		[Outlet]
		UIKit.UIPickerView pickerView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NullButton { get; set; }

		[Outlet]
		UIKit.UILabel NumPadLabel { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton1 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton2 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton3 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton4 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton5 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton6 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton7 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton8 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton9 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton NumberButton0 { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ClearButton { get; set; }

		[Outlet]
		UIKit.UIView NumPadView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SwapButton { get; set; }

		[Action ("SwapButtonTouchUpInside:")]
		partial void SwapButtonTouchUpInside (Foundation.NSObject sender);

		[Action ("NumberTouchUpInside:")]
		partial void NumberTouchUpInside (Foundation.NSObject sender);

		[Action ("ClearButtonTouchUpInside:")]
		partial void ClearButtonTouchUpInside (Foundation.NSObject sender);

		[Action ("SubtractClicked:")]
		partial void SubtractClicked (Foundation.NSObject sender);

		[Action ("SetClicked:")]
		partial void SetClicked (Foundation.NSObject sender);

		[Action ("AddClicked:")]
		partial void AddClicked (Foundation.NSObject sender);

		[Action ("NullButtonTouchUpInside:")]
		partial void NullButtonTouchUpInside (Foundation.NSObject sender);
	}
}
