using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

using EmbedIO;

namespace CombatManager.LocalService
{
    public static class LocalServiceUtilities
    {

        public static bool TryGetFirst(this NameValueCollection col, string name, out string val)
        {
            string[] vals = col.GetValues(name);
            return vals.TryGetIndex(0, out val);
        }

        public static bool TryGetInt(this NameValueCollection col, string name, out int value)
        {
            value = 0;
            return (col.TryGetFirst(name, out string str) && int.TryParse(str, out value));
        }

        public static bool TryGetBool (this NameValueCollection col, string name, out bool value)
        {
            value = false;
            return (col.TryGetFirst(name, out string str) && str.TryParseBool(out value));
        }

        public static bool TryGetUint(this NameValueCollection col, string name, out uint value)
        {
            value = 0;
            return (col.TryGetFirst(name, out string str) && str.TryParseAllowHex(out value));
        }
        public static string GetString(this NameValueCollection col, string name, string defval = "")
        {
            if (col.TryGetFirst(name, out string retval))
            {
                return retval;
            }
            return defval;
        }

        public static int GetInt(this NameValueCollection col, string name, int defval = 0)
        {
            if (col.TryGetInt(name, out int retval))
            {
                return retval;
            }
            return defval;
        }

        public static bool GetBool(this NameValueCollection col, string name, bool defval = false)
        {
            if (col.TryGetBool(name, out bool retval))
            {
                return retval;
            }
            return defval;
        }
    }
}
