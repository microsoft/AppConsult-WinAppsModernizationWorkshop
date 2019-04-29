using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Waf.MusicManager.Applications.Properties;

namespace Waf.MusicManager.Applications.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        AppSettings Settings { get; }
        
        object ShellView { get; }

        object ContentView { get; set; }

        object MusicPropertiesView { get; set; }

        object PlaylistView { get; set; }

        Lazy<object> TranscodingListView { get; set; }

        object PlayerView { get; set; }

        IReadOnlyCollection<Task> TasksToCompleteBeforeShutdown { get; }

        bool IsApplicationBusy { get; }

        event CancelEventHandler Closing;

        void ShowError(Exception exception, string displayMessage);
        
        void ShowMusicPropertiesView();

        void ShowPlaylistView();

        void ShowTranscodingListView();

        void AddTaskToCompleteBeforeShutdown(Task task);

        IDisposable SetApplicationBusy();
    }
}
