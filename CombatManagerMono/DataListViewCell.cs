/*
 *  DataListViewCell.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CombatManager;
using System.ComponentModel;
using CoreGraphics;

namespace CombatManagerMono
{
	public class DataListViewCell : UITableViewCell
	{
		public object Data {get; set;}
		
		public DataListViewCell (UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier)
		{
		}


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (AccessoryView != null)
            {
                CGRect accessoryViewFrame = AccessoryView.Frame;
                accessoryViewFrame.X = Frame.Width - accessoryViewFrame.Width - 1;
                AccessoryView.Frame = accessoryViewFrame;
            }
        }
	}
}

