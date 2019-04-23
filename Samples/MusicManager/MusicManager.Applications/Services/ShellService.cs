using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : Model, IShellService
    {
        private readonly Lazy<IShellView> shellView;
        private readonly List<Task> tasksToCompleteBeforeShutdown;
        private readonly List<ApplicationBusyContext> applicationBusyContext;
        private object contentView;
        private object musicPropertiesView;
        private object playlistView;
        private Lazy<object> transcodeListView;
        private object playerView;
        private bool isApplicationBusy;
        private bool isClosingEventInitialized;
        private event CancelEventHandler closing;

        [ImportingConstructor]
        public ShellService(Lazy<IShellView> shellView)
	    {
            this.shellView = shellView;
            tasksToCompleteBeforeShutdown = new List<Task>();
            applicationBusyContext = new List<ApplicationBusyContext>();
	    }

        public AppSettings Settings { get; set; }
        
        public object ShellView => shellView.Value;

        public object ContentView
        {
            get => contentView;
            set => SetProperty(ref contentView, value);
        }

        public object MusicPropertiesView
        {
            get => musicPropertiesView;
            set => SetProperty(ref musicPropertiesView, value);
        }

        public object PlaylistView
        {
            get => playlistView;
            set => SetProperty(ref playlistView, value);
        }

        public Lazy<object> TranscodingListView
        {
            get => transcodeListView;
            set => SetProperty(ref transcodeListView, value);
        }

        public object PlayerView
        {
            get => playerView;
            set => SetProperty(ref playerView, value);
        }

        public Action<Exception, string> ShowErrorAction { get; set; }

        public Action ShowMusicPropertiesViewAction { get; set; }

        public Action ShowPlaylistViewAction { get; set; }

        public Action ShowTranscodingListViewAction { get; set; }

        public IReadOnlyCollection<Task> TasksToCompleteBeforeShutdown => tasksToCompleteBeforeShutdown;

        public bool IsApplicationBusy
        {
            get => isApplicationBusy;
            private set => SetProperty(ref isApplicationBusy, value);
        }

        public event CancelEventHandler Closing
        {
            add 
            { 
                closing += value;
                InitializeClosingEvent();
            }
            remove { closing -= value; }
        }

        public void ShowError(Exception exception, string displayMessage)
        {
            ShowErrorAction(exception, displayMessage);
        }
        
        public void ShowMusicPropertiesView()
        {
            ShowMusicPropertiesViewAction();
        }
        
        public void ShowPlaylistView()
        {
            ShowPlaylistViewAction();
        }

        public void ShowTranscodingListView()
        {
            ShowTranscodingListViewAction();
        }

        public void AddTaskToCompleteBeforeShutdown(Task task)
        {
            tasksToCompleteBeforeShutdown.Add(task);
        }

        public IDisposable SetApplicationBusy()
        {
            var context = new ApplicationBusyContext()
            {
                DisposeCallback = ApplicationBusyContextDisposeCallback
            };
            applicationBusyContext.Add(context);
            IsApplicationBusy = true;
            return context;
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            closing?.Invoke(this, e);
        }

        private void ApplicationBusyContextDisposeCallback(ApplicationBusyContext context)
        {
            applicationBusyContext.Remove(context);
            IsApplicationBusy = applicationBusyContext.Any();
        }

        private void InitializeClosingEvent()
        {
            if (isClosingEventInitialized) { return; }

            isClosingEventInitialized = true;
            shellView.Value.Closing += ShellViewClosing;
        }

        private void ShellViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }
    }
}
