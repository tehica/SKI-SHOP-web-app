using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IResponseCacheService
    {
        // we use this method to cash any responses when we recieve some data from database
        // that response we cash in its entirity into memory
        // memory in this case is REDIS and REDIS is used for caching responses
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);

        // this task returns string
        Task<string> GetCachedResponseAsync(string cacheKey);
    }
}
