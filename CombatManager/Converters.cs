/*
 *  Converters.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

﻿﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace CombatManager
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    class ActiveToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush) )
            {
                return null;
            }

            return (((bool)value) ? Brushes.Red : Brushes.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    class ItemExistsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool res = (value != null);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(int), typeof(bool))]
    class GreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            if (parameter == null)
            {
                return ((int)value) > 0;
            }
            else
            {
                return ((int)value) > (int)parameter;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(bool), typeof(bool))]
    class BoolInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                return null;
            }

            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                return null;
            }

            return !((bool)value);
        }


    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			
            if (targetType != typeof(Visibility))
            {
                return null;
            }
			
			bool visible = (bool)value;
			if (parameter != null && parameter.GetType() == typeof(bool))
			{
				if ((bool)parameter)
				{
					visible = !visible;
				}
			}
            else if (parameter != null && parameter.GetType() == typeof(string))
            {
                if (String.Compare((string)parameter, "true", true) == 0)
                {
                    visible = !visible;
                }
            }
			
            return ((bool)visible) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                return null;

            }
            Visibility vis = (Visibility)value;
			
			bool invert = false;
			if (parameter != null && parameter.GetType() == typeof(bool))
			{
				invert = (bool)parameter;
			}

            return invert?(vis != Visibility.Visible):(vis==Visibility.Visible);
        }
    }
	
	
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BoolToVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                return null;
            }


            Visibility falseVis = Visibility.Collapsed;
            Visibility trueVis = Visibility.Visible;

            if (parameter != null)
            {

                falseVis = Visibility.Visible;
                trueVis = Visibility.Collapsed;
                    
                
            }

            return ((bool)value) ? trueVis : falseVis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool) || targetType == typeof(bool?))
            {
                return null;

            }
            Visibility vis = (Visibility)value;

            return (vis == Visibility.Visible);
        }
    }

    [ValueConversion(typeof(int), typeof(Visibility))]
    class IntGreaterThanZeroVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                return null;
            }
            return (((int)value) > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(object), typeof(Visibility))]
    class NotNullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
	
	[ValueConversion(typeof(object), typeof(Visibility))]
    class NotNullVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Visibility nullVis = Visibility.Collapsed;
            Visibility notNullVis = Visibility.Visible;

            if (parameter != null)
            {
                nullVis = Visibility.Visible;
                notNullVis = Visibility.Collapsed;
            }


			if (value != null &&  value.GetType() == typeof(String))
			{
				String text = (String)value;

                return (text.Length == 0) ? nullVis : notNullVis;
			}

            return (value != null) ? notNullVis : nullVis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
	
	[ValueConversion(typeof(int), typeof(Brush))]
    class HPToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush) )
            {
                return null;
            }
			
			if ((int)value == 0)
			{
				return Brushes.Blue;
			}

            return (int)value < 0 ? Brushes.Red : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
	
	[ValueConversion(typeof(bool), typeof(Brush))]
    class ChangedToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush) )
            {
                return null;
            }
			

            return (bool)value ?  new SolidColorBrush((Color)parameter) : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(uint?), typeof(Brush))]
    class NullableUintToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
            {
                return null;
            }

            if (value == null)
            {
                return null;
            }
            else
            {
                uint ui = ((uint?)value).Value;
                byte a = (byte)(ui >> 24);
                byte r = (byte)(ui >> 16);
                byte g = (byte)(ui >> 8);
                byte b = (byte)ui;
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    [ValueConversion(typeof(int?), typeof(string))]
    class TurnsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            if (value == null)
            {
                return "-";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int?))
            {
                return DependencyProperty.UnsetValue;
            }

            if (((string)value) == "-")
            {
                return null;
            }

            int outVal;

            if (int.TryParse((string)value, out outVal))
            {
                if (outVal < 0)
                {
                    return DependencyProperty.UnsetValue;
                }

                return outVal;
            }

            return DependencyProperty.UnsetValue;

        }
    }

    [ValueConversion(typeof(int?), typeof(string))]
    class AbilityValueConverter : NullableIntValueConverter
    {
        public AbilityValueConverter()
            : base(0, 999)
        {
        }
    }

    [ValueConversion(typeof(int?), typeof(string))]
    class BonusValueConverter : NullableIntValueConverter
    {
        public BonusValueConverter()
            : base(-999, 999)
        {
        }
    }

    [ValueConversion(typeof(int?), typeof(string))]
    class SpeedValueConverter : NullableIntValueConverter
    {
        public SpeedValueConverter()
            : base(0, 30000)
        {
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    class HPModConverter : IntValueConverter
    {
        public HPModConverter()
            : base(0, 32000)
        {
        }
    }


    [ValueConversion(typeof(int?), typeof(string))]
    class NullableIntValueConverter : IValueConverter
    {
        int minVal;
        int maxVal;

        public NullableIntValueConverter(int minVal, int maxVal)
        {
            this.minVal = minVal;
            this.maxVal = maxVal;
        }


        public NullableIntValueConverter() : this(int.MinValue, int.MaxValue)
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            if (value == null)
            {
                return "-";
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int?))
            {
                return DependencyProperty.UnsetValue;
            }

            if (((string)value) == "-")
            {
                return null;
            }

            int outVal;

            if (int.TryParse((string)value, out outVal))
            {
                if (outVal < minVal || outVal > maxVal)
                {
                    return DependencyProperty.UnsetValue;
                }

                return outVal;
            }

            return DependencyProperty.UnsetValue;

        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    class IntValueConverter : IValueConverter
    {
        int minVal;
        int maxVal;

        public IntValueConverter(int minVal, int maxVal)
        {
            this.minVal = minVal;
            this.maxVal = maxVal;
        }


        public IntValueConverter()
            : this(int.MinValue, int.MaxValue)
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
            {
                return DependencyProperty.UnsetValue;
            }

            int outVal;

            if (int.TryParse((string)value, out outVal))
            {
                if (outVal < minVal || outVal > maxVal)
                {
                    return DependencyProperty.UnsetValue;
                }

                return outVal;
            }

            return DependencyProperty.UnsetValue;

        }
    }

    [ValueConversion(typeof(int?), typeof(string))]
    class AbilityBonusConverter : IValueConverter
    {



        public AbilityBonusConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            if (value == null)
            {
                return "0";
            }

            else
            {
                return CMStringUtilities.PlusFormatNumber(Monster.AbilityBonus((int)value));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return DependencyProperty.UnsetValue;

        }
    }

	
    [ValueConversion(typeof(string), typeof(int))]
    class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
            {
                return DependencyProperty.UnsetValue;
            }


            return (int)SizeMods.GetSize((string)value);
			
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            return SizeMods.GetSizeText((MonsterSize)value);
			
        }
    }
	
	[ValueConversion(typeof(string), typeof(int))]
    class AlignmentIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
            {
                return DependencyProperty.UnsetValue;
            }

            Monster.AlignmentType al = Monster.ParseAlignment((string)value);

            int val = 0;

            val += (int)al.Order;
            val += 3 * (int)al.Moral;

            return val;
            
			
			
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            Monster.AlignmentType type = new Monster.AlignmentType();
            type.Moral = (Monster.MoralAxis)((int)value / 3);
            type.Order = (Monster.OrderAxis)((int)value % 3);

            return Monster.AlignmentText(type) ;
			
        }
    }

    [ValueConversion(typeof(string), typeof(int))]
    class MonsterTypeIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
            {
                return DependencyProperty.UnsetValue;
            }

            return (int)Monster.ParseCreatureType((string)value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            return Monster.CreatureTypeText((CreatureType)value);

        }
    }

    [ValueConversion(typeof(Attack), typeof(ImageSource))]
    class AttackToTypeImageConverter : IValueConverter
    {
        static BitmapImage natural;
        static BitmapImage melee;
        static BitmapImage ranged;

        static AttackToTypeImageConverter()
        {
            try
            {
                natural = new BitmapImage(new Uri("pack://application:,,,/Images/claw-16.png"));
                melee = new BitmapImage(new Uri("pack://application:,,,/Images/sword-single-16.png"));
                ranged = new BitmapImage(new Uri("pack://application:,,,/Images/bow-16.png"));
            }
            catch (Exception)
            {
                //this is to prevent problems in the editor
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
            {
                return null;
            }

            Attack attack = (Attack)value;

            if (attack.Weapon != null)
            {
                if (attack.Weapon.Class == "Natural")
                {
                    return natural; 
                }
                else if (attack.Weapon.Ranged)
                {
                    return ranged;
                }
                else
                {
                    return melee;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(String), typeof(ImageSource))]
    class StringImageSmallIconConverter : IValueConverter
    {

        public static Dictionary<string, BitmapImage> loadedImages;

        static StringImageSmallIconConverter()
        {
            loadedImages = new Dictionary<string, BitmapImage>();
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string name = (string)value;

            return FromName(name);
        }

        public static BitmapImage FromName(string name)
        {
            BitmapImage image = null;

            if (name != null)
            {

                if (loadedImages.ContainsKey(name))
                {
                    image = loadedImages[name];
                }

                else
                {
                    try
                    {
                        image = new BitmapImage(new Uri("pack://application:,,,/Images/" + name + "-16.png"));
                        if (image != null)
                        {
                            loadedImages[name] = image;
                        }
                    }
                    catch (IOException)
                    {
                    }
                }

            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(String), typeof(Image))]
    class StringImageConverter : IValueConverter
    {
        public static Dictionary<string, ImageSource> loadedImages;

        static StringImageConverter()
        {
            loadedImages = new Dictionary<string, ImageSource>();
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string name = (string)value;

            ImageSource image = null;

            if (name != null)
            {

                if (loadedImages.ContainsKey(name))
                {
                    image = loadedImages[name];
                }

                else
                {
                    image = new BitmapImage(new Uri("pack://application:,,,/Images/" + name + "-16.png"));
                    if (image != null)
                    {
                        loadedImages[name] = image;
                    }
                }

            }
            Image imageControl = new Image();
            imageControl.Source = image;
            return imageControl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    class StringCapitalizeConverter : IValueConverter
    {

        public static SortedSet<String> ignoreWords;

        static StringCapitalizeConverter()
        {
            ignoreWords = new SortedSet<string>(new InsensitiveComparer());
            
            string[] ignore = { "the", "of", "from", "to", "and" };

            foreach (string str in ignore)
            {
                ignoreWords.Add(str);
            }
        }



        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }
						

            return Capitalize ((string)value);
			


        }

        public static string Capitalize(string text)
        {
            if (text != null)
            {

                Regex regWord = new Regex("\\w+('s)?");

                text = regWord.Replace(text, delegate(Match m)
                {
                    string x = m.Value;

                    if (!ignoreWords.Contains(x))
                    {

                        x = x.Substring(0, 1).ToUpper() + x.Substring(1);
                    }

                    return x;
                });
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            return ((string)value).ToLower();

        }
    }

    [ValueConversion(typeof(WeaponItem), typeof(string))]
    class WeaponItemToHandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            WeaponItem item = (WeaponItem)value;

			if (item == null)
			{
				return DependencyProperty.UnsetValue;
			}
			
            if (item.MainHand)
            {

                return "Main";
            }
            else
            {

                return "Off";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
           
        }
    }

    class SensesConverter : IMultiValueConverter
    {
        private static int _Perception;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            if (values.Length != 2 || !(values[0] is string) || !(values[1] is int))
            {
                return DependencyProperty.UnsetValue;
            }

            string senses = (string)values[0];
            _Perception = (int)values[1];

            Regex regEx = new Regex("(?<senses>.*?)(; )?Perception (\\+|\\-)[0-9]+");

            Match m = regEx.Match(senses);

            if (m.Success)
            {
                return m.Groups["senses"].Value;
            }
            else
            {
                return senses;

            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {

            object[] objects = new object[targetTypes.Length];

            string senses = ((string)value).Trim();

            string sensesText = "";

            if (senses.Length > 0)
            {
                sensesText += senses + "; ";
            }

            objects[0] = sensesText + "Perception " + CMStringUtilities.PlusFormatNumber(_Perception);
            objects[1] = _Perception;
            

            return objects;
        }


    }

    [ValueConversion(typeof(string), typeof(string))]
    class CRValidatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            string cr = (string)value;

            long? xpVal = Monster.TryGetCRValue(cr);

            if (xpVal == null)
            {
                return DependencyProperty.UnsetValue;
            }
            

            return cr.Trim();

        }
    }

    [ValueConversion(typeof(string), typeof(int))]
    class FtPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string strVal = (string)value;


            int retVal = 0;

            if (strVal != null)
            {

                Regex regFt = new Regex("(?<num>[0-9]+) +ft\\.");
                Match m = regFt.Match(strVal);


                if (m.Success)
                {
                    retVal = int.Parse(m.Groups["num"].Value);
                }
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (int)value + " ft.";
            }
            else
            {
                return ((string)value) + " ft.";
            }
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    class ParenthesesPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string strVal = (string)value;


            if (strVal != null)
            {

                strVal = strVal.Trim(new char[] {'(', ')', ' '});
            }

            return strVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strVal = (string)value;

            if (strVal != null && strVal.Trim().Length > 0)
            {
                strVal = "(" + strVal + ")";
            }
            else
            {
                strVal = null;
            }

            return strVal;
        }
    }

    [ValueConversion(typeof(string), typeof(int))]
    class SpellSchoolIndexConverter : IValueConverter
    {
        private static List<string> _Schools;

        static SpellSchoolIndexConverter()
        {
            try
            {
                _Schools = new List<string>();
                foreach (Rule r in Rule.Rules.FindAll(a => a.Type == "Magic" && a.Subtype == "School"))
                {
                    _Schools.Add(r.Name.ToLower());
                }
                _Schools.Sort();
            }
            catch (Exception)
            {
                //should only happen at design time
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(int))
            {
                return DependencyProperty.UnsetValue;
            }

            if (value == null)
            {
                return 0;
            }


            return _Schools.IndexOf(((string)value).ToLower());

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            int index = (int)value;

            if (index < _Schools.Count)
            {
                return _Schools[(int)value];
            }

            return null;

        }

        public static IEnumerable<string> Schools
        {
            get
            {
                return _Schools;
            }
        }
    }

    [ValueConversion(typeof(Key), typeof(String))]
    class KeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String))
            {
                return null;
            }

            Key k = (Key)value;

            if (k >= Key.A && k <= Key.Z)
            {
                int diff = k - Key.A;

                Char c = (Char)((int)'A' + diff);
                String s = c.ToString();
                return s;
            }
            else if (k >= Key.NumPad0 && k <= Key.NumPad9)
            {
                int diff = k - Key.NumPad0;               
                return diff.ToString();
            }
            else if (k >= Key.F1 && k <= Key.F10)
            {
                int val = k - Key.F1 + 1;

                return "F" + val;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Key))
            {
                return null;

            }

            String s = (String)value;
            Match m = Regex.Match(s, "F(?<num>[0-9]+)");
            if (m.Success)
            {
                int key = int.Parse(m.Groups["num"].Value) - 1;
                
                return (Key)(Key.F1 + key);
            }
            if (Regex.Match(s, "[0-9]").Success)
            {
                Char c = s[0];
                int diff = c - '0';
                return (Key)(Key.NumPad0 + diff);
            }
            if (Regex.Match(s, "[A-Z]").Success)
            {
                Char c = s[0];
                int diff = c - 'A';
                return (Key)(Key.A + diff);
            }

            return null;
        }
    }
	
}
