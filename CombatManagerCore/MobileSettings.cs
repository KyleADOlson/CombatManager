using System;
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
                return Preferences.Get("RunLocalService", true);
            }
            set
            {
                Preferences.Set("RunLocalService", value);
                Notify("RunLocalService");
            }
        }
        

        public int LocalServicePort
        {
            get
            {
                return Preferences.Get("LocalServicePort", 15247);
            }
            set
            {
                if (value > 0 && value < 32778)
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
                Preferences.Set("LocalServicePasscode", value);
                Notify("LocalServicePasscode");
            }
        }
    }
}
