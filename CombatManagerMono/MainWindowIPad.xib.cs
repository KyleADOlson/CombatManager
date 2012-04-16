
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
	