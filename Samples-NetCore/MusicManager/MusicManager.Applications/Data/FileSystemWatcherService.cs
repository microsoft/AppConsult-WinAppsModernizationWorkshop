using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Waf.MusicManager.Domain;

namespace Waf.MusicManager.Applications.Data
{
    [Export, Export(typeof(IFileSystemWatcherService)), PartMetadata(UnitTestMetadata.Name, UnitTestMetadata.Data)]
    internal class FileSystemWatcherService : Disposable, IFileSystemWatcherService
    {
        private readonly TaskScheduler taskScheduler;
        private readonly FileSystemWatcher watcher;

        [ImportingConstructor]
        public FileSystemWatcherService()
        {
            taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            watcher = new FileSystemWatcher();
            watcher.Created += WatcherCreated;
            watcher.Renamed += WatcherRenamed;
            watcher.Deleted += WatcherDeleted;
        }

        public NotifyFilters NotifyFilter
        {
            get => watcher.NotifyFilter;
            set => watcher.NotifyFilter = value;
        }

        public string Path
        {
            get => watcher.Path;
            set => watcher.Path = value;
        }

        public bool EnableRaisingEvents
        {
            get => watcher.EnableRaisingEvents;
            set => watcher.EnableRaisingEvents = value;
        }

        public event FileSystemEventHandler Created;

        public event RenamedEventHandler Renamed;

        public event FileSystemEventHandler Deleted;

        protected virtual void OnCreated(FileSystemEventArgs e)
        {
            Created?.Invoke(this, e);
        }

        protected virtual void OnRenamed(RenamedEventArgs e)
        {
            Renamed?.Invoke(this, e);
        }

        protected virtual void OnDeleted(FileSystemEventArgs e)
        {
            Deleted?.Invoke(this, e);
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            Log.Default.Trace(nameof(WatcherCreated));
            TaskHelper.Run(() => OnCreated(e), taskScheduler); 
        }

        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            Log.Default.Trace(nameof(WatcherRenamed));
            TaskHelper.Run(() => OnRenamed(e), taskScheduler); 
        }

        private void WatcherDeleted(object sender, FileSystemEventArgs e)
        {
            Log.Default.Trace(nameof(WatcherDeleted));
            TaskHelper.Run(() => OnDeleted(e), taskScheduler); 
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                watcher.Dispose();
            }
            base.Dispose(isDisposing);
        }
    }
}
