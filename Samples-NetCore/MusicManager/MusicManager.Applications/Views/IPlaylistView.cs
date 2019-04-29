using System.Waf.Applications;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.Views
{
    public interface IPlaylistView : IView
    {
        void FocusSearchBox();
        
        void FocusSelectedItem();
        
        void ScrollIntoView(PlaylistItem item);
    }
}
