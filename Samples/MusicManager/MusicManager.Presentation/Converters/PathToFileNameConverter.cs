using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class PathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ConverterHelper.IsParameterSet("WithExtension", parameter))
            {
                return Path.GetFileName((string)value);
            }
            return Path.GetFileNameWithoutExtension((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
