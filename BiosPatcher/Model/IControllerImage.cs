using System.Collections.Generic;

namespace BiosPatcher.Model
{
    internal interface IControllerImage
    {
        IReadOnlyList<byte> Image { get; }
        IReadOnlyList<int> TableOffsets { get; }
        int Checksum { get; }
    }
}