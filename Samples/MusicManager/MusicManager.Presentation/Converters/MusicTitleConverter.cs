using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.Converters
{
    public class MusicTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.First() == DependencyProperty.UnsetValue) { return DependencyProperty.UnsetValue; }
            
            var fileName = (string)values[0];
            var artists = values[1] as IEnumerable<string>;
            var title = values[2] as string;
            
            return MusicTitleHelper.GetTitleText(fileName, artists, title);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
