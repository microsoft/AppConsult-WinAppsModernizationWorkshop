using System;

namespace Waf.MusicManager.Domain.Playlists
{
    internal class RandomService : IRandomService
    {
        public int NextRandomNumber(int maxValue)
        {
            if (maxValue == int.MaxValue) throw new ArgumentOutOfRangeException(nameof(maxValue), "maxValue must be lower than Int32.MaxValue");
            return new Random().Next(maxValue + 1);
        }
    }
}
