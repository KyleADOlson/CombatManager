using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{

    public class InsensitiveEqualityCompararer : IEqualityComparer<string>
    {
        public bool Equals(string a, string b)
        {
            return String.Compare(a, b, true) == 0;
        }

        public int GetHashCode(string a)
        {
            return a.ToLower().GetHashCode();
        }
    }

    public class InsensitiveComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            return String.Compare(a, b, true);
        }
    }
}
