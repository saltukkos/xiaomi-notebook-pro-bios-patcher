using System.Collections.Generic;

namespace BiosPatcher.Model
{
    [Component]
    public sealed class DefaultValuesProvider : IDefaultValuesProvider
    {
        public IReadOnlyList<byte> OffLevels { get; } = new byte[] {38, 43, 47, 53, 59};

        public IReadOnlyList<byte> OnLevels { get; } = new byte[] {42, 46, 52, 58, 64};
    }
}