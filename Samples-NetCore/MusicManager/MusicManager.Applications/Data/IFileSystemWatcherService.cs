using System.IO;

namespace Waf.MusicManager.Applications.Data
{
    public interface IFileSystemWatcherService
    {
        NotifyFilters NotifyFilter { get; set; }

        string Path { get; set; }
        
        bool EnableRaisingEvents { get; set; }

        event FileSystemEventHandler Created;

        event RenamedEventHandler Renamed;

        event FileSystemEventHandler Deleted;
    }
}
