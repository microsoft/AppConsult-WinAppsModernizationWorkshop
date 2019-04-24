using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class MusicPropertiesProgressVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isMetadataLoaded = values[0] is bool b && b;
            var loadError = values[1];
            
            return !isMetadataLoaded && loadError == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
