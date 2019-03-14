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

        private List<List<String>> colors;
        private String name;
        private bool darkScheme;


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
        public bool DarkScheme
        {
            get
            {
                return darkScheme;
            }
            set
            {
                if (darkScheme != value)
                {
                    darkScheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DarkScheme"));
                }
            }
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
        }



        public object Clone()
        {
            return new ColorScheme(this);
        }
    }

}
