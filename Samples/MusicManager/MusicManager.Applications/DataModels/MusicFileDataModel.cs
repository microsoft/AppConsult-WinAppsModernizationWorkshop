using System;
using System.ComponentModel;
using System.Globalization;
using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.DataModels
{
    public class MusicFileDataModel : Model
    {
        public MusicFileDataModel(MusicFile musicFile)
        {
            MusicFile = musicFile;
            if (musicFile.IsMetadataLoaded)
            {
                MetadataLoaded();
            }
            else
            {
                PropertyChangedEventManager.AddHandler(musicFile, MusicFilePropertyChanged, "");
            }
        }


        public MusicFile MusicFile { get; }

        public string ArtistsString => string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ", 
                MusicFile.IsMetadataLoaded ? MusicFile.Metadata.Artists : Array.Empty<string>()); 


        private void MetadataLoaded()
        {
            PropertyChangedEventManager.AddHandler(MusicFile.Metadata, MetadataPropertyChanged, "");
        }

        private void MusicFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicFile.IsMetadataLoaded))
            {
                PropertyChangedEventManager.RemoveHandler(MusicFile, MusicFilePropertyChanged, "");
                MetadataLoaded();
                RaisePropertyChanged(nameof(ArtistsString));
            }
        }

        private void MetadataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicMetadata.Artists))
            {
                RaisePropertyChanged(nameof(ArtistsString));
            }
        }
    }
}
