using System.Collections.Generic;
using System.Linq;

namespace Waf.MusicManager.Domain.Playlists
{
    internal static class StatisticsHelper
    {
        public static double TruncatedMean(IEnumerable<double> values, double truncateRate)
        {
            var count = values.Count();
            if (count < 1) return 0;

            int truncateCount = (int)(count * truncateRate);
            var truncatedValues = values.OrderBy(x => x).Skip(truncateCount).Take(count - 2 * truncateCount);
            double truncatedMean = truncatedValues.Average();
            return truncatedMean;
        }
    }
}
