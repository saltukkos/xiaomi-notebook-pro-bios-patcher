using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using BiosPatcher.Models;
using JetBrains.Annotations;

namespace BiosPatcher.ViewModels
{
    public sealed class PatchingViewModel : INotifyPropertyChanged
    {
        [NotNull]
        private readonly PatchingModel _model;

        [NotNull]
        private string _inputFileName = string.Empty;

        [NotNull]
        private string _outputFileName = string.Empty;

        private bool _isAutoSettingOut = true;

        public PatchingViewModel()
        {
            _model = new PatchingModel();
        }

        [NotNull]
        public string InputFileName
        {
            get { return _inputFileName; }
            set
            {
                _inputFileName = value;
                _model.SetInputFilePath(value);
                OnPropertyChanged();

                if (_isAutoSettingOut)
                {
                    AutoSetOutput();
                }
            }
        }

        [NotNull]
        public string OutputFileName
        {
            get { return _outputFileName; }
            set
            {
                _isAutoSettingOut = false;
                _outputFileName = value;
                _model.SetOutputFilePath(value);
                OnPropertyChanged();
            }
        }

        public bool InputCorrect
        {
            get { return _model.CheckCorrect(); }
        }

        [NotNull]
        public string ErrorMessage
        {
            get { return _model.LastError; }
        }

        public int CustomTablesOffset
        {
            get { return _model.CustomOffset; }
            set
            {
                _model.CustomOffset = value;
                OnPropertyChanged();
            }
        }

        #region LevelsData

        public int Level1Off
        {
            get { return _model.OffLevels[0]; }
            set
            {
                _model.OffLevels[0] = value;
                OnPropertyChanged();
            }
        }

        public int Level1On
        {
            get { return _model.OnLevels[0]; }
            set
            {
                _model.OnLevels[0] = value;
                OnPropertyChanged();
            }
        }

        public int Level2Off
        {
            get { return _model.OffLevels[1]; }
            set
            {
                _model.OffLevels[1] = value;
                OnPropertyChanged();
            }
        }

        public int Level2On
        {
            get { return _model.OnLevels[1]; }
            set
            {
                _model.OnLevels[1] = value;
                OnPropertyChanged();
            }
        }

        public int Level3Off
        {
            get { return _model.OffLevels[2]; }
            set
            {
                _model.OffLevels[2] = value;
                OnPropertyChanged();
            }
        }

        public int Level3On
        {
            get { return _model.OnLevels[2]; }
            set
            {
                _model.OnLevels[2] = value;
                OnPropertyChanged();
            }
        }

        public int Level4Off
        {
            get { return _model.OffLevels[3]; }
            set
            {
                _model.OffLevels[3] = value;
                OnPropertyChanged();
            }
        }

        public int Level4On
        {
            get { return _model.OnLevels[3]; }
            set
            {
                _model.OnLevels[3] = value;
                OnPropertyChanged();
            }
        }

        public int Level5Off
        {
            get { return _model.OffLevels[4]; }
            set
            {
                _model.OffLevels[4] = value;
                OnPropertyChanged();
            }
        }

        public int Level5On
        {
            get { return _model.OnLevels[4]; }
            set
            {
                _model.OnLevels[4] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public string Patch()
        {
            return _model.Patch();
        }

        private void AutoSetOutput()
        {
            var extension = Path.GetExtension(_inputFileName);
            _outputFileName =
                $"{_inputFileName.Substring(0, _inputFileName.Length - extension.Length)}-patched{extension}";
            _model.SetOutputFilePath(_outputFileName);
            OnPropertyChanged(nameof(OutputFileName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputCorrect)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
    }
}