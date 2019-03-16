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

        Dictionary<int, ColorScheme> schemeDictionary;

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

            schemeDictionary = new Dictionary<int, ColorScheme>();
            foreach (var v in colorSchemes)
            {
                schemeDictionary[v.ID] = v;
            }

        }

        public ColorScheme SchemeById(int id)
        {
            ColorScheme scheme;
            if (!schemeDictionary.TryGetValue(id, out scheme))
            {
                return schemeDictionary[0];
            }
            return scheme;
        }

        public List<ColorScheme> SortedSchemes
        {
            get
            {
                List<ColorScheme> list = new List<ColorScheme>();
                list.Add(schemeDictionary[0]);
                list.AddRange(from v in schemeDictionary.Values where v.ID != 0 orderby v.Name select v);
                return list;
                    
            }
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
