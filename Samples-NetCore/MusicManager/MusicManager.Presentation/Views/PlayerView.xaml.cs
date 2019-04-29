using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IPlayerView))]
    public partial class PlayerView : IPlayerView
    {
        private readonly Lazy<PlayerViewModel> viewModel;
        private readonly PlayerService playerService;
        private readonly MediaPlayer mediaPlayer;
        private readonly DispatcherTimer updateTimer;
        private readonly ThrottledAction throttledSliderValueChangedAction;
        private readonly Converters.DurationConverter duratonConverter;
        private readonly DelegateCommand previousCommand;
        private readonly DelegateCommand playPauseCommand;
        private readonly DelegateCommand nextCommand;
        private bool suppressPositionSliderValueChanged;
        private double lastUserSliderValue;
        
        
        [ImportingConstructor]
        public PlayerView(PlayerService playerService)
        {
            InitializeComponent();
            viewModel = new Lazy<PlayerViewModel>(this.GetViewModel<PlayerViewModel>);
            this.playerService = playerService;
            mediaPlayer = new MediaPlayer();
            duratonConverter = new Converters.DurationConverter();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            updateTimer.Tick += UpdateTimerTick;

            throttledSliderValueChangedAction = new ThrottledAction(ThrottledSliderValueChanged, ThrottledActionMode.InvokeMaxEveryDelayTime, TimeSpan.FromMilliseconds(100));

            previousCommand = new DelegateCommand(Previous, CanPrevious);
            playPauseCommand = new DelegateCommand(PlayPause, CanPlayPause);
            nextCommand = new DelegateCommand(Next, CanNext);
            playerService.PreviousCommand = previousCommand;
            playerService.PlayPauseCommand = playPauseCommand;
            playerService.NextCommand = nextCommand;
            playerService.IsPlayCommand = true;
            
            Loaded += FirstTimeLoadedHandler;
        }

        private PlayerViewModel ViewModel => viewModel.Value;
        
        public TimeSpan GetPosition() { return mediaPlayer.Position; }

        public void SetPosition(TimeSpan position) 
        { 
            positionSlider.Value = position.TotalSeconds;
        }
        
        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {
            Loaded -= FirstTimeLoadedHandler;

            ViewModel.PropertyChanged += ViewModelPropertyChanged;
            ViewModel.PlaylistManager.PropertyChanged += PlaylistManagerPropertyChanged;
            ViewModel.PreviousTrackCommand.CanExecuteChanged += (sender2, e2) => previousCommand.RaiseCanExecuteChanged();
            ViewModel.NextTrackCommand.CanExecuteChanged += (sender2, e2) => nextCommand.RaiseCanExecuteChanged();

            mediaPlayer.MediaFailed += MediaPlayerMediaFailed;
            mediaPlayer.MediaEnded += MediaPlayerMediaEnded;
            mediaPlayer.Volume = ViewModel.Volume;
        }

        private async void OpenCurrentItem()
        {
            updateTimer.Stop();
            playerService.IsPlayCommand = true;
            
            if (ViewModel.PlaylistManager.CurrentItem != null)
            {
                var musicUri = new Uri(ViewModel.PlaylistManager.CurrentItem.MusicFile.FileName);
                if (mediaPlayer.Source != musicUri)
                {
                    mediaPlayer.Open(musicUri);
                    
                    positionSlider.Maximum = 1; // Use a default value that will be updated as soon the metadata is loaded.
                    positionSlider.Value = 0;

                    try
                    {
                        var metadata = await ViewModel.PlaylistManager.CurrentItem.MusicFile.GetMetadataAsync();
                        positionSlider.Maximum = metadata.Duration.TotalSeconds;
                    }
                    catch (Exception ex)
                    {
                        Log.Default.Error(ex, "PlayerView: Cannot read the metadata");
                    }
                }
            }
            else
            {
                mediaPlayer.Close();
            }
        }
        
        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlayerViewModel.Volume))
            {
                mediaPlayer.Volume = ViewModel.Volume;
            }
        }

        private bool CanPrevious()
        {
            return mediaPlayer.Position.TotalSeconds > 1 || ViewModel.PreviousTrackCommand.CanExecute(null);
        }

        private void Previous()
        {
            if (mediaPlayer.Position.TotalSeconds > 1)
            {
                positionSlider.Value = 0;
            }
            else
            {
                if (ViewModel.PreviousTrackCommand.CanExecute(null))
                {
                    ViewModel.PreviousTrackCommand.Execute(null);
                }
            }
        }

        private bool CanNext()
        {
            return ViewModel.NextTrackCommand.CanExecute(null);
        }

        private void Next()
        {
            if (ViewModel.NextTrackCommand.CanExecute(null))
            {
                ViewModel.NextTrackCommand.Execute(null);
            }
        }

        private void PlaylistManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlaylistManager.CurrentItem))
            {
                OpenCurrentItem();
                playPauseCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanPlayPause()
        {
            return ViewModel.PlaylistManager?.CurrentItem?.MusicFile != null;
        }
        
        private void PlayPause()
        {
            if (!updateTimer.IsEnabled)
            {
                PlayCore();
            }
            else
            {
                PauseCore();
            }
        }

        private void PlayCore()
        {
            mediaPlayer.Play();
            updateTimer.Start();
            playerService.IsPlayCommand = false;
        }

        private void PauseCore()
        {
            mediaPlayer.Pause();
            updateTimer.Stop();
            playerService.IsPlayCommand = true;
        }

        private void UpdateTimerTick(object sender, EventArgs e)
        {
            suppressPositionSliderValueChanged = true;
            if (!throttledSliderValueChangedAction.IsRunning)
            {
                positionSlider.Value = mediaPlayer.Position.TotalSeconds;
            }
            suppressPositionSliderValueChanged = false;

            previousCommand.RaiseCanExecuteChanged();
        }

        private void PositionSliderMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = (Slider)sender;
            double delta = (slider.Maximum - slider.Minimum) * 0.025 * e.Delta / 120d;
            slider.SetCurrentValue(Slider.ValueProperty, slider.Value + delta);
        }
        
        private void PositionSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            positionLabel.Text = (string)duratonConverter.Convert(TimeSpan.FromSeconds(e.NewValue), null, null, null);

            if (suppressPositionSliderValueChanged) { return; }

            lastUserSliderValue = e.NewValue;
            throttledSliderValueChangedAction.InvokeAccumulated();
        }

        private void ThrottledSliderValueChanged()
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(lastUserSliderValue);
        }

        private void MediaPlayerMediaFailed(object sender, ExceptionEventArgs e)
        {
            Log.Default.Warn(e.ErrorException, "MediaPlayer.MediaFailed");
            if (e.ErrorException is InvalidWmpVersionException)
            {
                ViewModel.ShellService.ShowError(e.ErrorException, Properties.Resources.NewerMediaPlayerRequired);
            }
            else
            {
                ViewModel.ShellService.ShowError(e.ErrorException, Properties.Resources.CouldNotPlayFile, ViewModel.PlaylistManager.CurrentItem.MusicFile.FileName);
            }
            PlayNextOrPause();
        }

        private void MediaPlayerMediaEnded(object sender, EventArgs e)
        {
            PlayNextOrPause();
        }

        private void PlayNextOrPause()
        {
            if (ViewModel.NextTrackCommand.CanExecute(null))
            {
                ViewModel.NextTrackCommand.Execute(null);
            }
            else
            {
                ViewModel.PlaylistManager.Reset();
                PauseCore();
                positionSlider.Value = 0;
            }
        }

        private void VolumeButtonClick(object sender, RoutedEventArgs e)
        {
            volumePopup.IsOpen = true;
        }

        private void ToggleMuteClick(object sender, RoutedEventArgs e)
        {
            mediaPlayer.IsMuted = !mediaPlayer.IsMuted;
            if (mediaPlayer.IsMuted)
            {
                volumeButton.Content = muteButton.Content = "\uE198";
            }
            else
            {
                volumeButton.Content = muteButton.Content = "\uE15D";
            }
        }

        private void VolumeSliderMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = (Slider)sender;
            double delta = (slider.Maximum - slider.Minimum) * 0.025 * e.Delta / 120d;
            slider.SetCurrentValue(Slider.ValueProperty, slider.Value + delta);
        }

        private void MoreButtonClick(object sender, RoutedEventArgs e)
        {
            morePopup.IsOpen = true;
        }
    }
}
