using System;
using System.Globalization;
using System.Windows.Data;

namespace FileTransfer.Client.Converter
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                if ((bool)value)
                    return "YES";

                return "NO";
            }

            throw new ArgumentException();            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if (value.ToString().Equals("YES"))
                    return true;

                return false;
            }

            throw new ArgumentException();
        }
    }
}
