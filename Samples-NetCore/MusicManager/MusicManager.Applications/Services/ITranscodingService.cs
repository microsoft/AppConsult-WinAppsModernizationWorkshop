using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Waf.MusicManager.Applications.Services
{
    public interface ITranscodingService : INotifyPropertyChanged
    {
        ICommand ConvertToMp3AllCommand { get; }

        ICommand ConvertToMp3SelectedCommand { get; }

        ICommand CancelAllCommand { get; }
        
        ICommand CancelSelectedCommand { get; }

        event EventHandler<TranscodingTaskEventArgs> TranscodingTaskCreated;
    }
}
