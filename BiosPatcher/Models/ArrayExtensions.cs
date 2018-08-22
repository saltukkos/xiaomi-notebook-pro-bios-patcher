using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BiosPatcher.Models
{
    public static class ArrayExtensions
    {
        [NotNull]
        public static IReadOnlyList<int> FindSubArrayWithUniqueLeadingElement<T>(
            [NotNull] this T[] array, [NotNull] T[] subArray) where T : IEquatable<T>
        {
            var indices = new List<int>();
            if (subArray.Length == 0)
            {
                return indices;
            }

            var subArrayIndex = 0;
            for (var i = 0; i < array.Length; ++i)
            {
                if (!array[i].Equals(subArray[subArrayIndex]))
                {
                    subArrayIndex = 0;
                    continue;
                }

                if (++subArrayIndex == subArray.Length)
                {
                    indices.Add(i - subArray.Length + 1);
                    subArrayIndex = 0;
                }
            }

            return indices;
        }

        public static bool IsSubArrayEquals<T>(
            [NotNull] this T[] array, int from, int length, T[] other, int otherFrom) where T : IEquatable<T>
        {
            if (from < 0 || otherFrom < 0 || from + length < array.Length || otherFrom + length < other.Length)
            {
                return false;
            }

            for (var i = 0; i < length; ++i)
            {
                if (!array[from + i].Equals(other[otherFrom + i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsStrictlyIncreasing([NotNull] this byte[] data, int from, int length)
        {
            var last = data[from];
            for (var i = from + 1; i < from + length; ++i)
            {
                var current = data[i];
                if (current <= last)
                {
                    return false;
                }

                last = current;
            }

            return true;
        }
    }
}