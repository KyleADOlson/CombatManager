/*
 *  MainWindowIPad.xib.cs
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
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CombatManager;
using System.ComponentModel;
using System.Drawing;

namespace CombatManagerMono
{
	partial class MainWindowIPad : UIWindow
	{
		public static MainWindowIPad _MainWindow;
		
		
		
		public MainWindowIPad(IntPtr ptr) : base(ptr)
		{
			Initialize();
			
		}
		
		[Export ("init:")]
		public MainWindowIPad () : base ()
		{
			Initialize();
		}
		
		[Export("initWithCoder:")]
		public MainWindowIPad (NSCoder coder) : base (coder)
		{
			Initialize();
		}
		
		public MainWindowIPad (NSObjectFlag t) : base (t)
		{
			Initialize();
		}
		
		[Export ("initWithFrame:")]
		public MainWindowIPad (RectangleF frame) : base (frame)
		{
			Initialize();
		}	
		
		private void Initialize()
		{
			_MainWindow = this;
		}
		
	}
}
	