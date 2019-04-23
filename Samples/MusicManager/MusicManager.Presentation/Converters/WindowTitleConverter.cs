using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class WindowTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var stringList = values.OfType<string>().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return string.Join(" - ", stringList);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
