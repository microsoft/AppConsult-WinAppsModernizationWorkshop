using System;
using System.Collections.Generic;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public class MusicMetadata : Entity
    {
        private IReadOnlyList<string> artists = Array.Empty<string>();
        private string title = "";
        private uint rating;
        private string album = "";
        private uint trackNumber;
        private uint year;
        private IReadOnlyList<string> genre = Array.Empty<string>();
        private string albumArtist = "";
        private string publisher = "";
        private string subtitle = "";
        private IReadOnlyList<string> composers = Array.Empty<string>();
        private IReadOnlyList<string> conductors = Array.Empty<string>();

        private MusicMetadata(TimeSpan duration, long bitrate, bool isSupported)
        {
            IsSupported = isSupported;
            Duration = duration;
            Bitrate = bitrate;
        }

        public MusicMetadata(TimeSpan duration, long bitrate) : this(duration, bitrate, true)
        {
        }

        public static MusicMetadata CreateUnsupported(TimeSpan duration, long bitrate)
        {
            return new MusicMetadata(duration, bitrate, false);
        }

        public object Parent { get; set; }

        public bool IsSupported { get; }

        public IReadOnlyList<string> Artists
        {
            get => artists;
            set => SetPropertyAndTrackChanges(ref artists, value);
        }

        public string Title
        {
            get => title;
            set => SetPropertyAndTrackChanges(ref title, value);
        }

        public TimeSpan Duration { get; }

        public uint Rating
        {
            get => rating;
            set => SetPropertyAndTrackChanges(ref rating, value);
        }

        public string Album
        {
            get => album;
            set => SetPropertyAndTrackChanges(ref album, value);
        }

        public uint TrackNumber
        {
            get => trackNumber;
            set => SetPropertyAndTrackChanges(ref trackNumber, value);
        }

        public uint Year
        {
            get => year;
            set => SetPropertyAndTrackChanges(ref year, value);
        }

        public IReadOnlyList<string> Genre
        {
            get => genre;
            set => SetPropertyAndTrackChanges(ref genre, value);
        }

        public long Bitrate { get; }

        public string AlbumArtist
        {
            get => albumArtist;
            set => SetPropertyAndTrackChanges(ref albumArtist, value);
        }

        public string Publisher
        {
            get => publisher;
            set => SetPropertyAndTrackChanges(ref publisher, value);
        }

        public string Subtitle
        {
            get => subtitle;
            set => SetPropertyAndTrackChanges(ref subtitle, value);
        }

        public IReadOnlyList<string> Composers
        {
            get => composers;
            set => SetPropertyAndTrackChanges(ref composers, value);
        }

        public IReadOnlyList<string> Conductors
        {
            get => conductors;
            set => SetPropertyAndTrackChanges(ref conductors, value);
        }

        public void ApplyValuesFrom(MusicMetadata sourceMetadata)
        {
            Artists = sourceMetadata.Artists;
            Title = sourceMetadata.Title;
            Rating = sourceMetadata.Rating;
            Album = sourceMetadata.Album;
            TrackNumber = sourceMetadata.TrackNumber;
            Year = sourceMetadata.Year;
            Genre = sourceMetadata.Genre;
            AlbumArtist = sourceMetadata.AlbumArtist;
            Publisher = sourceMetadata.Publisher;
            Subtitle = sourceMetadata.Subtitle;
            Composers = sourceMetadata.Composers;
            Conductors = sourceMetadata.Conductors;
        }
    }
}
