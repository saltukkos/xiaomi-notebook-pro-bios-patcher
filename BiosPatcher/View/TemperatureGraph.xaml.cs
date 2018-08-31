using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

namespace BiosPatcher.View
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

        private void ResetHighlight()
        {
            Level0Graph.Fill = (Brush) FindResource("Color0");
            Level1Graph.Fill = (Brush) FindResource("Color1");
            Level2Graph.Fill = (Brush) FindResource("Color2");
            Level3Graph.Fill = (Brush) FindResource("Color3");
            Level4Graph.Fill = (Brush) FindResource("Color4");
            Level5Graph.Fill = (Brush) FindResource("Color5");
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ResetHighlight();
            DescriptionLabel.Content = Description;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            ResetHighlight();
            var position = e.GetPosition(Canvas);

            if (position.X / Canvas.ActualWidth + position.Y / Canvas.ActualHeight < 1)
            {
                DescriptionLabel.Content = Description;
                return;
            }

            var graphs = new[] {Level0Graph, Level1Graph, Level2Graph, Level3Graph, Level4Graph, Level5Graph};
            var levels = new[] {Level1, Level2, Level3, Level4, Level5};
            var levelUnderCursor = 5;
            for (var i = 0; i < levels.Length; ++i)
            {
                if (GetWidthForTemperature(levels[i]) > position.X)
                {
                    levelUnderCursor = i;
                    break;
                }
            }

            graphs[levelUnderCursor].Fill = new SolidColorBrush(Color.FromRgb(0, 255, 200));
           
            if (levelUnderCursor > 0)
            {
                DescriptionLabel.Content = $"{Description}{Environment.NewLine}Fan speed {levelUnderCursor}, at {levels[levelUnderCursor - 1]} °C";
            }
            else
            {
                DescriptionLabel.Content = $"{Description}{Environment.NewLine}Fan turned off";
            }
        }
    }
}