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

namespace CombatManagerDroid
{        
    public static class CMPreferences 
    {
        public static string FileName
        {
            get
            {
                return "CombatManager";
            }
        }

        public static string SettingConfirmInitiative
        {
            get
            {
                return "confirmInitiative";
            }
        }

        public static string SettingRoll3d6
        {
            get
            {
                return "use3d6";
            }
        }
        public static string SettingRollHP
        {
            get
            {
                return "rollHP";
            }
        }
        public static string SettingShowDieRoller
        {
            get
            {
                return "showDieRoller";
            }
        }

        public static ISharedPreferences GetCMPrefs(this Context c)
        {
            return c.GetSharedPreferences(FileName, 0);
        }

        public static bool GetConfirmInitiative(this ISharedPreferences p)
        {
            return p.GetBoolean(SettingConfirmInitiative, true);
        }

        public static bool GetRollHP(this ISharedPreferences p)
        {
            return p.GetBoolean(SettingRollHP, true);
        }


        public static bool GetShowDieRoller(this ISharedPreferences p)
        {
            return p.GetBoolean(SettingShowDieRoller, true);
        }
    }
}

