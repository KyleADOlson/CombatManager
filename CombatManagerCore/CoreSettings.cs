using System;
using CombatManager.LocalService;
#if MONO
using Xamarin.Essentials;
#else
using Microsoft.Win32;
using System.IO;
#endif

namespace CombatManager
{

    //these settings are set in a platform appropriate manner in the core
    //the settings are automatically stored when changed

    public class CoreSettings : SimpleNotifyClass
    {
        private static CoreSettings instance;

        private  CoreSettings()
        {
        }

        public static CoreSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CoreSettings();
                }
                return instance;
            }
        }

        public bool RunLocalService
        {
            get
            {
                return LoadBoolValue("RunLocalService", false);
            }
            set
            {
                if (RunLocalService != value)
                {
                    SaveBoolValue("RunLocalService", value);
                    Notify("RunLocalService");
                }
            }
        }
        

        public int LocalServicePort
        {
            get
            {
                return LoadIntValue("LocalServicePort", LocalCombatManagerService.DefaultPort);
            }
            set
            {
                if (value > 0 && value < 32778 &&LocalServicePort != value)
                {

                    SaveIntValue("LocalServicePort", value);
                    Notify("LocalServicePort");
                }
            }
        }

        public string LocalServicePasscode
        {

            get
            {
                return LoadStringValue("LocalServicePasscode", "");
            }
            set
            {
                if (LocalServicePasscode != value)
                {
                    SaveStringValue("LocalServicePasscode", value);
                    Notify("LocalServicePasscode");
                }
            }
        }

        public bool AutomaticStabilization
        {
            get
            {
                return LoadBoolValue("AutomaticStabilization", false);
            }
            set
            {
                if (AutomaticStabilization != value)
                {
                    SaveBoolValue("AutomaticStabilization", value);
                    Notify("AutomaticStabilization");
                }
            }
        }


#if MONO
        public static void SaveBoolValue(String name, bool value)
        {
            Preferences.Set(name, value);
        }
    

        public static bool LoadBoolValue(String name, bool def)
        {
            return Preferences.Get(name, def);

        }
        public static void SaveStringValue(String name, string value)
        {
            Preferences.Set(name, value);
        }
    

        public static String LoadStringValue(String name, string def)
        {
            return Preferences.Get(name, (string)def);
        }
        public static void SaveIntValue(String name, int value)
        {
            Preferences.Set(name, value);
        }
    

        public static int LoadIntValue(String name, int def)
        {
            return Preferences.Get(name, def);
        }
#endif

#if !MONO

        public static bool LoadBoolValue(string name, bool defaultValue)
        {

            bool value = defaultValue;

            try
            {
                RegistryKey key = RegKey;
                if (key != null)
                {
                    RegistryValueKind ki = key.GetValueKind(name);

                    if (ki == RegistryValueKind.DWord)
                    {
                        int val = (int)key.GetValue(name);

                        value = (val != 0);
                    }
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return value;


        }

        public static double LoadDoubleValue(String name, double defaultValue)
        {
            double value = defaultValue;
            try
            {
                RegistryKey key = RegKey;
                if (key != null)
                {
                    RegistryValueKind ki = key.GetValueKind(name);

                    if (ki == RegistryValueKind.String)
                    {
                        String val = (String)key.GetValue(name);

                        if (val != null)
                        {
                            double.TryParse(val, out value);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return value;

        }

        public static int LoadIntValue(String name, int defaultValue)
        {
            int value = defaultValue;
            try
            {

                RegistryKey key = RegKey;
                if (key != null)
                {
                    RegistryValueKind ki = key.GetValueKind(name);

                    if (ki == RegistryValueKind.DWord)
                    {
                        value = (int)key.GetValue(name);

                    }
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return value;

        }
        public static string LoadStringValue(String name, String defaultValue)
        {
            string value = defaultValue;
            try
            {
                RegistryKey key = RegKey;
                if (key != null)
                {
                    RegistryValueKind ki = key.GetValueKind(name);

                    if (ki == RegistryValueKind.String)
                    {
                        value = (String)key.GetValue(name);

                    }
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return value;

        }
        public static RegistryKey RegKey
        {
            get
            {
                return Registry.CurrentUser.OpenSubKey("Software\\CombatManager", true);
            }
        }

        public static void SaveBoolValue(String name, bool value)
        {

            RegKey.SetValue(name, value ? 1 : 0, RegistryValueKind.DWord);
        }

        public static void SaveDoubleValue(String name, double value)
        {
            RegKey.SetValue(name, value.ToString(), RegistryValueKind.String);
        }

        public static void SaveIntValue(String name, int value)
        {

            RegKey.SetValue(name, value, RegistryValueKind.DWord);

        }
        public static void SaveStringValue(String name, string value)
        {

            RegKey.SetValue(name, value, RegistryValueKind.String);

        }
#endif
    }
}
