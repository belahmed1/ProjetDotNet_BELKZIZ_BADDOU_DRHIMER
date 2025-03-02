using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Converters
{
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dec)
                return dec.ToString(culture);
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (decimal.TryParse(value?.ToString(), out decimal result))
                return result;
            return 0m;
        }
    }

}
