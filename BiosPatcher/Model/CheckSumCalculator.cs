namespace BiosPatcher.Model
{
    [Component]
    internal sealed class CheckSumCalculator : ICheckSumCalculator
    {
        public int Calculate(byte[] data, int from, int length)
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