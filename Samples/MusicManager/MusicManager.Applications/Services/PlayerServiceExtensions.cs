namespace Waf.MusicManager.Applications.Services
{
    public static class PlayerServiceExtensions
    {
        public static void Play(this IPlayerService playerService)
        {
            if (playerService.IsPlayCommand)
            {
                playerService.PlayPauseCommand.Execute(null);
            }
        }
    }
}
