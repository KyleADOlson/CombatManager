using System;
using CombatManager.LocalService;
using Xamarin.Essentials;

namespace CombatManager
{
    public class MobileSettings : SimpleNotifyClass
    {
        private static MobileSettings instance;

        private  MobileSettings()
        {
        }

        public static MobileSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MobileSettings();
                }
                return instance;
            }
        }

        public bool RunLocalService
        {
            get
            {
                return Preferences.Get("RunLocalService", false);
            }
            set
            {
                if (RunLocalService != value)
                {
                    Preferences.Set("RunLocalService", value);
                    Notify("RunLocalService");
                }
            }
        }
        

        public int LocalServicePort
        {
            get
            {
                return Preferences.Get("LocalServicePort", LocalCombatManagerService.DefaultPort);
            }
            set
            {
                if (value > 0 && value < 32778 &&LocalServicePort != value)
                {

                    Preferences.Set("LocalServicePort", value);
                    Notify("LocalServicePort");
                }
            }
        }

        public string LocalServicePasscode
        {

            get
            {
                return Preferences.Get("LocalServicePasscode", "");
            }
            set
            {
                if (LocalServicePasscode != value)
                {
                    Preferences.Set("LocalServicePasscode", value);
                    Notify("LocalServicePasscode");
                }
            }
        }
    }
}
