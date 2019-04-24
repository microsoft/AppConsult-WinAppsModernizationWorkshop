using NLog;

namespace Waf.MusicManager.Applications
{
    internal static class Log
    {
        public static Logger Default { get; } = LogManager.GetLogger("MusicManager.A");
    }
}
