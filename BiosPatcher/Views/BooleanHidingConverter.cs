using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BiosPatcher.Views
{
    public class BooleanHidingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            if (value is bool b)
            {
                result = b;
            }

            return result ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return visibility == Visibility.Visible;
            return false;
        }
    }
}