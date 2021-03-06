﻿namespace Zagorapps.Core.Library.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class GenericExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.SafeAny();
        }

        public static bool SafeAny<T>(this IEnumerable<T> source, Func<T, bool> filter = null)
        {
            if (source == null)
            {
                return false;
            }

            return filter == null
                ? source.Any()
                : source.Any(filter);
        }

        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(type, typeof(TAttribute));
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return Attribute.GetCustomAttributes(type, typeof(TAttribute)).Select(t => (TAttribute)t).ToArray();
        }
    }
}