using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.WinUtil
{
    public static class RegistryHelper
    {
        const string BaseKey = "Software\\CombatManager";

        public static RegistryKey LoadCU(String subkey = null)
        {

            String path = BaseKey + (subkey == null ? "" : ("\\" + subkey));
            return Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
        }


        public static void SaveBool(this RegistryKey key, String name, bool value)
        {

            key.SetValue(name, value ? 1 : 0, RegistryValueKind.DWord);
        }

        public static void SaveDouble(this RegistryKey key, String name, double value)
        {
            key.SetValue(name, value.ToString(), RegistryValueKind.String);
        }

        public static void SaveInt(this RegistryKey key, String name, int value)
        {

            key.SetValue(name, value, RegistryValueKind.DWord);

        }
        public static void SaveString(this RegistryKey key, String name, string value)
        {

            key.SetValue(name, value, RegistryValueKind.String);

        }

        public static bool LoadBool(this RegistryKey key, string name, bool defaultValue)
        {

            bool value = defaultValue;

            try
            {
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

        public static double LoadDouble(this RegistryKey key, String name, double defaultValue)
        {
            double value = defaultValue;
            try
            {
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

        public static int LoadInt(this RegistryKey key, String name, int defaultValue)
        {
            int value = defaultValue;
            try
            {
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


        public static string LoadString(this RegistryKey key, String name)
        {
            return key.LoadString(name, null);
        }

        public static string LoadString(this RegistryKey key, String name, String defaultValue)
        {
            string value = defaultValue;
            try
            {
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


    }
}
