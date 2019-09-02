using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            list.Add(newItem);
        }

        public static bool IsEmptyOrNull<T>(this ICollection<T> list)
        {
            return list == null || list.Count == 0;
               
        }

        public static bool NotEmptyOrNull<T>(this ICollection<T> list)
        {
            return !list.IsEmptyOrNull();
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

        public static bool IndexInList<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        

    }

    public static class DictionaryLister
    {
        public static void ToTupleList<T, U>(this Dictionary<T, U> source, List<Tuple<T, U>> outputList)
        {
            outputList.Clear();

            foreach (var kv in source)
            {
                outputList.Add(new Tuple<T, U>(kv.Key, kv.Value));
            }
        }

        public static void ToTupleList<T, U>(this Dictionary<T, U> source, ObservableCollection<Tuple<T, U>> outputList)
        {
            outputList.Clear();

            foreach (var kv in source)
            {
                outputList.Add(new Tuple<T, U>(kv.Key, kv.Value));
            }
        }

        public static void UpdateFromTupleList<T, U>(this Dictionary<T, U> target, List<Tuple<T, U>> source)
        {
            target.Clear();
            foreach (var t in source)
            {
                target[t.Item1] = t.Item2;
            }

        }

        public static void UpdateFromTupleList<T, U>(this Dictionary<T, U> target, ObservableCollection<Tuple<T, U>> source)
        {
            target.Clear();
            foreach (var t in source)
            {
                target[t.Item1] = t.Item2;
            }

        }

        public static void ToList<T>(this HashSet<T> source, ObservableCollection<T> target)
        {
            target.Clear();
            foreach (var x in source)
            {
                target.Add(x);
            }
        }

        public static void FromList<T>(this HashSet<T> target, ObservableCollection<T> source)
        {
            target.Clear();

            foreach (var x in source)
            {
                target.Add(x);
            }
        }


    }
}
