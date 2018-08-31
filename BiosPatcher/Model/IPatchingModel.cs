using JetBrains.Annotations;

namespace BiosPatcher.Model
{
    public interface IPatchingModel
    {
        int[] OffLevels { get; }
        int[] OnLevels { get; }
        string CurrentStateDescription { get; }
        void SetInputFilePath([NotNull] string filename);
        void SetOutputFilePath([NotNull] string filename);
        bool CheckCorrect();
        string Patch();
    }
}