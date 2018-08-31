using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public interface IBitConverterLittleEndian
    {
        int ReadInt([NotNull] byte[] bytes, int offset);
        void WriteInt(int value, [NotNull] byte[] bytes, int offset);
    }
}