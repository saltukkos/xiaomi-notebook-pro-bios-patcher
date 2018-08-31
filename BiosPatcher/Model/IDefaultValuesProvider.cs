using System.Collections.Generic;
using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public interface IDefaultValuesProvider
    {
        [NotNull]
        IReadOnlyList<byte> OffLevels { get; }

        [NotNull]
        IReadOnlyList<byte> OnLevels { get; }
    }
}