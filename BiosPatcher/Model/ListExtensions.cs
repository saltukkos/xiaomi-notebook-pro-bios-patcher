using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public static class ListExtensions
    {
        [NotNull]
        public static IReadOnlyList<int> FindSubListWithUniqueLeadingElement<T>(
            [NotNull] this IReadOnlyList<T> list, [NotNull] IReadOnlyList<T> subList) where T : IEquatable<T>
        {
            var indices = new List<int>();
            if (subList.Count == 0)
            {
                return indices;
            }

            var subArrayIndex = 0;
            for (var i = 0; i < list.Count; ++i)
            {
                if (!list[i].Equals(subList[subArrayIndex]))
                {
                    subArrayIndex = 0;
                    continue;
                }

                if (++subArrayIndex == subList.Count)
                {
                    indices.Add(i - subList.Count + 1);
                    subArrayIndex = 0;
                }
            }

            return indices;
        }

        public static bool IsSubListEquals<T>(
            [NotNull] this IReadOnlyList<T> list, int from, int length, IReadOnlyList<T> other, int otherFrom) where T : IEquatable<T>
        {
            if (from < 0 || otherFrom < 0 || from + length >= list.Count || otherFrom + length >= other.Count)
            {
                return false;
            }

            for (var i = 0; i < length; ++i)
            {
                if (!list[from + i].Equals(other[otherFrom + i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsAlternatingStrictlyIncreasing<T>([NotNull] this IReadOnlyList<T> data, int from, int length) where T : IComparable<T>
        {
            if (length < 2)
            {
                return true;
            }

            var last = data[from + 1];
            var jumped = true;
            for (var i = from; i < from + length; i = jumped ? i + 3 : i - 1, jumped = !jumped)
            {
                var current = data[i];
                if (current.CompareTo(last) <= 0)
                {
                    return false;
                }

                last = current;
            }

            return true;
        }
    }
}