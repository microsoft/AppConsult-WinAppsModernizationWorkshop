using System.Collections.Generic;

namespace Waf.MusicManager.Applications.Services
{
    public interface IEnvironmentService
    {
        IReadOnlyList<string> MusicFilesToLoad { get; }
        
        string MusicPath { get; }

        string PublicMusicPath { get; }
    }
}
