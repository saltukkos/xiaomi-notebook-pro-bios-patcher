using System.Windows;
using BiosPatcher.ViewModel;
using Microsoft.Win32;

namespace BiosPatcher.View
{
    /// <summary>
    /// Interaction logic for Patching.xaml
    /// </summary>
    public partial class Patching
    {
        public Patching()
        {
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
            var result = ((PatchingViewModel)DataContext).Patch();
            MessageBox.Show(result);
        }
    }
}
