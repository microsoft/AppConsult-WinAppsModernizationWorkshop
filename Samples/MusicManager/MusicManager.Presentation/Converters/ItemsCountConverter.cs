using System;
using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Presentation.Properties;

namespace Waf.MusicManager.Presentation.Converters
{
    public class ItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int itemsCount = (int)value;
            if (itemsCount == 1)
            {
                return Resources.OneItem;
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, Resources.NumberOfItems, itemsCount);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
