namespace Waf.MusicManager.Domain.Playlists
{
    public interface IRandomService
    {
        int NextRandomNumber(int maxValue);
    }
}
