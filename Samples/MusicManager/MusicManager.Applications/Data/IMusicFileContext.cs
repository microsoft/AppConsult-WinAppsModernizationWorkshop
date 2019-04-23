using System.Collections.Generic;
using System.Threading.Tasks;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Data
{
    public interface IMusicFileContext
    {
        MusicFile Create(string fileName);

        MusicFile CreateFromMultiple(IEnumerable<MusicFile> musicFiles);

        void ApplyChanges(MusicFile musicFile);

        Task SaveChangesAsync(MusicFile musicFile);
    }
}
