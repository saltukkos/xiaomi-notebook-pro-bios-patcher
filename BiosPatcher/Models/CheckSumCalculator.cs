using JetBrains.Annotations;

namespace BiosPatcher.Models
{
    public static class CheckSumCalculator
    {
        public static int Calculate([NotNull] byte[] data, int from, int length)
        {
            int result = 0;
            for (int i = from; i < length + from; ++i)
            {
                result += data[i];
            }

            return result;
        }
    }
}