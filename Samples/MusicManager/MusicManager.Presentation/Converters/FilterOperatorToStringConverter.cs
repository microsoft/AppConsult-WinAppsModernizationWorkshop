using System;
using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Presentation.Properties;

namespace Waf.MusicManager.Presentation.Converters
{
    public class FilterOperatorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return null; }

            var isDescription = ConverterHelper.IsParameterSet("description", parameter);
            var filterOperator = (FilterOperator)value;
            switch (filterOperator)
            {
                case FilterOperator.Ignore:
                    return isDescription ? Resources.IgnoreValue : "";
                case FilterOperator.LessThanOrEqual:
                    return isDescription ? Resources.LessThanOrEqual : "<=";
                case FilterOperator.GreaterThanOrEqual:
                    return isDescription ? Resources.GreaterThanOrEqual : ">=";
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
