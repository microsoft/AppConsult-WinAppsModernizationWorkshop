using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export]
    internal class PlayerController
    {
        private readonly IShellService shellService;
        private readonly ISelectionService selectionService;
        private readonly IEnvironmentService environmentService;
        private readonly PlayerService playerService;
        private readonly IMusicPropertiesService musicPropertiesService;
        private readonly IPlaylistService playlistService;
        private readonly Lazy<PlayerViewModel> playerViewModel;
        private readonly ExportFactory<InfoViewModel> infoViewModelFactory;
        private readonly DelegateCommand playAllCommand;
        private readonly DelegateCommand playSelectedCommand;
        private readonly DelegateCommand enqueueAllCommand;
        private readonly DelegateCommand enqueueSelectedCommand;
        private readonly DelegateCommand previousTrackCommand;
        private readonly DelegateCommand nextTrackCommand;
        private readonly DelegateCommand infoCommand;
        private readonly DelegateCommand showMusicPropertiesCommand;
        private readonly DelegateCommand showPlaylistCommand;
        
        [ImportingConstructor]
        public PlayerController(IShellService shellService, IEnvironmentService environmentService, ISelectionService selectionService, PlayerService playerService,
            IMusicPropertiesService musicPropertiesService, IPlaylistService playlistService, Lazy<PlayerViewModel> playerViewModel, ExportFactory<InfoViewModel> infoViewModelFactory)
        {
            this.shellService = shellService;
            this.environmentService = environmentService;
            this.selectionService = selectionService;
            this.playerService = playerService;
            this.musicPropertiesService = musicPropertiesService;
            this.playlistService = playlistService;
            this.playerViewModel = playerViewModel;
            this.infoViewModelFactory = infoViewModelFactory;
            playAllCommand = new DelegateCommand(PlayAll, CanPlayAll);
            playSelectedCommand = new DelegateCommand(PlaySelected, CanPlaySelected);
            enqueueAllCommand = new DelegateCommand(EnqueueAll, CanEnqueueAll);
            enqueueSelectedCommand = new DelegateCommand(EnqueueSelected, CanEnqueueSelected);
            previousTrackCommand = new DelegateCommand(PreviousTrack, CanPreviousTrack);
            nextTrackCommand = new DelegateCommand(NextTrack, CanNextTrack);
            infoCommand = new DelegateCommand(ShowInfo);
            showMusicPropertiesCommand = new DelegateCommand(ShowMusicProperties);
            showPlaylistCommand = new DelegateCommand(ShowPlaylist);
        }

        public PlaylistManager PlaylistManager { get; set; }

        public PlaylistSettings PlaylistSettings { get; set; }
        
        private PlayerViewModel PlayerViewModel => playerViewModel.Value;

        public void Initialize()
        {
            playerService.PlayAllCommand = playAllCommand;
            playerService.PlaySelectedCommand = playSelectedCommand;
            playerService.EnqueueAllCommand = enqueueAllCommand;
            playerService.EnqueueSelectedCommand = enqueueSelectedCommand;
            playerService.PlayingMusicFile = PlaylistManager.CurrentItem?.MusicFile;
            
            PlaylistManager.Shuffle = shellService.Settings.Shuffle;
            PlaylistManager.Repeat = shellService.Settings.Repeat;
            PlaylistManager.PropertyChanged += PlaylistManagerPropertyChanged;

            PlayerViewModel.Volume = shellService.Settings.Volume;
            PlayerViewModel.PlaylistManager = PlaylistManager;
            PlayerViewModel.PreviousTrackCommand = previousTrackCommand;
            PlayerViewModel.NextTrackCommand = nextTrackCommand;
            PlayerViewModel.InfoCommand = infoCommand;
            PlayerViewModel.ShowMusicPropertiesCommand = showMusicPropertiesCommand;
            PlayerViewModel.ShowPlaylistCommand = showPlaylistCommand;
            
            shellService.PlayerView = PlayerViewModel.View;

            selectionService.MusicFiles.CollectionChanged += MusicFilesCollectionChanged;
            ((INotifyCollectionChanged)selectionService.SelectedMusicFiles).CollectionChanged += SelectedMusicFilesCollectionChanged;
        }

        public void Run()
        {
            if (environmentService.MusicFilesToLoad.Any())
            {
                var item = PlaylistManager.Items.FirstOrDefault(x => x.MusicFile.FileName == environmentService.MusicFilesToLoad.First());
                if (item != null)
                {
                    PlaylistManager.CurrentItem = item;
                    playerService.Play();
                }
            }
            else if (!string.IsNullOrEmpty(PlaylistSettings.LastPlayedFileName))
            {
                var item = PlaylistManager.Items.FirstOrDefault(x => x.MusicFile.FileName == PlaylistSettings.LastPlayedFileName);
                if (item != null)
                {
                    PlaylistManager.CurrentItem = item;
                    PlayerViewModel.SetPosition(PlaylistSettings.LastPlayedFilePosition);
                }
            }
        }

        public void Shutdown()
        {
            PlaylistSettings.LastPlayedFileName = PlaylistManager.CurrentItem?.MusicFile.FileName;
            PlaylistSettings.LastPlayedFilePosition = PlayerViewModel.GetPosition();
            
            // Stops playing the current music file.
            PlaylistManager.CurrentItem = null;

            shellService.Settings.Volume = PlayerViewModel.Volume;
            shellService.Settings.Shuffle = PlaylistManager.Shuffle;
            shellService.Settings.Repeat = PlaylistManager.Repeat;
        }
        
        private bool CanPlayAll()
        {
            return selectionService.MusicFiles.Any();
        }

        private void PlayAll()
        {
            Play(selectionService.MusicFiles.Select(x => x.MusicFile));
        }

        private bool CanPlaySelected()
        {
            return selectionService.SelectedMusicFiles.Any();
        }

        private void PlaySelected()
        {
            Play(selectionService.SelectedMusicFiles.Select(x => x.MusicFile));
        }

        private bool CanEnqueueAll()
        {
            return selectionService.MusicFiles.Any();
        }

        private void EnqueueAll()
        {
            Enqueue(selectionService.MusicFiles.Select(x => x.MusicFile));
        }

        private bool CanEnqueueSelected()
        {
            return selectionService.SelectedMusicFiles.Any();
        }

        private void EnqueueSelected()
        {
            Enqueue(selectionService.SelectedMusicFiles.Select(x => x.MusicFile));
        }

        private void Play(IEnumerable<MusicFile> musicFiles)
        {
            var playlistItems = musicFiles.Select(x => new PlaylistItem(x)).ToArray();
            PlaylistManager.AddAndReplaceItems(playlistItems);
            PlaylistManager.CurrentItem = playlistItems.First();
            playerService.Play();
            shellService.ShowPlaylistView();
        }

        private void Enqueue(IEnumerable<MusicFile> musicFiles)
        {
            PlaylistManager.AddItems(musicFiles.Select(x => new PlaylistItem(x)));
            shellService.ShowPlaylistView();
        }

        private bool CanPreviousTrack()
        {
            return PlaylistManager.CanPreviousItem;
        }

        private void PreviousTrack()
        {
            var wasPlaying = !playerService.IsPlayCommand;
            PlaylistManager.PreviousItem();
            if (wasPlaying) { playerService.Play(); }
        }

        private bool CanNextTrack()
        {
            return PlaylistManager.CanNextItem;
        }

        private void NextTrack()
        {
            var wasPlaying = !playerService.IsPlayCommand;
            PlaylistManager.NextItem();
            if (wasPlaying) { playerService.Play(); }
        }

        private void ShowInfo()
        {
            var infoViewModel = infoViewModelFactory.CreateExport().Value;
            infoViewModel.ShowDialog(shellService.ShellView);
        }

        private void ShowMusicProperties()
        {
            musicPropertiesService.SelectMusicFiles(new[] { PlaylistManager.CurrentItem?.MusicFile }.Where(x => x != null).ToArray());
            shellService.ShowMusicPropertiesView();
        }

        private void ShowPlaylist()
        {
            playlistService.TrySelectMusicFile(PlaylistManager.CurrentItem?.MusicFile);
            shellService.ShowPlaylistView();
        }

        private void PlaylistManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlaylistManager.CurrentItem))
            {
                playerService.PlayingMusicFile = PlaylistManager.CurrentItem?.MusicFile;
            }
            else if (new[] { nameof(PlaylistManager.CanPreviousItem), nameof(PlaylistManager.CanNextItem) }.Contains(e.PropertyName))
            {
                UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            previousTrackCommand.RaiseCanExecuteChanged();
            nextTrackCommand.RaiseCanExecuteChanged();
        }

        private void MusicFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            playAllCommand.RaiseCanExecuteChanged();
            enqueueAllCommand.RaiseCanExecuteChanged();
        }

        private void SelectedMusicFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            playSelectedCommand.RaiseCanExecuteChanged();
            enqueueSelectedCommand.RaiseCanExecuteChanged();
        }
    }
}
