using System;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Waf.Foundation;
using System.Collections.Generic;
using Waf.MusicManager.Applications.Properties;

namespace Waf.MusicManager.Applications.DataModels
{
    public class SearchFilterDataModel : Model
    {
        private string userSearchFilter = "";
        private string artistFilter = "";
        private string titleFilter = "";
        private string albumFilter = "";
        private IReadOnlyList<string> genreFilter = Array.Empty<string>();
        private FilterOperator ratingFilterOperator;
        private uint ratingFilter;
        private uint? fromYearFilter;
        private uint? toYearFilter;
        
        public string DisplaySearchFilter
        {
            get 
            {
                string result = "";
                if (!string.IsNullOrEmpty(UserSearchFilter))
                {
                    result = UserSearchFilter;
                }
                if (!string.IsNullOrEmpty(ApplicationSearchFilter))
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += "; ";
                    }
                    result += Resources.FilterIsSet;
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = Resources.Search;
                }
                return result;
            }
        }
        
        public string UserSearchFilter
        {
            get => userSearchFilter;
            set => SetProperty(ref userSearchFilter, value);
        }

        public string ArtistFilter
        {
            get => artistFilter;
            set => SetProperty(ref artistFilter, value);
        }

        public string TitleFilter
        {
            get => titleFilter;
            set => SetProperty(ref titleFilter, value);
        }

        public string AlbumFilter
        {
            get => albumFilter;
            set => SetProperty(ref albumFilter, value);
        }

        public IReadOnlyList<string> GenreFilter
        {
            get => genreFilter;
            set => SetProperty(ref genreFilter, value);
        }

        public FilterOperator RatingFilterOperator
        {
            get => ratingFilterOperator;
            set => SetProperty(ref ratingFilterOperator, value);
        }

        public uint RatingFilter
        {
            get => ratingFilter;
            set
            {
                if (SetProperty(ref ratingFilter, value) && RatingFilterOperator == FilterOperator.Ignore)
                {
                    RatingFilterOperator = FilterOperator.GreaterThanOrEqual;
                }
            }
        }

        public string FromYearFilter
        {
            get => fromYearFilter?.ToString(CultureInfo.CurrentCulture);
            set
            {
                uint? newValue = string.IsNullOrEmpty(value) ? (uint?)null : uint.Parse(value, CultureInfo.CurrentCulture);
                if (fromYearFilter != newValue)
                {
                    fromYearFilter = newValue;
                    RaisePropertyChanged();
                }
            }
        }

        public string ToYearFilter
        {
            get => toYearFilter?.ToString(CultureInfo.CurrentCulture);
            set
            {
                uint? newValue = string.IsNullOrEmpty(value) ? (uint?)null : uint.Parse(value, CultureInfo.CurrentCulture);
                if (toYearFilter != newValue)
                {
                    toYearFilter = newValue;
                    RaisePropertyChanged();
                }
            }
        }

        public string ApplicationSearchFilter
        {
            get
            {
                var artistFilterCore = string.IsNullOrEmpty(ArtistFilter) ? null : string.Format(CultureInfo.InvariantCulture, "System.Music.Artist:~=\"{0}\"", ArtistFilter);
                var titleFilterCore = string.IsNullOrEmpty(TitleFilter) ? null : string.Format(CultureInfo.InvariantCulture, "System.Title:~=\"{0}\"", TitleFilter);
                var albumFilterCore = string.IsNullOrEmpty(AlbumFilter) ? null : string.Format(CultureInfo.InvariantCulture, "System.Music.AlbumTitle:~=\"{0}\"", AlbumFilter);

                var genreFilterCore = !GenreFilter.Any() ? null : string.Join(" OR ", GenreFilter.Select(x => string.Format(CultureInfo.InvariantCulture, "System.Music.Genre:\"{0}\"", x)));

                var ratingFilterOperatorCore = GetRatingFilterOperatorCore();
                var ratingFilterCore = string.IsNullOrEmpty(ratingFilterOperatorCore) ? null : string.Format(CultureInfo.InvariantCulture, "System.Rating:{0}{1}", ratingFilterOperatorCore, RatingFilter);

                var fromYearFilterCore = string.IsNullOrEmpty(FromYearFilter) ? null : string.Format(CultureInfo.InvariantCulture, ">={0}", FromYearFilter);
                var toYearFilterCore = string.IsNullOrEmpty(ToYearFilter) ? null : string.Format(CultureInfo.InvariantCulture, "<={0}", ToYearFilter);
                var combinedYearFilterCore = string.Join(" ", new[] { fromYearFilterCore, toYearFilterCore }.Where(x => !string.IsNullOrEmpty(x)));
                var yearFilterCore = string.IsNullOrEmpty(combinedYearFilterCore) ? null : string.Format(CultureInfo.InvariantCulture, "System.Media.Year:{0}", combinedYearFilterCore);

                return string.Join(" AND ", new[] { artistFilterCore, titleFilterCore, albumFilterCore, genreFilterCore, ratingFilterCore, yearFilterCore }.Where(x => !string.IsNullOrEmpty(x)));
            }
        }

        public void Clear()
        {
            UserSearchFilter = "";
            ArtistFilter = "";
            TitleFilter = "";
            AlbumFilter = "";
            if (GenreFilter.Any())
            {
                GenreFilter = Array.Empty<string>();
            }
            FromYearFilter = "";
            ToYearFilter = "";
            RatingFilter = 0;
            RatingFilterOperator = FilterOperator.Ignore;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName != nameof(DisplaySearchFilter) && e.PropertyName != nameof(ApplicationSearchFilter))
            {
                RaisePropertyChanged(nameof(DisplaySearchFilter));
            }
            if (new[] { nameof(ArtistFilter), nameof(TitleFilter), nameof(AlbumFilter), nameof(GenreFilter), nameof(RatingFilterOperator), nameof(RatingFilter),
                    nameof(FromYearFilter), nameof(ToYearFilter) }.Contains(e.PropertyName))
            {
                RaisePropertyChanged(nameof(ApplicationSearchFilter));
            }
        }

        private string GetRatingFilterOperatorCore()
        {
            switch (RatingFilterOperator)
            {
                case FilterOperator.GreaterThanOrEqual:
                    return ">=";
                case FilterOperator.LessThanOrEqual:
                    return "<=";
                default:
                    return null;
            }
        }
    }
}
