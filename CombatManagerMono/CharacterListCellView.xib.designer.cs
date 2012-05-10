/*
 *  CharacterListCellView.xib.designer.cs
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
using MonoTouch.Foundation;

namespace CombatManagerMono
{
	[Register ("CharacterListCellView")]
	partial class CharacterListCellView
	{
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
		MonoTouch.UIKit.UITableViewCell cellmain { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView nameContainer { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton nameField { get; set; }
	}
}
