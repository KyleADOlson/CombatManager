using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public static class CMMathUtilities
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static double Lerp(this double t, double a, double b)
        {

            double tv = t.Clamp(0, 1);

            return (b - a) * tv + a;
           
        }

        public static T Max<T>(this T val, T max) where T : IComparable<T>
        {
            if (val.CompareTo(max) > 0) return max;
            else return val;
        }


        public static T Min<T>(this T val, T min) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else return val;
        }

        public static bool IsEven(this int val)
        {
            return val % 2 == 0;
        }
        public static bool IsEven(this long val)
        {
            return val % 2 == 0;
        }

        public static bool IsOdd(this int val)
        {
            return val % 2 == 1;
        }

        public static bool IsOdd(this long val)
        {
            return val % 2 == 1;
        }


        public static bool IsFactor(this int val, int num)
        {
            return val % num == 0;
        }
        public static bool IsFactor(this int val, long num)
        {
            return val % num == 0;
        }

    }
}
