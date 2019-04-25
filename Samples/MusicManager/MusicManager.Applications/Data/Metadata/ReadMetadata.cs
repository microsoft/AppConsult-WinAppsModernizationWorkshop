using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Waf.MusicManager.Domain.MusicFiles;
using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Applications.Data.Metadata
{
    internal abstract class ReadMetadata
    {
        protected virtual bool IsSupported => true;

        public async Task<MusicMetadata> CreateMusicMetadata(MusicProperties properties, CancellationToken cancellationToken)
        {
            var propertiesToRetrieve = new List<string>();
            AddPropertiesToRetrieve(propertiesToRetrieve);
            var customProperties = await properties.RetrievePropertiesAsync(propertiesToRetrieve).AsTask(cancellationToken).ConfigureAwait(false);
            
            TimeSpan duration = ReadDuration(properties, customProperties);
            uint bitrate = ReadBitrate(properties, customProperties);
            if (!IsSupported || (duration == TimeSpan.Zero && bitrate == 0))
            {
                return MusicMetadata.CreateUnsupported(duration, bitrate);
            }
            
            return new MusicMetadata(duration, bitrate)
            {
                Title = ReadTitle(properties, customProperties),
                Artists = ToSaveArray(ReadArtists(properties, customProperties)),
                Rating = ReadRating(properties, customProperties),
                Album = ReadAlbum(properties, customProperties),
                TrackNumber = ReadTrackNumber(properties, customProperties),
                Year = ReadYear(properties, customProperties),
                Genre = ToSaveArray(ReadGenre(properties, customProperties)),
                AlbumArtist = ReadAlbumArtist(properties, customProperties),
                Publisher = ReadPublisher(properties, customProperties),
                Subtitle = ReadSubtitle(properties, customProperties),
                Composers = ToSaveArray(ReadComposers(properties, customProperties)),
                Conductors = ToSaveArray(ReadConductors(properties, customProperties))
            };
        }

        protected virtual void AddPropertiesToRetrieve(IList<string> propertiesToRetrieve)
        {
            propertiesToRetrieve.Add(PropertyNames.Artist);
        }
        
        protected virtual TimeSpan ReadDuration(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Duration;
        }

        protected virtual uint ReadBitrate(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Bitrate;
        }

        protected virtual string ReadTitle(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Title;
        }

        protected virtual IEnumerable<string> ReadArtists(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return (IEnumerable<string>)customProperties[PropertyNames.Artist];
        }

        protected virtual uint ReadRating(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Rating;
        }

        protected virtual string ReadAlbum(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Album;
        }

        protected virtual uint ReadTrackNumber(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.TrackNumber;
        }

        protected virtual uint ReadYear(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Year;
        }

        protected virtual IEnumerable<string> ReadGenre(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Genre;
        }

        protected virtual string ReadAlbumArtist(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.AlbumArtist;
        }

        protected virtual string ReadPublisher(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Publisher;
        }

        protected virtual string ReadSubtitle(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Subtitle;
        }

        protected virtual IEnumerable<string> ReadComposers(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Composers;
        }

        protected virtual IEnumerable<string> ReadConductors(MusicProperties properties, IDictionary<string, object> customProperties)
        {
            return properties.Conductors;
        }

        private static IReadOnlyList<T> ToSaveArray<T>(IEnumerable<T> collection)
        {
            return collection?.ToArray() ?? Array.Empty<T>();
        }
    }
}
