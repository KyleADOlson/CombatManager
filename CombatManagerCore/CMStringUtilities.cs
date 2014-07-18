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

        public static String ToTokenString(this List<String> strings, char token)
        {
            if (strings == null || strings.Count == 0)
            {
                return null;
            }
            String list = "";
            foreach (String str in strings)
            {
                if (list != "")
                {
                    list += token + " ";
                }
                list += str;
            }
            return list;
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
    }
}
