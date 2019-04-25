using System.ComponentModel;

namespace Waf.MusicManager.Applications.Services
{
    public interface IManagerStatusService : INotifyPropertyChanged
    {
        bool UpdatingFilesList { get; }

        int TotalFilesCount { get; }
    }
}
