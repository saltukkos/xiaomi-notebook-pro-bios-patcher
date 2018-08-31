using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public static class DefaultValues
    {
        [NotNull]
        public static readonly int[] OffLevels = {38, 43, 47, 53, 59};

        [NotNull]
        public static readonly int[] OnLevels = {42, 46, 52, 58, 64};
    }
}