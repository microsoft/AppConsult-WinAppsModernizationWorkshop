using System;
using System.Globalization;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var duration = (TimeSpan)value;
            if (duration < TimeSpan.FromHours(1))
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}:{1:00}:{2:00}", (int)duration.TotalHours, duration.Minutes, duration.Seconds);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
