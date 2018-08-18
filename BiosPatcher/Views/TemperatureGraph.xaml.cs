using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace BiosPatcher.Views
{
    public sealed partial class TemperatureGraph
    {
        public TemperatureGraph()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(string), typeof(TemperatureGraph), new FrameworkPropertyMetadata(ChangeDescription));

        public static readonly DependencyProperty FromTemperatureProperty = DependencyProperty.Register(
            nameof(FromTemperature), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        public static readonly DependencyProperty ToTemperatureProperty = DependencyProperty.Register(
            nameof(ToTemperature), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        public static readonly DependencyProperty Level1Property = DependencyProperty.Register(
            nameof(Level1), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        public static readonly DependencyProperty Level2Property = DependencyProperty.Register(
            nameof(Level2), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        public static readonly DependencyProperty Level3Property = DependencyProperty.Register(
            nameof(Level3), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        public static readonly DependencyProperty Level4Property = DependencyProperty.Register(
            nameof(Level4), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        public static readonly DependencyProperty Level5Property = DependencyProperty.Register(
            nameof(Level5), typeof(int), typeof(TemperatureGraph), new FrameworkPropertyMetadata(RedrawGraph));

        [CanBeNull]
        public string Description
        {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public int FromTemperature
        {
            get { return (int) GetValue(FromTemperatureProperty); }
            set { SetValue(FromTemperatureProperty, value); }
        }

        public int ToTemperature
        {
            get { return (int) GetValue(ToTemperatureProperty); }
            set { SetValue(ToTemperatureProperty, value); }
        }

        public int Level1
        {
            get { return (int) GetValue(Level1Property); }
            set { SetValue(Level1Property, value); }
        }

        public int Level2
        {
            get { return (int) GetValue(Level2Property); }
            set { SetValue(Level2Property, value); }
        }

        public int Level3
        {
            get { return (int) GetValue(Level3Property); }
            set { SetValue(Level3Property, value); }
        }

        public int Level4
        {
            get { return (int) GetValue(Level4Property); }
            set { SetValue(Level4Property, value); }
        }

        public int Level5
        {
            get { return (int) GetValue(Level5Property); }
            set { SetValue(Level5Property, value); }
        }

        private int GetWidthForTemperature(int temperature)
        {
            var percent = (temperature - FromTemperature) / (double) (ToTemperature - FromTemperature);
            var res = (int) (percent * Canvas.ActualWidth);
            return res > 0 ? res : 0;
        }

        private void RedrawGraph()
        {
            Header.Points = new PointCollection(new[]
            {
                new Point(0, Canvas.ActualHeight),
                new Point(-1, Canvas.ActualHeight),
                new Point(-1, -1),
                new Point(Canvas.ActualWidth, -1),
                new Point(Canvas.ActualWidth, 0)
            });

            Level0Graph.Width = GetWidthForTemperature(Level1);
            Level1Graph.Width = GetWidthForTemperature(Level2);
            Level2Graph.Width = GetWidthForTemperature(Level3);
            Level3Graph.Width = GetWidthForTemperature(Level4);
            Level4Graph.Width = GetWidthForTemperature(Level5);
            Level5Graph.Width = Canvas.ActualWidth;
        }

        private static void ChangeDescription(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TemperatureGraph graph))
            {
                return;
            }

            graph.DescriptionLabel.Content = graph.Description;
        }

        private static void RedrawGraph(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TemperatureGraph)?.RedrawGraph();
        }

        private void OnCanvasResize(object sender, SizeChangedEventArgs e)
        {
            RedrawGraph();
        }
    }
}