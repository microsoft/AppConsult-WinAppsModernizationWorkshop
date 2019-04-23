using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    internal interface IPlaylistService
    {
        void TrySelectMusicFile(MusicFile musicFile);
    }
}
