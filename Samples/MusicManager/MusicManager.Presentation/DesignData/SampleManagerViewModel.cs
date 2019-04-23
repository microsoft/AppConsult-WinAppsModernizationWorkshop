using System;
using System.Linq;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SampleManagerViewModel : ManagerViewModel
    {
        public SampleManagerViewModel() : base(new MockManagerView(), new Lazy<ISelectionService>(() => new MockSelectionService()), null, null, null)
        {
            var musicFiles = new[] 
            {
                new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 3, 45), 320)
                {
                    Artists = new[] { @"Culture Beat" },
                    Title = @"Serenity (Epilog)",
                    
                }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.mp3"),
                new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 2, 2), 320)
                {
                    Artists = new[] { "First artist", "Second artist" },
                    Title = "",
                }, ""),
                new SampleMusicFile(new MusicMetadata(new TimeSpan(1, 33, 0), 320)
                {
                    Artists = new string[0],
                    Title = "",
                }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Mr. Vain.mp3")
            };
            ((MockSelectionService)SelectionService).SetMusicFiles(musicFiles.Select(x => new MusicFileDataModel(x)).ToArray());
        }


        private class MockManagerView : MockView, IManagerView
        {
        }
    }
}
