using System;
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


        public static string PlusFormat(this int number)
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
