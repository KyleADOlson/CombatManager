using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager
{
    public static class CMListUtilities
    {
        public static T PopFront<T>(this IList<T> list)
        {
            return list.PopLoc(0);
        }

        public static T PopEnd<T>(this IList<T> list)
        {
            return list.PopLoc(list.Count - 1);
        }

        public static T PopLoc<T>(this IList<T> list, int loc)
        {
            if (loc < 0 || loc > list.Count -1)
            {
                return default(T);
            }
            else
            {
                T retval = list[loc];
                list.RemoveAt(loc);
                return retval;

            }
        }

        public static void PushFront<T>(this IList<T> list, T newItem)
        {
            list.Insert(0, newItem);
        }

        public static void PushEnd<T>(this IList<T> list,  T newItem)
        {
            list.Append(newItem);
        }

        public static bool IsEmptyOrNull<T>(this ICollection<T> list)
        {
            return list == null || list.Count == 0;
               
        }

        public static T FirstOrDefaultSafe<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return default(T);

            }
            return list.FirstOrDefault();

        }

        public static T FirstOrDefaultSafe<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            if (list == null)
            {
                return default(T);
               
            }
            return list.FirstOrDefault(predicate);
                

        }
    }
}
