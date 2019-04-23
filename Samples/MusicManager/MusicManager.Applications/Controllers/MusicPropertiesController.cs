using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export, Export(typeof(IMusicPropertiesService))]
    internal class MusicPropertiesController : IMusicPropertiesService
    {
        private readonly IShellService shellService;
        private readonly IMusicFileContext musicFileContext;
        private readonly ISelectionService selectionService;
        private readonly Lazy<MusicPropertiesViewModel> musicPropertiesViewModel;
        private readonly ChangeTrackerService changeTrackerService;
        private readonly HashSet<MusicFile> musicFilesToSaveAfterPlaying;
        private TaskCompletionSource<object> allFilesSavedCompletion;
        
        [ImportingConstructor]
        public MusicPropertiesController(IShellService shellService, IMusicFileContext musicFileContext, ISelectionService selectionService, Lazy<MusicPropertiesViewModel> musicPropertiesViewModel)
        {
            this.shellService = shellService;
            this.musicFileContext = musicFileContext;
            this.selectionService = selectionService;
            this.musicPropertiesViewModel = musicPropertiesViewModel;
            changeTrackerService = new ChangeTrackerService();
            musicFilesToSaveAfterPlaying = new HashSet<MusicFile>();
        }

        public PlaylistManager PlaylistManager { get; set; }

        private MusicPropertiesViewModel MusicPropertiesViewModel => musicPropertiesViewModel.Value;

        public void Initialize()
        {
            ServiceLocator.RegisterInstance<IChangeTrackerService>(changeTrackerService);

            PlaylistManager.PropertyChanged += PlaylistManagerPropertyChanged;
            ((INotifyCollectionChanged)selectionService.SelectedMusicFiles).CollectionChanged += SelectedMusicFilesCollectionChanged;
            shellService.MusicPropertiesView = MusicPropertiesViewModel.View;
        }

        public void Shutdown()
        {
            var task = SaveDirtyFilesAsync();
            shellService.AddTaskToCompleteBeforeShutdown(task);

            if (musicFilesToSaveAfterPlaying.Any())
            {
                allFilesSavedCompletion = new TaskCompletionSource<object>();
                shellService.AddTaskToCompleteBeforeShutdown(allFilesSavedCompletion.Task);
            }
        }

        public void SelectMusicFiles(IReadOnlyList<MusicFile> musicFiles)
        {
            SaveDirtyFilesAsync().NoWait();
            
            if (musicFiles.Count <= 1)
            {
                MusicPropertiesViewModel.MusicFile = musicFiles.FirstOrDefault();
            }
            else
            {
                MusicPropertiesViewModel.MusicFile = musicFileContext.CreateFromMultiple(musicFiles);
            }
        }

        private async Task SaveDirtyFilesAsync()
        {
            musicFileContext.ApplyChanges(MusicPropertiesViewModel.MusicFile);

            var tasks = changeTrackerService.GetEntitiesWithChanges().Cast<MusicMetadata>().Select(x => SaveChangesAsync((MusicFile)x.Parent));
            await Task.WhenAll(tasks);
        }

        private async Task SaveMusicFilesToSaveAfterPlayingAsync()
        {
            var tasks = musicFilesToSaveAfterPlaying.ToArray().Select(x => SaveChangesAsync(x));
            await Task.WhenAll(tasks);
        }

        private async void PlaylistManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlaylistManager.CurrentItem))
            {
                await SaveMusicFilesToSaveAfterPlayingAsync();
            }
        }

        private async Task SaveChangesAsync(MusicFile musicFile)
        {
            if (musicFile == null)
            {
                return;
            }
            IReadOnlyCollection<MusicFile> allFilesToSave;
            if (musicFile.SharedMusicFiles.Any())
            {
                allFilesToSave = musicFile.SharedMusicFiles;
            }
            else
            {
                allFilesToSave = new[] { musicFile };
            }

            // Filter out the music file that is currently playing
            var playingMusicFile = PlaylistManager.CurrentItem?.MusicFile;
            var filesToSave = allFilesToSave.Except(new[] { playingMusicFile }).ToArray();
            foreach (var x in allFilesToSave.Intersect(new[] { playingMusicFile })) { musicFilesToSaveAfterPlaying.Add(x); }
            
            if (!filesToSave.Any())
            {
                return;
            }
            var tasks = filesToSave.Select(x => SaveChangesCoreAsync(x)).ToArray();

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "SaveChangesAsync");
                if (filesToSave.Count() == 1)
                {
                    shellService.ShowError(ex, Resources.CouldNotSaveFile, filesToSave.First().FileName);
                }
                else
                {
                    shellService.ShowError(ex, Resources.CouldNotSaveFiles);
                }
            }
            finally
            {
                RemoveMusicFilesToSaveAfterPlaying(filesToSave);
            }
        }

        private async Task SaveChangesCoreAsync(MusicFile musicFile)
        {
            try
            {
                changeTrackerService.RemoveEntity(musicFile.Metadata);
                await musicFileContext.SaveChangesAsync(musicFile);
            }
            catch (Exception)
            {
                changeTrackerService.EntityHasChanges(musicFile.Metadata);
                throw;
            }
        }

        private void RemoveMusicFilesToSaveAfterPlaying(IEnumerable<MusicFile> musicFiles)
        {
            foreach (var x in musicFiles) { musicFilesToSaveAfterPlaying.Remove(x); }
            
            if (allFilesSavedCompletion != null && !musicFilesToSaveAfterPlaying.Any())
            {
                allFilesSavedCompletion.TrySetResult(null);
            }
        }

        private void SelectedMusicFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectMusicFiles(selectionService.SelectedMusicFiles.Select(x => x.MusicFile).ToArray());
        }
    }
}
