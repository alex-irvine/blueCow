using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random rand)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static bool OrderAndStringEquals(this List<string> listToCheck, List<string> secondList)
        {
            if (listToCheck.Count != secondList.Count)
            {
                return false;
            }
            for (var i = 0; i < listToCheck.Count; i++)
            {
                if (listToCheck[i] != secondList[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static int IndexOf<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            int i = 0;
            foreach (var pair in dictionary)
            {
                if (pair.Key.Equals(key))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public static bool OrderAndBoolEquals(this bool[] array1, bool[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (var i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }


}
