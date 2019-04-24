using System.Collections.Generic;
using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Applications.Data.Metadata
{
    internal class Mp4SaveMetadata : SaveMetadata
    {
        protected override void ApplyGenre(MusicProperties properties, IDictionary<string, object> customProperties, IEnumerable<string> genre)
        {
            ApplyAsOneItem(properties.Genre, genre);
        }
        
        private static void ApplyAsOneItem(IList<string> target, IEnumerable<string> source)
        {
            // The WinRT API does not support some of the multiple tags for MP4 files
            target.Clear();
            target.Add(StringListConverter.ToString(source));
        }
    }
}
