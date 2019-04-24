namespace Waf.MusicManager.Domain.MusicFiles
{
    public interface IChangeTrackerService
    {
        void EntityHasChanges(Entity entity);
    }
}
