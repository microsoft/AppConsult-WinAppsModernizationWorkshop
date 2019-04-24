using System.ComponentModel;
using System.Windows.Input;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    public interface IPlayerService : INotifyPropertyChanged
    {
        ICommand PlayAllCommand { get; }
        
        ICommand PlaySelectedCommand { get; }
        
        ICommand EnqueueAllCommand { get; }
        
        ICommand EnqueueSelectedCommand { get; }
        
        ICommand PreviousCommand { get; }

        ICommand PlayPauseCommand { get; }

        ICommand NextCommand { get; }

        bool IsPlayCommand { get; }

        MusicFile PlayingMusicFile { get; }
    }
}
