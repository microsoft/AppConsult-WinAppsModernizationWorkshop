using System;
using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Presentation.Converters
{
    public class IsPlaylistItemPlayingMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var playingPlaylistItem = (PlaylistItem)values[0];
            var currentPlaylistItem = (PlaylistItem)values[1];

            return playingPlaylistItem == currentPlaylistItem;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
