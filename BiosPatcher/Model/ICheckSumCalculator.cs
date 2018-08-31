using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public interface ICheckSumCalculator
    {
        int Calculate([NotNull] byte[] data, int from, int length);
    }
}