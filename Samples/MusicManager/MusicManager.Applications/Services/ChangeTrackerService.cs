using System.Collections.Generic;
using System.Linq;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services
{
    internal class ChangeTrackerService : IChangeTrackerService
    {
        private readonly HashSet<Entity> entitiesWithChanges;

        public ChangeTrackerService()
        {
            entitiesWithChanges = new HashSet<Entity>();
        }

        public IEnumerable<Entity> GetEntitiesWithChanges()
        {
            return entitiesWithChanges.ToArray();
        }

        public void EntityHasChanges(Entity entity)
        {
            entitiesWithChanges.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entitiesWithChanges.Remove(entity);
        }
    }
}
