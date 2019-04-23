using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Presentation.Views
{
    [Export(typeof(IShellView))]
    public partial class ShellWindow : IShellView
    {
        private readonly Lazy<ShellViewModel> viewModel;

        public ShellWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<ShellViewModel>(this.GetViewModel<ShellViewModel>);
            Loaded += LoadedHandler;

            // Workaround: Need to load both DrawingImages now; otherwise the first one is not shown at the beginning.
            playPauseButton.ImageSource = (ImageSource)FindResource("PlayButtonImage");
            playPauseButton.ImageSource = (ImageSource)FindResource("PauseButtonImage");
        }

        public double VirtualScreenWidth => SystemParameters.VirtualScreenWidth;

        public double VirtualScreenHeight => SystemParameters.VirtualScreenHeight;

        public bool IsMaximized
        {
            get => WindowState == WindowState.Maximized;
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }

        private ShellViewModel ViewModel => viewModel.Value;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.MediaPlayPause)
            {
                ViewModel.PlayerService.PlayPauseCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.MediaPreviousTrack)
            {
                ViewModel.PlayerService.PreviousCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.MediaNextTrack)
            {
                ViewModel.PlayerService.NextCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            ViewModel.ShellService.PropertyChanged += ShellServicePropertyChanged;
            ViewModel.PlayerService.PropertyChanged += PlayerServicePropertyChanged;
            UpdatePlayPauseButton();
        }

        private void PlayerServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IPlayerService.IsPlayCommand))
            {
                UpdatePlayPauseButton();
            }
        }

        private void UpdatePlayPauseButton()
        {
            if (ViewModel.PlayerService.IsPlayCommand)
            {
                playPauseButton.ImageSource = (ImageSource)FindResource("PlayButtonImage");
            }
            else
            {
                playPauseButton.ImageSource = (ImageSource)FindResource("PauseButtonImage");
            }
        }

        private void ShellServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IShellService.IsApplicationBusy))
            {
                if (ViewModel.ShellService.IsApplicationBusy)
                {        
                    Mouse.OverrideCursor = Cursors.Wait;    
                }
                else
                {
                    // Delay removing the wait cursor so that the UI has finished its work as well.
                    Dispatcher.InvokeAsync(() => Mouse.OverrideCursor = null, DispatcherPriority.ApplicationIdle);
                }
            }
        }
    }
}
