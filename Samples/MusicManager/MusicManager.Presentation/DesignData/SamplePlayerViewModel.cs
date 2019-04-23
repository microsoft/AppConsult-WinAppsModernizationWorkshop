using System;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SamplePlayerViewModel : PlayerViewModel
    {
        public SamplePlayerViewModel() : base(new MockPlayerView(), null, null)
        {
            var playlistManager = new PlaylistManager();
            playlistManager.CurrentItem = new PlaylistItem(new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 3, 45), 320)
            {
                Artists = new[] { @"Culture Beat" },
                Title = @"Serenity (Epilog)",
                Genre = new[] { "Electronic", "Dance" }
            }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity"));
            PlaylistManager = playlistManager;
        }


        private class MockPlayerView : MockView, IPlayerView
        {
            public TimeSpan GetPosition() { return new TimeSpan(0, 3, 33); }

            public void SetPosition(TimeSpan position) { }
        }
    }
}
