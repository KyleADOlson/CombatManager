using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Personalization
{
    public class ColorSchemeManager
    {
        List<ColorScheme> colorSchemes;
        List<ColorScheme> defaultSchemes;

        private static ColorSchemeManager manager;
        public static ColorSchemeManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = new ColorSchemeManager();
                }
                return manager;
            }
        }

        private ColorSchemeManager()
        {
            defaultSchemes = XmlListLoader<ColorScheme>.Load("DefaultColorSchemes.xml");
            colorSchemes = new List<ColorScheme>();
            colorSchemes.AddRange(defaultSchemes);
            XmlListLoader<ColorScheme>.Save(defaultSchemes, "Temp.xml");

        }

        public List<ColorScheme> ColorSchemes
        {
            get
            {
                return colorSchemes;
            }
        }

    }
}
