
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CombatManagerMono
{
	public class Application
	{
		static void Main (string[] args)
		{
			CombatManager.DBSettings.UseDB = false;
			UIApplication.Main (args, null, "AppDelegateIPad");
		}
	}
}

