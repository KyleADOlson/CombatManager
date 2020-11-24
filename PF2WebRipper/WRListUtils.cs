using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF2WebRipper
{
    public static class WRListUtils
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
            if (loc < 0 || loc > list.Count - 1)
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

        public static void PushEnd<T>(this IList<T> list, T newItem)
        {
            list.Add(newItem);
        }

        public static void ShortenToLength<T>(this IList<T> list, int length)
        {
            if (length < 0 || list.Count <= length)
            {
                return;
            }
            if (list is List<T>)
            {
                ((List<T>)list).RemoveRange(length, list.Count - length);
            }
            else
            {
                while (list.Count > length)
                {
                    list.PopEnd();
                }
            }
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

        public static void RunOnIndex<T>(this IList<T> list, int index, Action<T> action)
        {
            if (list.IndexInList(index))
            {
                action(list[index]);
            }
        }

        public static void RunOnRange<T>(this IList<T> list, int start, int end, Action<T> action)
        {
            int index = Math.Max(0, start);

            while (index <= end && index < list.Count)
            {
                action(list[index]);
                index++;
            }
        }


        public static List<T> ToListAlways<T>(this T[] array)
        {
            if (array == null)
            {
                return new List<T>(); ;
            }

            return new List<T>(array);
        }

        public static List<T> CopyOrCreate<T>(this List<T> list)
        {
            if (list == null)
            {
                return new List<T>();
            }
            return new List<T>(list);
        }

        public static List<T> ToList<T>(this T[] array)
        {
            if (array == null)
            {
                return null;
            }

            return new List<T>(array);
        }

        public static List<T> TrimFront<T>(this List<T> list, T item)
        {

            while (list.Count >= 0 && list.First().Equals(item))
            {
                list.PopFront();
            }
            return list;
        }

        public static List<T> TrimEnd<T>(this List<T> list, T item)
        {

            while (list.Count >= 0 && list.Last().Equals(item))
            {
                list.PopEnd();
            }
            return list;
        }

        public static List<T> Trim<T>(this List<T> list, T item)
        {
            return list.TrimFront(item).TrimEnd(item); ;
        }

        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> col, IEnumerable<T> items)
        {
            foreach (T t in items)
            {
                col.Add(t);
            }
            return col;
        }

        public static ObservableCollection<T> ReplaceContents<T>(this ObservableCollection<T> col, IEnumerable<T> items)
        {
            col.Clear();
            return col.AddRange(items);
        }

        public static T CloneOrNull<T>(this T obj) where T : ICloneable
        {
            if (obj == null)
            {
                return obj;
            }
            return (T)obj.Clone();
        }

        public static ObservableCollection<T> AddRangeClone<T>(this ObservableCollection<T> col, IEnumerable<T> items) where T : ICloneable
        {

            foreach (T t in items)
            {
                col.Add(t.CloneOrNull());
            }
            return col;
        }
        public static ObservableCollection<T> ReplaceClone<T>(this ObservableCollection<T> col, IEnumerable<T> items) where T : ICloneable
        {
            col.Clear();
            return col.AddRangeClone(items);
        }

        public static ObservableCollection<T> CloneContents<T>(this ObservableCollection<T> col) where T : ICloneable
        {
            if (col == null)
            {
                return null;
            }
            return new ObservableCollection<T>(from s in col select s.CloneOrNull());

        }
    }

    public static class WRArrayUtilities
    {
        public static bool IsEmptyOrNull<T>(this T[] array)
        {
            if (array == null)
            {
                return true;
            }
            return array.Length == 0;
        }

        public static bool NotEmptyOrNull<T>(this T[] array)
        {
            return !array.IsEmptyOrNull();
        }

        public static T[] CombineEvenNull<T>(this T[] array, T[] array2)
        {
            if (array == null)
            {
                return array2;
            }
            else if (array2 == null)
            {
                return array;
            }
            else
            {
                return array.Concat(array2).ToArray();
            }
        }
        public static T[] AppendEvenNull<T>(this T[] array, T item)
        {
            if (array == null)
            {
                return new T[] { item };
            }
            else if (item == null)
            {
                return array;
            }
            else
            {
                return array.Concat(new T[] { item }).ToArray();
            }
        }

        public static T[] PopFront<T>(this T[] array)
        {
            List<T> list = array.ToList();
            list.PopFront();
            return list.ToArray();
        }

        public static T[] PopEnd<T>(this T[] array)
        {
            List<T> list = array.ToList();
            list.PopEnd();
            return list.ToArray();
        }
        public static T[] PopLoc<T>(this T[] array, int loc)
        {
            List<T> list = new List<T>(array);
            list.PopLoc(loc);
            return list.ToArray();
        }

        public static T[] PushFront<T>(this T[] array, T item)
        {
            List<T> list = array.ToList();
            list.PushFront(item);
            return list.ToArray();
        }

        public static T[] PushEnd<T>(this T[] array, T item)
        {
            List<T> list = array.ToList();
            list.PushEnd(item);
            return list.ToArray();
        }

        public static T[] Insert<T>(this T[] array, int index, T item)
        {
            List<T> list = array.ToList();
            list.Insert(index, item);
            return list.ToArray();
        }



        public static T[] InsertRange<T>(this T[] array, int index, IEnumerable<T> items)
        {
            List<T> list = array.ToList();
            list.InsertRange(index, items);
            return list.ToArray();
        }

        public static T[] TrimFront<T>(this T[] array, T item)
        {
            return array.ToList().TrimFront(item).ToArray();
        }

        public static T[] TrimEnd<T>(this T[] array, T item)
        {

            return array.ToList().TrimEnd(item).ToArray();
        }

        public static T[] Trim<T>(this T[] array, T item)
        {

            return array.ToList().Trim(item).ToArray();
        }

        public static bool TryGetIndex<T>(this T[] array, int index, out T item)
        {
            if (array == null || array.Length <= index)
            {
                item = default(T);
                return false;
            }

            item = array[index];
            return true;

        }

        public static bool TryGetIndexNotNull<T>(this T[] array, int index, out T item)
        {
            if (array == null || array.Length <= index || array[index] == null)
            {
                item = default(T);
                return false;
            }

            item = array[index];
            return true;

        }


        public static bool TryGetInt(this string[] array, int index, out int value)
        {
            if (array == null || array.Length <= index)
            {
                value = 0;
                return false;
            }

            string text = array[index];
            return int.TryParse(text, out value);
        }

        public static bool TryGetGuid(this string[] array, int index, out Guid value)
        {
            if (array == null || array.Length <= index)
            {
                value = Guid.Empty;
                return false;
            }

            string text = array[index];
            return Guid.TryParse(text, out value);
        }

        public static bool TryGetUint(this string[] array, int index, out uint value)
        {
            if (array == null || array.Length <= index)
            {
                value = 0;
                return false;
            }

            string text = array[index];
            if (text.StartsWith("0x") || text.StartsWith("#"))
            {

            }

            return uint.TryParse(text, out value);
        }


        public static bool TryGetFloat(this string[] array, int index, out float value)
        {
            if (array == null || array.Length <= index)
            {
                value = 0;
                return false;
            }

            string text = array[index];
            return float.TryParse(text, out value);
        }
        public static bool TryGetDouble(this string[] array, int index, out double value)
        {
            if (array == null || array.Length <= index)
            {
                value = 0;
                return false;
            }

            string text = array[index];
            return double.TryParse(text, out value);
        }

        public static bool TryGetBool(this string[] array, int index, out bool value)
        {
            if (array.TryGetIndexNotNull(index, out var text))
            {
                value = false;
                return false;
            }
            return text.TryParseBool(out value);
        }

        public static bool TryParseBool(this string txt, out bool value)
        {
            string text = txt.ToLower();
            if (text == "true" || text == "1")
            {
                value = true;
                return true;
            }
            if (text == "false" || text == "0")
            {
                value = false;
                return true;
            }

            value = false;
            return false;
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

        public static void WeaveList<T>(this IEnumerable<T> list, Action<T> onItem = null, Action<T, T> betweenItems = null)
        {
            var enu = list.GetEnumerator();
            bool more = enu.MoveNext();
            while (more)
            {
                T item = enu.Current;
                onItem?.Invoke(item);

                more = enu.MoveNext();
                if (more)
                {
                    T next = enu.Current;
                    betweenItems?.Invoke(item, next);
                }
            }
        }

        public static void All<T>(this Action<T> action) where T : System.Enum
        {
            foreach (T v in Enum.GetValues(typeof(T)))
            {
                action(v);
            }
        }

    }

    public static class WRTupleUtils
    {
        public static List<T> ToList<T>(this (T, T) tuple)
        {
            return new List<T>() { tuple.Item1, tuple.Item2 };

        }

        public static List<T> ToList<T>(this (T, T, T) tuple)
        {
            return new List<T>() { tuple.Item1, tuple.Item2, tuple.Item3 };

        }

        public static List<T> ToList<T>(this (T, T, T, T) tuple)
        {
            return new List<T>() { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };

        }

        public static List<T> ToList<T>(this (T, T, T, T, T) tuple)
        {
            return new List<T>() { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };

        }

        public static List<T> ToList<T>(this (T, T, T, T, T, T) tuple)
        {
            return new List<T>() { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };

        }

        public static List<string> ToStringList<T, U>(this (T, U) tuple)
        {
            return new List<string>() { tuple.Item1.ToString(), tuple.Item2.ToString() };

        }
        public static List<string> ToStringList<T, U, V>(this (T, U, V) tuple)
        {
            return new List<string>() { tuple.Item1.ToString(), tuple.Item2.ToString(), tuple.Item3.ToString() };

        }
        public static List<string> ToStringList<T, U, V, W>(this (T, U, V, W) tuple)
        {
            return new List<string>() { tuple.Item1.ToString(), tuple.Item2.ToString(), tuple.Item3.ToString(), tuple.Item4.ToString() };

        }
        public static List<string> ToStringList<T, U, V, W, X>(this (T, U, V, W, X) tuple)
        {
            return new List<string>() { tuple.Item1.ToString(), tuple.Item2.ToString(), tuple.Item3.ToString(), tuple.Item4.ToString(), tuple.Item5.ToString() };

        }

        public static List<string> ToStringList<T, U, V, W, X, Y>(this (T, U, V, W, X, Y) tuple)
        {
            return new List<string>() { tuple.Item1.ToString(), tuple.Item2.ToString(), tuple.Item3.ToString(), tuple.Item4.ToString(), tuple.Item5.ToString(), tuple.Item6.ToString() };

        }


    }

}
