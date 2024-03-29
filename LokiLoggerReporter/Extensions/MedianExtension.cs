using System;
using System.Collections.Generic;
using System.Linq;

namespace lokiloggerreporter.Extensions
{
    public static class MedianExtensions
{
    
    public static IEnumerable<T> GetNth<T>(this IList<T> list, int n)
    {
        for (int i = 0; i < list.Count;i += n)
            yield return list[i];
    }
    public static IEnumerable<List<T>> SplitList<T>(this List<T> locations, int nSize=30)  
    {        
        for (int i = 0; i < locations.Count; i += nSize) 
        { 
            yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i)); 
        }  
    } 
    
    public static double Median(this IEnumerable<int> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.OrderBy(n => n).ToArray();
        if (data.Length == 0)
            throw new InvalidOperationException();
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
        return data[data.Length / 2];
    }
    
    public static double? Median(this IEnumerable<int?> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
        if (data.Length == 0)
            return null;
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
        return data[data.Length / 2];
    }
    
    public static double Median(this IEnumerable<long> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.OrderBy(n => n).ToArray();
        if (data.Length == 0)
            throw new InvalidOperationException();
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
        return data[data.Length / 2];
    }
    
    public static double? Median(this IEnumerable<long?> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
        if (data.Length == 0)
            return null;
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
        return data[data.Length / 2];
    }
    
    public static float Median(this IEnumerable<float> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.OrderBy(n => n).ToArray();
        if (data.Length == 0)
            throw new InvalidOperationException();
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0f;
        return data[data.Length / 2];
    }
    
    public static float? Median(this IEnumerable<float?> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
        if (data.Length == 0)
            return null;
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0f;
        return data[data.Length / 2];
    }
    
    public static double Median(this IEnumerable<double> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.OrderBy(n => n).ToArray();
        if (data.Length == 0)
            throw new InvalidOperationException();
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
        return data[data.Length / 2];
    }
    
    public static double? Median(this IEnumerable<double?> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
        if (data.Length == 0)
            return null;
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
        return data[data.Length / 2];
    }
    
    public static decimal Median(this IEnumerable<decimal> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.OrderBy(n => n).ToArray();
        if (data.Length == 0)
            throw new InvalidOperationException();
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0m;
        return data[data.Length / 2];
    }
    
    public static decimal? Median(this IEnumerable<decimal?> source)
    {
        if (source == null)
            throw new ArgumentNullException("source");
        var data = source.Where(n => n.HasValue).Select(n => n.Value).OrderBy(n => n).ToArray();
        if (data.Length == 0)
            return null;
        if (data.Length % 2 == 0)
            return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0m;
        return data[data.Length / 2];
    }
    
    public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static float Median<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static float? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static decimal Median<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
    {
        return source.Select(selector).Median();
    }
    
    public static decimal? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
    {
        return source.Select(selector).Median();
    }
}
}