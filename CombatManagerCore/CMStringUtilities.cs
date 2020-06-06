/*
 *  CMStringUtilities.cs
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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public static class CMStringUtilities
    {

        public static bool CommaMatch(string name1, String name2)
        {
            return String.Compare(DecommaText(name1), DecommaText(name2), true) == 0;
        }

        public static String DecommaText(string text)
        {
            bool changed;
            return DecommaText(text, out changed);
        }

        public static String DecommaText(string text, out bool changed)
        {


            int loc = text.IndexOf(',');

            if (loc == -1)
            {
                changed = false;
                return text;
            }

            else
            {
                changed = true;
                return text.Substring(loc + 1).Trim() + " " + text.Substring(0, loc);
            }
        }

        public static String Decomma(this String text)
        {
            return DecommaText(text);
        }

        public static String Capitalize(this String text)
        {
            return StringCapitalizer.Capitalize(text);
        }

        public static List<String> Tokenize(this String text, char token)
        {
            if (text == null)
            {
                return new List<String>();
            }
            List<String> strings = new List<String>();
            foreach (string str in ((String)text).Split(new char[]{','}))
            {
                strings.Add(str.Trim());
            }
            return strings;
        }

        public static string ToTokenString(this IEnumerable<string> strings, char token)
        {
            if (strings == null || !strings.Any())

            {
                return null;
            }
            string list = "";
            foreach (string str in strings)
            {
                if (list != "")
                {
                    list += token + " ";
                }
                list += str;
            }
            return list;
        }

        public static List<string> ToStringList<T>(this IEnumerable<T> obs)
        {
            List<string> strings = new List<string>();
            foreach (var o in obs)
            {
                strings.Add(o.ToString());
            }
            return strings;
        }

        public static List<string> AddIfNotNull(this List<string> list, object ob, string prefix = null, string  text = null, string postfix = null)
        {
            if (ob != null)
            {
                
                list.Add((prefix ?? "") + (text ?? ob.ToString()) + (postfix ?? ""));
            }
            return list;
        }
        
        public static string WeaveObjectsToString<T>(this IEnumerable<T> ob, string space)
        {
            StringBuilder b = new StringBuilder();
            ob.WeaveList(s => b.Append(s.ToString()), (x, y) => b.Append(space));
            return b.ToString();
        }

        public static string WeaveString(this IEnumerable<string> strings, string space)
        {
            StringBuilder b = new StringBuilder();
            strings.WeaveList(s => b.Append(s), (x, y) => b.Append(space));
            return b.ToString();
        }


        public static string PlusFormatSpaceNumber(int number)
        {
            if (number >= 0)
            {
                return "+ " + number.ToString();
            }
            else
            {
                return "- " + Math.Abs(number).ToString();
            }
        }

        public static string PlusFormatNumber(int number)
        {
            if (number >= 0)
            {
                return "+" + number.ToString();
            }
            else
            {
                return number.ToString();
            }
        }

        public static string PlusFormatNumber(int? number)
        {
            if (number == null)
            {
                return "-";
            }
            else return ((int)number).PlusFormat();
        }


        public static string PlusFormat(this int number)
        {
            return PlusFormatNumber(number);
        }

        public static string PlusFormat(this int? number)
        {
            return PlusFormatNumber(number);
        }

        public static string AppendListItem(this string str, string text)
        {
            string res = str;

            if (res != null && res.Length > 0)
            {
                res += ", " + text;
            }

            else
            {
                res = text;
            }

            return res;
        }
		
		
        public static string PastTense(this int num)
        {


            string res = num.ToString();


            int last = num % 10;

            switch (last)
            {
                case 1:
                    res += "st";
                    break;
                case 2:
                    res += "nd";
                    break;
                case 3:
                    res += "rd";
                    break;
                default:
                    res += "th";
                    break;
            }
                
            return res;
        
        }

        public static readonly string[] HexPrefixes  = { "#", "0x", "&h" };

        public static bool TryParseAllowHex(this string s, out uint value)
        {
            value = 0;
            if (s == null)
            {
                return false;
            }
            if (s.StartsWith(HexPrefixes))
            {

                string parser = s.TrimStart(HexPrefixes);
                return uint.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out value);
            }
            else
            {
                return uint.TryParse(s, out value);
            }

        }

        public static bool TryParseAllowHex(this string s, out ulong value)
        {
            value = 0;
            if (s == null)
            {
                return false;
            }
            if (s.StartsWith(HexPrefixes))
            {

                string parser = s.TrimStart(HexPrefixes);
                return ulong.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out value);
            }
            else
            {
                return ulong.TryParse(s, out value);
            }

        }

        public static bool StartsWith(this string s, IEnumerable<string> matches)
        {
            foreach (var m in matches)
            {
                if (s.StartsWith(m))
                {
                    return true;
                }
            }

            return false;
        }

        public static string TrimStart(this string s, string trim)
        {
            if (s == null) return null;
            string outs = s;
            if (trim != null && trim.Length > 0)
            {
                while (outs.StartsWith(trim))
                {
                    outs = outs.Substring(trim.Length);
                }
            }
            return outs;
            
        }



        public static string TrimStart(this string s, IEnumerable<string> trims)
        {
            if (s == null) return null;
            string outs = s;
            if (trims != null)
            {
                foreach (var trim in trims)
                {
                    outs = outs.TrimStart(trim);
                }
            }
            return outs;
        }


        public static string TrimEnd(this string s, string trim)
        {
            if (s == null) return null;
            string outs = s;
            if (trim != null && trim.Length > 0)
            {
                while (outs.EndsWith(trim))
                {
                    outs = outs.Substring(0, outs.Length - trim.Length);
                }
            }
            return outs;

        }


        public static string TrimEnd(this string s, IEnumerable<string> trims)
        {
            if (s == null) return null;
            string outs = s;
            if (trims != null)
            {
                foreach (var trim in trims)
                {
                    outs = outs.TrimEnd(trim);
                }
            }
            return outs;
        }

        public static string Trim(this string s, string trim)
        {
            return s.TrimStart(trim).TrimEnd(trim);
        }

        public static string Trim(this string s, IEnumerable<string> trims)
        {
            return s.TrimStart(trims).TrimEnd(trims);
        }


        public static bool IsEmptyOrNull(this string s)
        {
            return s == null || s.Length == 0;
        }

        public static bool NotNullString(this string str)
        {
            return str != null && str.Trim() != "";
        }

        public static bool ContainsIgnoreCase(this string target, string contains)
        {
            if (target == null || contains == null)
            {
                return false;
            }
            return target.ToUpper().Contains(contains.ToString());
        }

        public static bool ContainsOrNullIgnoreCase(this string target, string contains)
        {
            if (contains == null)
            {
                return true;
            }
            return target.ToUpper().Contains(contains.ToString());
        }
        
    }
}
