using System;

using System.IO;
using System.Collections;
using System.Linq;
namespace lokiloggerreporter.Extensions {
	public static class Time {

		public static DateTime Now => DateTime.Now;

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
{
    HashSet<TKey> seenKeys = new HashSet<TKey>();
    foreach (TSource element in source)
    {
        if (seenKeys.Add(keySelector(element)))
        {
            yield return element;
        }
    }
}
	}
}