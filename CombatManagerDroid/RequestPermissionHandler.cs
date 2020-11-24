using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace CombatManagerDroid
{
    public static class RequestPermissionHandler
    {
        static Dictionary<int, Action<string[], Permission[]>> _baseActions;

        static Dictionary<int, Action<string[], Permission[]>> Actions
        {
            get

            {
                if (_baseActions == null)
                {
                    _baseActions = new Dictionary<int, Action<string[], Permission[]>>();
                }
                return _baseActions;
            }

        }

        public static void RegisterAction(int code, Action<string[], Permission[]> action)
        {
            Actions[code] = action;
        }

        public static void Result(int code, string[] permissions, Permission[] results)
        {
            if (Actions.TryGetValue(code, out var act))
            {
                act(permissions, results);
                Actions.Remove(code);
            }

        }

    }
}