using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Gauniv.WebServer.Dtos;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Converters
{
    public class CategoriesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<CategoryDto> categories)
            {
                return string.Join(", ", categories.Select(c => c.Name));
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
