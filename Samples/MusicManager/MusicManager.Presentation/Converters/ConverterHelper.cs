using System;

namespace Waf.MusicManager.Presentation.Converters
{
    internal static class ConverterHelper
    {
        public static bool IsParameterSet(string expectedParameter, object actualParameter)
        {
            return string.Equals(actualParameter as string, expectedParameter, StringComparison.OrdinalIgnoreCase);
        }
    }
}
