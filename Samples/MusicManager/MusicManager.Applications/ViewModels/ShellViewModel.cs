using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class ShellViewModel : ViewModel<IShellView>
    {
        private readonly AppSettings settings;
        private readonly ObservableCollection<Tuple<Exception, string>> errors;
        private object detailsView;
        
        [ImportingConstructor]
        public ShellViewModel(IShellView view, IShellService shellService, IPlayerService playerService)
            : base(view)
        {
            ShellService = shellService;
            PlayerService = playerService;
            settings = shellService.Settings;
            errors = new ObservableCollection<Tuple<Exception, string>>();
            ExitCommand = new DelegateCommand(Close);
            CloseErrorCommand = new DelegateCommand(CloseError);
            GarbageCollectorCommand = new DelegateCommand(GC.Collect);

            errors.CollectionChanged += ErrorsCollectionChanged;
            view.Closed += ViewClosed;

            // Restore the window size when the values are valid.
            if (settings.Left >= 0 && settings.Top >= 0 && settings.Width > 0 && settings.Height > 0
                && settings.Left + settings.Width <= view.VirtualScreenWidth
                && settings.Top + settings.Height <= view.VirtualScreenHeight)
            {
                view.Left = settings.Left;
                view.Top = settings.Top;
                view.Height = settings.Height;
                view.Width = settings.Width;
            }
            view.IsMaximized = settings.IsMaximized;
        }

        public string Title => ApplicationInfo.ProductName;

        public IShellService ShellService { get; }

        public IPlayerService PlayerService { get; }

        public IReadOnlyList<Tuple<Exception, string>> Errors => errors;

        public Tuple<Exception, string> LastError => errors.LastOrDefault();

        public ICommand ExitCommand { get; }

        public ICommand CloseErrorCommand { get; }

        public ICommand GarbageCollectorCommand { get; }

        public object DetailsView
        {
            get => detailsView;
            private set => SetProperty(ref detailsView, value);
        }

        public bool IsMusicPropertiesViewVisible
        {
            get => DetailsView == ShellService.MusicPropertiesView;
            set { if (value) { DetailsView = ShellService.MusicPropertiesView; } }
        }

        public bool IsPlaylistViewVisible
        {
            get => DetailsView == ShellService.PlaylistView;
            set { if (value) { DetailsView = ShellService.PlaylistView; } }
        }

        public bool IsTranscodingListViewVisible
        {
            get => ShellService.TranscodingListView.IsValueCreated && DetailsView == ShellService.TranscodingListView.Value;
            set { if (value) { DetailsView = ShellService.TranscodingListView.Value; } }
        }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        public void ShowError(Exception exception, string message)
        {
            errors.Add(Tuple.Create(exception, message));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(DetailsView))
            {
                RaisePropertyChanged(nameof(IsMusicPropertiesViewVisible));
                RaisePropertyChanged(nameof(IsPlaylistViewVisible));
                RaisePropertyChanged(nameof(IsTranscodingListViewVisible));
            }
        }

        private void CloseError()
        {
            if (errors.Any())
            {
                errors.RemoveAt(errors.Count - 1);
            }
        }

        private void ErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(LastError));
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            settings.Left = ViewCore.Left;
            settings.Top = ViewCore.Top;
            settings.Height = ViewCore.Height;
            settings.Width = ViewCore.Width;
            settings.IsMaximized = ViewCore.IsMaximized;
        }
    }
}
