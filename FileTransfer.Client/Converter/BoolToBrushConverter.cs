using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FileTransfer.Client.Converter
{
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                if ((bool)value)
                    return Brushes.Green;

                return Brushes.Red;
            }
            
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Brushes)
            {
                if (value == Brushes.Green)
                    return true;

                return false;
            }

            throw new ArgumentException();
        }
    }
}
