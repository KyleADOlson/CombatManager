using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.Personalization
{
    public class ColorScheme : ICloneable, INotifyPropertyChanged
    {
        public static IReadOnlyList<string> Prefixes { get; } = new string[]
                { "PrimaryColor", "SecondaryColorA", "SecondaryColorB" };

        public static IReadOnlyList<string> Shades { get; } = new string[]
                { "Lighter", "Light", "Medium", "Dark", "Darker" };

        public static string GetColorName(int hue, int shade)
        {
            return Prefixes[hue] + Shades[shade];
        }

        public static String GetTextColorBaseName(bool fore)
        {
            return "ThemeText" + (fore ? "Foreground" : "Background");
        }

        public static String GetTextColorName(bool fore, bool dark)
        {
            return GetTextColorBaseName(fore) + ( dark ? "Dark" : "Light");
        }

        private List<List<String>> colors;
        private String name;

        private List<int> textColors;

        private int id;

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public List<List<String>> Colors
        {
            get
            {
                return colors;
            }
            set
            {
                if (colors != value)
                {
                    colors = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Colors"));
                }
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        [DataMember]
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ID"));
                }
            }
        }

        [DataMember]
        public List<int> TextColors
        {
            get

            {
                return textColors;
            }
            set
            {
                if (textColors != value)
                {
                    textColors = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextColors"));
                }
            }
        }

        public UInt32 GetTextColor(bool fore, bool dark)
        {
            if (TextColors == null || TextColors.Count < 4)
            {
                return 0;
            }

            int index = TextColors[(dark ? 2 : 0) + (fore?0:1)];
            if (index == -1)
            {
                return 0;
            }
            else
            {
                return UInt32FromString(ColorFromIndex(index));
            }

        }

        public String ColorFromIndex(int index)
        {
            if (index == -1)
            {
                return null;
            }

            int list = index / 5;
            int level = index % 5;

            return Colors[list][index];

        }


        public ColorScheme()
        {
            /*colors = new List<List<string>>();

            for (int i = 0; i < 3; i++)
            {
                var sub = new List<string>();
                colors.Add(sub);
                for (int j = 0; j < 5; j++)
                {
                    sub.Add("FF000000");
                }
            }*/
        }

        public UInt32 GetColorUInt32(int hue, int shade)
        {
            String color = Colors[hue][shade];
            return UInt32FromString(color);
        }

        private UInt32 UInt32FromString (String color)
        {

            return UInt32.Parse(color, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public void SetColorUInt32(int hue, int shade, UInt32 value)
        {
            Colors[hue][shade] = value.ToString("X8");
        }



        public ColorScheme(ColorScheme old) 
        {
            colors = new List<List<string>>();
            for (int i = 0; i < 3; i++)
            {
                var sub = new List<string>();
                colors.Add(sub);
                for (int j = 0; j < 5; j++)
                {
                    sub.Add(old.Colors[i][j]);
                }
            }
            textColors = new List<int>();
            textColors.AddRange(old.TextColors);
        }



        public object Clone()
        {
            return new ColorScheme(this);
        }
    }

}
