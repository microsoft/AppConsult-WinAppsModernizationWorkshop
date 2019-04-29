using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Waf.Foundation;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class MockShellService : Model, IShellService
    {
        public MockShellService()
        {
            Settings = new AppSettings();
        }
        
        public AppSettings Settings { get; set; }
        
        public object ShellView { get; set; }
        
        public object ContentView { get; set; }
        
        public object MusicPropertiesView { get; set; }
        
        public object PlaylistView { get; set; }

        public Lazy<object> TranscodingListView { get; set; }

        public object PlayerView { get; set; }

        public IReadOnlyCollection<Task> TasksToCompleteBeforeShutdown { get; set; }
        
        public bool IsApplicationBusy { get; set; }

        public event CancelEventHandler Closing;

        public void ShowError(Exception exception, string displayMessage)
        {
        }

        public void ShowMusicPropertiesView()
        {
        }

        public void ShowPlaylistView()
        {
        }

        public void ShowTranscodingListView()
        {
        }

        public void AddTaskToCompleteBeforeShutdown(Task task)
        {
        }

        public IDisposable SetApplicationBusy()
        {
            return null;
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            Closing?.Invoke(this, e);
        }
    }
}
