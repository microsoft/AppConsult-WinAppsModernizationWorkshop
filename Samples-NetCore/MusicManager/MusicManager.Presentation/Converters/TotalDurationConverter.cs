using System;
using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Presentation.Properties;

namespace Waf.MusicManager.Presentation.Converters
{
    public class TotalDurationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isTotalDurationEstimated = (bool)values[0];
            var totalDuration = (string)new DurationConverter().Convert(values[1], null, null, null);
            if (isTotalDurationEstimated)
            {
                totalDuration = string.Format(CultureInfo.CurrentCulture, Resources.AboutDuration, totalDuration);
            }
            return totalDuration;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
