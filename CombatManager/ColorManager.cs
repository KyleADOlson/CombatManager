using CombatManager.Personalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CombatManager
{
    static class ColorManager
    {
        public static void PrepareCurrentScheme()
        {
            SetNewScheme(UserSettings.Settings.ColorScheme, UserSettings.Settings.DarkScheme);
        }

        public static void SetNewScheme(int id, bool darkScheme)
        {
            ColorScheme scheme = ColorSchemeManager.Manager.SchemeById(id);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    String change = ColorScheme.GetColorName(i, j);
                    String changeBrush = change + "Brush";
                    object res = App.Current.Resources[change];

                    Color c = scheme.GetColorUInt32(i, j).ToColor();
                    App.Current.Resources[change] = c;
                    App.Current.Resources[changeBrush] = new SolidColorBrush(c);

                }
            }

            Color fore = darkScheme ? 0xFFEEEEEE.ToColor() : Colors.Black;
            Color back = darkScheme ? 0xFF111111.ToColor() : Colors.White;

            App.Current.Resources["ThemeTextForeground"] = fore;
            App.Current.Resources["ThemeTextForegroundBrush"] = new SolidColorBrush(fore);
            App.Current.Resources["ThemeTextBackground"] = back;
            App.Current.Resources["ThemeTextBackgroundBrush"] = new SolidColorBrush(back);

        }
    }
}
