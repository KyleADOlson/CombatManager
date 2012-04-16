using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CombatManager
{
    public static class CMRegexUtilities
    {
        public static bool GroupSuccess(this Match m, string group)
        {
            return m.Groups[group].Success;
        }

        public static bool AnyGroupSuccess(this Match m, IEnumerable<string> groups)
        {
            foreach (string s in groups)
            {
                if (m.GroupSuccess(s))
                {
                    return true;
                }
            }

            return false;
        }

        public static string Value(this Match m, string group)
        {
            string val = null;

            if (m.Groups[group].Success)
            {
                val = m.Groups[group].Value;
            }

            return val;
        }


        public static int? IntValue(this Match m, string name)
        {
            if (m.Groups[name].Success)
            {
                int val;
                if (int.TryParse(m.Groups[name].Value, out val))
                {
                    return val;
                }
            }
            return null;
        }
    }
}
