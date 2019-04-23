using System;
using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Domain.Transcoding;
using Waf.MusicManager.Presentation.Properties;

namespace Waf.MusicManager.Presentation.Converters
{
    public class TranscodeStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var transcodeStatus = (TranscodeStatus)value;
            switch (transcodeStatus)
            {
                case TranscodeStatus.InProgress:
                    return Resources.InProgress;
                case TranscodeStatus.Pending:
                    return Resources.Pending;
                case TranscodeStatus.Error:
                    return Resources.Error;
                case TranscodeStatus.Completed:
                    return Resources.Completed;
                default:
                    throw new InvalidOperationException("Enum value is unknown.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
