using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.MusicFiles;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Applications.Data.Metadata
{
    internal abstract class SaveMetadata
    {
        protected virtual bool IsSupported => true;
        
        public async Task SaveChangesAsync(MusicFile musicFile)
        {
            if (!IsSupported || !musicFile.IsMetadataLoaded || !musicFile.Metadata.HasChanges)
            {
                return;
            }
            // Manipulating changes must be done synchronously; no await call is allowed before ClearChanges().
            var metadata = musicFile.Metadata;
            var changedProperties = metadata.GetChanges();
            metadata.ClearChanges();

            var file = await StorageFile.GetFileFromPathAsync(musicFile.FileName);
            var musicProperties = await file.Properties.GetMusicPropertiesAsync();

            var customProperties = new Dictionary<string, object>();
            if (changedProperties.Contains(nameof(MusicMetadata.Title))) { ApplyTitle(musicProperties, customProperties, metadata.Title); }
            if (changedProperties.Contains(nameof(MusicMetadata.Artists))) { ApplyArtists(musicProperties, customProperties, metadata.Artists); }
            if (changedProperties.Contains(nameof(MusicMetadata.Rating))) { ApplyRating(musicProperties, customProperties, metadata.Rating); }
            if (changedProperties.Contains(nameof(MusicMetadata.Album))) { ApplyAlbum(musicProperties, customProperties, metadata.Album); }
            if (changedProperties.Contains(nameof(MusicMetadata.TrackNumber))) { ApplyTrackNumber(musicProperties, customProperties, metadata.TrackNumber); }
            if (changedProperties.Contains(nameof(MusicMetadata.Year))) { ApplyYear(musicProperties, customProperties, metadata.Year); }
            if (changedProperties.Contains(nameof(MusicMetadata.Genre))) { ApplyGenre(musicProperties, customProperties, metadata.Genre); }
            if (changedProperties.Contains(nameof(MusicMetadata.AlbumArtist))) { ApplyAlbumArtist(musicProperties, customProperties, metadata.AlbumArtist); }
            if (changedProperties.Contains(nameof(MusicMetadata.Publisher))) { ApplyPublisher(musicProperties, customProperties, metadata.Publisher); }
            if (changedProperties.Contains(nameof(MusicMetadata.Subtitle))) { ApplySubtitle(musicProperties, customProperties, metadata.Subtitle); }
            if (changedProperties.Contains(nameof(MusicMetadata.Composers))) { ApplyComposers(musicProperties, customProperties, metadata.Composers); }
            if (changedProperties.Contains(nameof(MusicMetadata.Conductors))) { ApplyConductors(musicProperties, customProperties, metadata.Conductors); }

            Log.Default.Trace("SaveMetadata.SaveChangesAsync:Save: {0}", musicFile.FileName);
            await musicProperties.SavePropertiesAsync(customProperties);
        }

        protected virtual void ApplyTitle(MusicProperties properties, IDictionary<string, object> customProperties, string title)
        {
            properties.Title = title;
        }

        protected virtual void ApplyArtists(MusicProperties properties, IDictionary<string, object> customProperties, IEnumerable<string> artists)
        {
            customProperties.Add(PropertyNames.Artist, artists);
        }

        protected virtual void ApplyRating(MusicProperties properties, IDictionary<string, object> customProperties, uint rating)
        {
            properties.Rating = rating;
        }

        protected virtual void ApplyAlbum(MusicProperties properties, IDictionary<string, object> customProperties, string album)
        {
            properties.Album = album;
        }

        protected virtual void ApplyTrackNumber(MusicProperties properties, IDictionary<string, object> customProperties, uint trackNumber)
        {
            properties.TrackNumber = trackNumber;
        }

        protected virtual void ApplyYear(MusicProperties properties, IDictionary<string, object> customProperties, uint year)
        {
            properties.Year = year;
        }

        protected virtual void ApplyGenre(MusicProperties properties, IDictionary<string, object> customProperties, IEnumerable<string> genre)
        {
            ApplyList(properties.Genre, genre);
        }

        protected virtual void ApplyAlbumArtist(MusicProperties properties, IDictionary<string, object> customProperties, string albumArtist)
        {
            properties.AlbumArtist = albumArtist;
        }

        protected virtual void ApplyPublisher(MusicProperties properties, IDictionary<string, object> customProperties, string publisher)
        {
            properties.Publisher = publisher;
        }

        protected virtual void ApplySubtitle(MusicProperties properties, IDictionary<string, object> customProperties, string subtitle)
        {
            properties.Subtitle = subtitle;
        }

        protected virtual void ApplyComposers(MusicProperties properties, IDictionary<string, object> customProperties, IEnumerable<string> composers)
        {
            ApplyList(properties.Composers, composers);
        }

        protected virtual void ApplyConductors(MusicProperties properties, IDictionary<string, object> customProperties, IEnumerable<string> conductors)
        {
            ApplyList(properties.Conductors, conductors);
        }

        protected void ApplyList<T>(IList<T> target, IEnumerable<T> source)
        {
            target.Clear();
            foreach (T item in source)
            {
                target.Add(item);
            }
        }
    }
}
