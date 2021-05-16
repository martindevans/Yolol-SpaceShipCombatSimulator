using System;
using System.Collections.Generic;

namespace ShipCombatCore.Extensions
{
    public static class DictionaryExtensions
    {
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dict, TK key, Func<TK, TV> value)
        {
            if (!dict.TryGetValue(key, out var v))
            {
                v = value(key);
                dict[key] = v;
            }
            return v;
        }
    }
}
