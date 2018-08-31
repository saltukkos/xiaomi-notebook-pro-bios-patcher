using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public static class CheckSumCalculator
    {
        public static int Calculate([NotNull] byte[] data, int from, int length)
        {
            var result = 0;
            for (var i = from; i < length + from; ++i)
            {
                result += data[i];
            }

            return result;
        }
    }
}