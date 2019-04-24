using NLog;

namespace Waf.MusicManager.Domain
{
    internal static class Log
    {
        public static Logger Default { get; } = LogManager.GetLogger("MusicManager.D");
    }
}
