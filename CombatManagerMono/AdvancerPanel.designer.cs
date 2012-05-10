/*
 *  AdvancerPanel.designer.cs
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
	[Register ("AdvancerPanel")]
	partial class AdvancerPanel
	{
		[Outlet]
		CombatManagerMono.GradientButton AddMonsterButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView RacialView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView SimpleView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView OtherView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView SimpleHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView RacialHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientView OtherHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AdvancedButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton SimpleSizeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OutsiderButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton AugmentSummoningButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialHitDieButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialBonusButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton RacialSizeButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton OtherTemplateButton { get; set; }

		[Outlet]
		CombatManagerMono.GradientView AdvancerHeaderView { get; set; }

		[Outlet]
		CombatManagerMono.GradientButton ResetButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView OtherTemplateOptionView { get; set; }

		[Action ("AddMonsterButtonTouchUpInside:")]
		partial void AddMonsterButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("AdvancerHeaderButtonTouchUpInside:")]
		partial void AdvancerHeaderButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("ResetButtonTouchUpInside:")]
		partial void ResetButtonTouchUpInside (MonoTouch.Foundation.NSObject sender);
	}
}
