using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.Data;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;
using Windows.Media.Playlists;
using Windows.Storage;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export, Export(typeof(IPlaylistService))]
    internal class PlaylistController : IPlaylistService
    {
        private readonly IFileDialogService fileDialogService;
        private readonly IShellService shellService;
        private readonly IEnvironmentService environmentService;
        private readonly IMusicFileContext musicFileContext;
        private readonly IPlayerService playerService;
        private readonly IMusicPropertiesService musicPropertiesService;
        private readonly Lazy<PlaylistViewModel> playlistViewModel;
        private readonly DelegateCommand playSelectedCommand;
        private readonly DelegateCommand removeSelectedCommand;
        private readonly DelegateCommand showMusicPropertiesCommand;
        private readonly DelegateCommand openListCommand;
        private readonly DelegateCommand saveListCommand;
        private readonly DelegateCommand clearListCommand;
        private readonly FileType openPlaylistFileType;
        private readonly FileType savePlaylistFileType;

        [ImportingConstructor]
        public PlaylistController(IFileDialogService fileDialogService, IShellService shellService, IEnvironmentService environmentService, 
            IMusicFileContext musicFileContext, IPlayerService playerService, IMusicPropertiesService musicPropertiesService, Lazy<PlaylistViewModel> playlistViewModel)
        {
            this.fileDialogService = fileDialogService;
            this.playlistViewModel = playlistViewModel;
            this.shellService = shellService;
            this.environmentService = environmentService;
            this.musicFileContext = musicFileContext;
            this.playerService = playerService;
            this.musicPropertiesService = musicPropertiesService;
            playSelectedCommand = new DelegateCommand(PlaySelected, CanPlaySelected);
            removeSelectedCommand = new DelegateCommand(RemoveSelected, CanRemoveSelected);
            showMusicPropertiesCommand = new DelegateCommand(ShowMusicProperties);
            openListCommand = new DelegateCommand(OpenList);
            saveListCommand = new DelegateCommand(SaveList);
            clearListCommand = new DelegateCommand(ClearList);
            openPlaylistFileType = new FileType(Resources.Playlist, SupportedFileTypes.PlaylistFileExtensions);
            savePlaylistFileType = new FileType(Resources.Playlist, SupportedFileTypes.PlaylistFileExtensions.First());
        }

        public PlaylistSettings PlaylistSettings { get; set; } 
        
        public PlaylistManager PlaylistManager { get; set; }

        private PlaylistViewModel PlaylistViewModel => playlistViewModel.Value;

        public void Initialize()
        {
            PlaylistViewModel.PlaylistManager = PlaylistManager;
            PlaylistViewModel.PlaySelectedCommand = playSelectedCommand;
            PlaylistViewModel.RemoveSelectedCommand = removeSelectedCommand;
            PlaylistViewModel.ShowMusicPropertiesCommand = showMusicPropertiesCommand;
            PlaylistViewModel.OpenListCommand = openListCommand;
            PlaylistViewModel.SaveListCommand = saveListCommand;
            PlaylistViewModel.ClearListCommand = clearListCommand;
            PlaylistViewModel.InsertFilesAction = InsertFiles;
            PlaylistViewModel.InsertMusicFilesAction = InsertMusicFiles;
            PlaylistViewModel.PropertyChanged += PlaylistViewModelPropertyChanged;

            shellService.PlaylistView = PlaylistViewModel.View;
        }

        public void Run()
        {
            IReadOnlyList<string> musicFilesToLoad;
            if (environmentService.MusicFilesToLoad.Any())
            {
                musicFilesToLoad = environmentService.MusicFilesToLoad;
            }
            else
            {
                musicFilesToLoad = PlaylistSettings.FileNames;
            }
            InsertFiles(0, musicFilesToLoad);
        }

        public void Shutdown()
        {
            PlaylistSettings.ReplaceAll(PlaylistManager.Items.Select(x => x.MusicFile.FileName));
        }

        public void TrySelectMusicFile(MusicFile musicFile)
        {
            var playlistItem = PlaylistManager.Items.FirstOrDefault(x => x.MusicFile == musicFile);
            if (playlistItem != null)
            {
                PlaylistViewModel.SelectedPlaylistItem = playlistItem;
                PlaylistViewModel.ScrollIntoView();
                PlaylistViewModel.FocusSelectedItem();
            }
        }

        private bool CanPlaySelected()
        {
            return PlaylistViewModel.SelectedPlaylistItem != null;
        }

        private void PlaySelected()
        {
            bool oldCanNextItem = PlaylistManager.CanNextItem;
            PlaylistManager.CurrentItem = PlaylistViewModel.SelectedPlaylistItem;
            playerService.Play();
            if (!oldCanNextItem)
            {
                // When playing in shuffle mode then reset the shuffle stack if the old item was the last one.
                PlaylistManager.Reset();
            }
        }
        
        private bool CanRemoveSelected()
        {
            return PlaylistViewModel.SelectedPlaylistItem != null;
        }

        private void RemoveSelected()
        {
            var playListItemsToExclude = PlaylistViewModel.SelectedPlaylistItems.Except(new[] { PlaylistViewModel.SelectedPlaylistItem }).ToArray();
            var nextPlaylistItem = PlaylistManager.Items.Except(playListItemsToExclude).GetNextElementOrDefault(PlaylistViewModel.SelectedPlaylistItem);

            PlaylistManager.RemoveItems(PlaylistViewModel.SelectedPlaylistItems);
            PlaylistViewModel.SelectedPlaylistItem = nextPlaylistItem ?? PlaylistManager.Items.LastOrDefault();
            PlaylistViewModel.FocusSelectedItem();
        }

        private void ShowMusicProperties()
        {
            musicPropertiesService.SelectMusicFiles(PlaylistViewModel.SelectedPlaylistItems.Select(x => x.MusicFile).ToArray());
            shellService.ShowMusicPropertiesView();
        }

        private void InsertFiles(int index, IEnumerable<string> fileNames)
        {
            Log.Default.Trace("PlaylistController.InsertFiles:Start");
            var musicFileNames = fileNames.Where(x => SupportedFileTypes.MusicFileExtensions.Contains(Path.GetExtension(x))).ToArray();
            InsertFilesCore(index, musicFileNames);

            Log.Default.Trace("PlaylistController.InsertFiles:OpenPlaylists");
            var playlistFileNames = fileNames.Where(x => SupportedFileTypes.PlaylistFileExtensions.Contains(Path.GetExtension(x))).ToArray();
            foreach (var playlistFileName in playlistFileNames)
            {
                OpenListCore(playlistFileName);
            }

            Log.Default.Trace("PlaylistController.InsertFiles:End");
        }
        
        private void InsertMusicFiles(int index, IEnumerable<MusicFile> musicFiles)
        {
            PlaylistManager.InsertItems(index, musicFiles.Select(x => new PlaylistItem(x)));
        }

        private void OpenList()
        {
            var result = fileDialogService.ShowOpenFileDialog(shellService.ShellView, openPlaylistFileType);
            if (!result.IsValid)
            {
                return;
            }
            OpenListCore(result.FileName);
        }

        private void OpenListCore(string playlistFileName)
        {
            Playlist playlist;
            try
            {
                var playlistFile = StorageFile.GetFileFromPathAsync(playlistFileName).GetResult();
                // MS Issue: LoadAsync cannot load a playlist when one of the files do not exists anymore.
                playlist = Playlist.LoadAsync(playlistFile).GetResult();
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "OpenListCore");
                shellService.ShowError(ex, Resources.CouldNotLoadPlaylist);
                return;
            }
            InsertFilesCore(PlaylistManager.Items.Count, playlist.Files.Select(x => x.Path).ToArray());
        }

        private void InsertFilesCore(int index, IEnumerable<string> fileNames)
        {
            try
            {
                var musicFiles = fileNames.Select(musicFileContext.Create).ToArray();
                InsertMusicFiles(index, musicFiles);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "PlaylistController.InsertFileCore");
                shellService.ShowError(ex, Resources.CouldNotOpenFiles);
                return;
            }
        }

        private void SaveList()
        {
            var result = fileDialogService.ShowSaveFileDialog(shellService.ShellView, savePlaylistFileType);
            if (!result.IsValid)
            {
                return;
            }

            var playlist = new Playlist();
            try
            {
                foreach (var item in PlaylistManager.Items)
                {
                    var file = StorageFile.GetFileFromPathAsync(item.MusicFile.FileName).GetResult();
                    playlist.Files.Add(file);
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "SaveList GetStorageFiles");
                shellService.ShowError(ex, Resources.CouldNotSavePlaylistBecauseMissingFiles);
                return;
            }
            
            try
            {
                var targetFolder = StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(result.FileName)).GetResult();
                var name = Path.GetFileNameWithoutExtension(result.FileName);
                playlist.SaveAsAsync(targetFolder, name, NameCollisionOption.ReplaceExisting, PlaylistFormat.M3u).Wait();
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "SaveList SaveAs");
                shellService.ShowError(ex, Resources.CouldNotSavePlaylist);
            }
        }

        private void ClearList()
        {
            PlaylistManager.ClearItems();
        }

        private void PlaylistViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlaylistViewModel.SelectedPlaylistItem))
            {
                UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            playSelectedCommand.RaiseCanExecuteChanged();
            removeSelectedCommand.RaiseCanExecuteChanged();
        }
    }
}
