using System;
using System.Linq;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SamplePlaylistViewModel : PlaylistViewModel
    {
        public SamplePlaylistViewModel() : base(new MockPlaylistView())
        {
            var musicFiles = new[] 
            {
                new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 3, 45), 320000)
                {
                    Artists = new[] { @"Culture Beat" },
                    Title = @"Serenity (Epilog)",
                }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity"),
                new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 2, 2), 320000)
                {
                    Artists = new[] { "First artist", "Second artist" },
                    Title = "This track has a very long title. Let's see how the UI handles this.",
                }, ""),
                new SampleMusicFile(new MusicMetadata(new TimeSpan(1, 33, 0), 320000)
                {
                    Artists = new string[0],
                    Title = "",
                }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity"),
                new MusicFile(x => { throw new InvalidOperationException("Sample exception."); }, @"C:\corruptfile.mp3")
            };
            var playlistManager = new PlaylistManager();
            playlistManager.AddItems(musicFiles.Select(x => new PlaylistItem(x)));
            PlaylistManager = playlistManager;
            playlistManager.CurrentItem = playlistManager.Items.First();
        }
        
        
        private class MockPlaylistView : MockView, IPlaylistView
        {
            public void FocusSearchBox() { }
            
            public void FocusSelectedItem() { }

            public void ScrollIntoView(PlaylistItem item) { }
        }
    }
}
