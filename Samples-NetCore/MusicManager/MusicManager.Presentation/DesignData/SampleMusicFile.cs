using System.Threading.Tasks;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Presentation.DesignData
{
    public class SampleMusicFile : MusicFile
    {
        public SampleMusicFile(MusicMetadata metadata, string fileName) : base(x => Task.FromResult(metadata), fileName)
        {
        }
    }
}
