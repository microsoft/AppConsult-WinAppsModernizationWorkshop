using System;
using System.Linq;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Transcoding;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SampleTranscodingListViewModel : TranscodingListViewModel
    {
        public SampleTranscodingListViewModel() : base(new MockTranscodingListView(), null)
        {
            var musicFiles = new[] 
            {
                new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 3, 45), 320000)
                {
                    Artists = new[] { @"Culture Beat" },
                    Title = @"Serenity (Epilog)",
                }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.waf"),
                new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 2, 2), 320000)
                {
                    Artists = new[] { "First artist", "Second artist" },
                    Title = "This track has a very long title. Let's see how the UI handles this.",
                }, @"C:\Users\Public\Music\test.m4a"),
                new SampleMusicFile(new MusicMetadata(new TimeSpan(1, 33, 0), 320000)
                {
                    Artists = new string[0],
                    Title = "",
                }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp4"),
            };
            var transcodingManager = new TranscodingManager();
            musicFiles.Select(x => new TranscodeItem(x, x.FileName + ".mp3")).ToList().ForEach(transcodingManager.AddTranscodeItem);
            transcodingManager.TranscodeItems[0].Progress = 1;
            transcodingManager.TranscodeItems[1].Progress = 0.27;
            transcodingManager.TranscodeItems[2].Error = new InvalidOperationException("Test");
            TranscodingManager = transcodingManager;

            SelectedTranscodeItems.Add(transcodingManager.TranscodeItems.Last());
        }


        private class MockTranscodingListView : MockView, ITranscodingListView
        {
        }
    }
}
