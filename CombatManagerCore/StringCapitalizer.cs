using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CombatManager
{
    public class StringCapitalizer
    {

        public static SortedDictionary<String, String> ignoreWords;

        static StringCapitalizer()
        {
            ignoreWords = new SortedDictionary<String, String>(new InsensitiveComparer());

            string[] ignore = { "the", "of", "from", "to", "and" };

            foreach (string str in ignore)
            {
                ignoreWords[str] = str;
            }
        }




        public static string Capitalize(string text)
        {
            if (text != null)
            {

                Regex regWord = new Regex("\\w+('s)?");

                text = regWord.Replace(text, delegate(Match m)
                {
                    string x = m.Value;

                    if (!ignoreWords.ContainsKey(x))
                    {

                        x = x.Substring(0, 1).ToUpper() + x.Substring(1);
                    }

                    return x;
                });
            }

            return text;
        }
    }
}
