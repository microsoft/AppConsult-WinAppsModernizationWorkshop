using System.Collections.Generic;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public static class Genres
    {
        private static readonly string[] defaultValues = 
        { 
            "Blues",
            "Classical",
            "Comedy",
            "Country",
            "Dance",
            "Easy Listening",
            "Electronic",
            "Funk",
            "Hip Hop",
            "Jazz",
            "Oldies",
            "Pop",
            "R&B",
            "Reggae",
            "Rock",
            "Soundtrack",
            "Techno"
        };


        public static IReadOnlyList<string> DefaultValues => defaultValues;
    }
}
