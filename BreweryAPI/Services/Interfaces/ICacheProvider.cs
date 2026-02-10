using System;
using System.Threading.Tasks;

namespace BreweryAPI.Services.Interfaces
{
    public interface ICacheProvider
    {
        Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory);
        void Remove(string key);
    }
}
