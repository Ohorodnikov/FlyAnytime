using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Tools
{
    public static class DictionaryExt
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueGenerator)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            var val = valueGenerator(key);

            dict.Add(key, val);

            return val;
        }
    }
}
