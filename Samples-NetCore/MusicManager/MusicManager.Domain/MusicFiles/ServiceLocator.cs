using System;
using System.Collections.Concurrent;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public static class ServiceLocator
    {
        private static readonly ConcurrentDictionary<Type, object> services = new ConcurrentDictionary<Type, object>();

        public static TId Get<TId>()
        {
            return (TId)services[typeof(TId)];
        }

        public static void RegisterInstance<TId>(TId instance)
        {
            services[typeof(TId)] = instance;
        }
    }
}
