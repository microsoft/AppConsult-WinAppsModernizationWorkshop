using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Waf.Foundation;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public class MusicFile : Model
    {
        private readonly TaskCompletionSource<MusicMetadata> loadMetadataCompletionSource;
        private readonly Func<string, Task<MusicMetadata>> loadMetadata;
        private IReadOnlyCollection<MusicFile> sharedMusicFiles;
        private MusicMetadata metadata;
        private bool loadCalled;
        private bool isMetadataLoaded;
        private Exception loadError;

        public MusicFile(Func<string, Task<MusicMetadata>> loadMetadata, string fileName)
        {
            loadMetadataCompletionSource = new TaskCompletionSource<MusicMetadata>();
            this.loadMetadata = loadMetadata;
            FileName = fileName;
            sharedMusicFiles = Array.Empty<MusicFile>();
        }

        public string FileName { get; }

        public IReadOnlyCollection<MusicFile> SharedMusicFiles
        {
            get => sharedMusicFiles;
            set => SetProperty(ref sharedMusicFiles, value);
        }

        public MusicMetadata Metadata
        {
            get
            {
                if (metadata == null)
                {
                    LoadMetadataCore();
                }
                return metadata;
            }
            private set => SetProperty(ref metadata, value);
        }

        public bool IsMetadataLoaded
        {
            get => isMetadataLoaded;
            private set => SetProperty(ref isMetadataLoaded, value);
        }

        public Exception LoadError
        {
            get => loadError;
            private set => SetProperty(ref loadError, value);
        }

        public Task<MusicMetadata> GetMetadataAsync()
        {
            LoadMetadataCore();
            return loadMetadataCompletionSource.Task;
        }

        private async void LoadMetadataCore()
        {
            if (loadCalled) { return; }
            loadCalled = true;
            try
            {
                var musicMetadata = await loadMetadata(FileName);
                if (musicMetadata == null) { throw new InvalidOperationException("The loadMetadata delegate must not return null."); }
                musicMetadata.Parent = this;
                musicMetadata.EntityLoadCompleted();
                Metadata = musicMetadata;
                IsMetadataLoaded = true;
                loadMetadataCompletionSource.SetResult(Metadata);
                if (!string.IsNullOrEmpty(FileName))
                {
                    Log.Default.Trace("MusicFile.MetadataLoaded: {0}", FileName);
                }
            }
            catch (Exception e)
            {
                Log.Default.Error(e, "LoadMetadataCore");
                LoadError = e;
                // Observe the exception
                loadMetadataCompletionSource.Task.NoWait(ignoreExceptions: true);
                loadMetadataCompletionSource.SetException(e);
            }
        }
    }
}
