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
using CombatManager;

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

        private const String SettingMainTab = "mainTab";
        private const String SettingCombatTab = "combatTab";

        static int? lastMainTab;
        static int? lastCombatTab;

        public static int GetLastMainTab(Context c)
        {
            if (lastMainTab == null)
            {
                lastMainTab = c.GetCMPrefs().GetInt(SettingMainTab, 0);
            }
            return lastMainTab.Value;
        }

        public static void SetLastMainTab(Context c, int value)
        {
            lastMainTab = value;
            ISharedPreferences sp = c.GetCMPrefs();
            var ed = sp.Edit();
            ed.PutInt(SettingMainTab, value);
            ed.Commit();
        }

        public static int GetLastCombatTab(Context c)
        {
            if (lastCombatTab == null)
            {
                lastCombatTab = c.GetCMPrefs().GetInt(SettingCombatTab, 0);
            }
            return lastCombatTab.Value;
        }

        public static void SetLastCombatTab(Context c, int value)
        {
            lastCombatTab = value;
            ISharedPreferences sp = c.GetCMPrefs();
            var ed = sp.Edit();
            ed.PutInt(SettingCombatTab, value.Clamp(0, 2));
            ed.Commit();
        }

    }
}

