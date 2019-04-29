using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Playlists
{
    public class PlaylistItem : Model
    {
        public PlaylistItem(MusicFile musicFile)
        {
            MusicFile = musicFile;
        }

        public MusicFile MusicFile { get; }
    }
}
