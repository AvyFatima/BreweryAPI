using System;
using System.Threading.Tasks;
using BreweryAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BreweryAPI.Services
{
    public sealed class CacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public CacheProvider(IMemoryCache cache) => _cache = cache;

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out T? value) && value is not null) return value;

            var created = await factory();
            _cache.Set(key, created, ttl);
            return created;
        }

        public void Remove(string key) => _cache.Remove(key);
    }

}
