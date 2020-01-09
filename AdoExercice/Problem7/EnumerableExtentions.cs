using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Problem7
{
    public static class EnumerableExtentions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var arr = enumerable.ToArray();

            for (int i = 0; i < arr.Count(); i++)
            {
                action(arr[i], i);
            }

            return enumerable;
        }
    }
}
