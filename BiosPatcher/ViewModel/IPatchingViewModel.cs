using System.ComponentModel;

namespace BiosPatcher.ViewModel
{
    public interface IPatchingViewModel : INotifyPropertyChanged
    {
        string InputFileName { get; set; }
        string OutputFileName { get; set; }
        bool InputCorrect { get; }
        string ErrorMessage { get; }
        int Level1Off { get; set; }
        int Level1On { get; set; }
        int Level2Off { get; set; }
        int Level2On { get; set; }
        int Level3Off { get; set; }
        int Level3On { get; set; }
        int Level4Off { get; set; }
        int Level4On { get; set; }
        int Level5Off { get; set; }
        int Level5On { get; set; }
        string Patch();
    }
}