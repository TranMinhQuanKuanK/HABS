using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Redis
{
    public class RedisService 
    {
        private readonly IDistributedCache _distributedCache;
        public RedisService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public void SetValueToKey(string key, string value)
        {
           var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60*10));
            _distributedCache.SetString(key, value, options);
        }
        public void RemoveValueToKey(string key)
        {
            _distributedCache.Remove(key);
        }
        public string GetValueFromKey(string key)
        {
            return _distributedCache.GetString(key);
        }
    }
}
