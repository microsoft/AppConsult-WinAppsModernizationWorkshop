using System.Collections.Generic;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    internal interface IMusicPropertiesService
    {
        void SelectMusicFiles(IReadOnlyList<MusicFile> musicFiles);
    }
}
