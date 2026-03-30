using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.DataAccess.Repositories
{
    public class RedisPreferencesRepository : IPreferencesCacheRepository
    {
        private readonly IDistributedCache _cache;
        private const string CacheKey = "all_preferences";

        public RedisPreferencesRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<IEnumerable<Preference>> GetAllAsync()
        {
            var cachedData = await _cache.GetStringAsync(CacheKey);
            if (string.IsNullOrEmpty(cachedData))
                return new List<Preference>();

            return JsonSerializer.Deserialize<List<Preference>>(cachedData);
        }

        public async Task<Preference> GetByIdAsync(Guid id)
        {
            var all = await GetAllAsync();

            return all.FirstOrDefault(x => x.Id == id);
        }

        public async Task InitializeCacheAsync(IEnumerable<Preference> preferences)
        {
            var serialized = JsonSerializer.Serialize(preferences);
            
            await _cache.SetStringAsync(CacheKey, serialized);
        }
    }
}