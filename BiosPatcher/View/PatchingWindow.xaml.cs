using System.Windows;
using BiosPatcher.ViewModel;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace BiosPatcher.View
{
    /// <summary>
    /// Interaction logic for Patching.xaml
    /// </summary>
    //[Component]
    public partial class PatchingWindow
    {
        [NotNull]
        private readonly IPatchingViewModel _viewModel;

        public PatchingWindow([NotNull] IPatchingViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void SelectInputFileName(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                InputFileName.Text = openFileDialog.FileName;
            }
        }

        private void SelectOutputFileName(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                OutputFileName.Text = openFileDialog.FileName;
            }
        }

        private void PatchClicked(object sender, RoutedEventArgs e)
        {
            var result = _viewModel.Patch();
            MessageBox.Show(result);
        }
    }
}
