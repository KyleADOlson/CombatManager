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
            SetNewScheme(UserSettings.Settings.ColorScheme);
        }

        public static void SetNewScheme(int index)
        {
            ColorScheme scheme = ColorSchemeManager.Manager.ColorSchemes[index];

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
        }
    }
}
