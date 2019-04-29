using System;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SampleMusicPropertiesViewModel : MusicPropertiesViewModel
    {
        public SampleMusicPropertiesViewModel() : base(new MockMusicPropertiesView(), null)
        {
            MusicFile = new SampleMusicFile(new MusicMetadata(new TimeSpan(0, 3, 45), 320000)
            {
                Artists = new[] { @"Culture Beat" },
                Title = @"Serenity (Epilog)",
                Genre = new[] { "Electronic", "Dance" }
            }, @"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity");
        }


        private class MockMusicPropertiesView : MockView, IMusicPropertiesView
        {
        }
    }
}
