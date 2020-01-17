using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Personalization
{
    public class FavoriteColors
    {
        static Dictionary<String, List<uint>> recentColors;

        static Dictionary<String, List<uint>> RecentColors
        {
            get
            {
                if (recentColors == null)
                {
                    LoadColors();
                }
                return recentColors;
            }
        }

        static void SaveColors()
        {
            List<ColorListEntry> cle = new List<ColorListEntry>();
            foreach (var kv in recentColors)
            {
                ColorListEntry cl = new ColorListEntry();
                cl.Name = kv.Key;
                cl.Colors = kv.Value;
                cle.Add(cl);
            }
            XmlListLoader<ColorListEntry>.Save(cle, "FavoriteColors.xml", true);
        }

        static void LoadColors()
        {
            recentColors = new Dictionary<string, List<uint>>();

            var cle = XmlListLoader<ColorListEntry>.Load("FavoriteColors.xml", true);

            if (cle != null)
            {

                foreach (var cl in cle)
                {
                    recentColors[cl.Name] = cl.Colors;
                }
            }

        }

        public static List<uint> GetList(String name, List<uint> defaultColors = null)
        {
            List<uint> colors;

            if (!RecentColors.TryGetValue(name, out colors))
            {
                colors = new List<uint>();
                if (defaultColors != null)
                {
                    colors.AddRange(defaultColors);
                }

                RecentColors[name] = colors;
                SaveColors();
            }

            return colors;
        }

        public static void SetList(String name, List<uint> list)
        {
            RecentColors[name] = list;
            SaveColors();
        }

        public static void Push(String name, uint color, List<uint> defaultColors = null, int ? limit = null)
        {
            List<uint> list = GetList(name, defaultColors);

            int loc = list.IndexOf(color);

            if (loc != -1)
            {
                list.RemoveAt(loc);
            }

            list.PushFront(color);

            if (limit != null)
            {
                list.ShortenToLength(limit.Value);
            }

            SaveColors();
        }

        
        public class ColorListEntry
        {
            public String Name;
            public List<uint> Colors;
        }

    }
}
