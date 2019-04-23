using System;
using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Presentation.Converters
{
    public class MusicPropertiesEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var musicFile = ((MusicFile)values[0]);
            // values[1] = musicFile.IsMetadataLoaded; only used to update the Binding
            return musicFile != null && musicFile.IsMetadataLoaded && musicFile.Metadata.IsSupported;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
