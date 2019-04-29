using NLog;

namespace Waf.MusicManager.Presentation
{
    internal static class Log
    {
        public static Logger Default { get; } = LogManager.GetLogger("MusicManager.P");

        public static Logger App { get; } = LogManager.GetLogger("App");
    }
}
