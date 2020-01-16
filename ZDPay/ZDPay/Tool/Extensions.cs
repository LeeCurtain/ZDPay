using System;
using System.Collections.Generic;
using System.Text;

namespace ZDPay.Tool
{
    public static class Extensions
    {
        public static Dictionary<TValue, TKey>AsInverted<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            var inverted = new Dictionary<TValue, TKey>();
            foreach (KeyValuePair<TKey, TValue> key in source)
                inverted.Add(key.Value, key.Key);
            return inverted;
        }
        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            return new SortedDictionary<TKey, TValue>(dictionary);
        }

        public static IDictionary<TKey, TValue> Sort<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            return new SortedDictionary<TKey, TValue>(dictionary, comparer);
        }
    }
}
