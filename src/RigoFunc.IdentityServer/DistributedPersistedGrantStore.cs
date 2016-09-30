using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace RigoFunc.IdentityServer {
    public class DistributedPersistedGrantStore : IPersistedGrantStore {
        private const string DefaultKey = "59d63571-c4c2-4daa-aac6-969f581dc1fa";
        private readonly IDistributedCache _cache;
        private DistributedCacheEntryOptions _dceo;

        public DistributedPersistedGrantStore(IDistributedCache cache) {
            _cache = cache;
            _dceo = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(15));
                //.SetSlidingExpiration(TimeSpan.FromDays(1));
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId) {
            var list = new List<PersistedGrant>();
            var keys = await _cache.GetStringAsync(DefaultKey);
            if (!string.IsNullOrEmpty(keys)) {
                var keyList = JsonConvert.DeserializeObject<List<string>>(keys);
                foreach (var key in keyList) {
                    var grant = await GetAsync(key);
                    if (grant?.SubjectId == subjectId) {
                        list.Add(grant);
                    }
                }
            }

            return list;
        }

        public async Task<PersistedGrant> GetAsync(string key) {
            var data = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data)) {
                return null;
            }

            return JsonConvert.DeserializeObject<PersistedGrant>(data);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId) {
            var keys = await _cache.GetStringAsync(DefaultKey);
            if (!string.IsNullOrEmpty(keys)) {
                var keyList = JsonConvert.DeserializeObject<List<string>>(keys);
                var removed = new List<string>();
                foreach (var key in keyList) {
                    var grant = await GetAsync(key);
                    if (grant?.SubjectId == subjectId && grant?.ClientId == clientId) {
                        await RemoveAsync(key);
                        removed.Add(key);
                    }
                }
                if (removed.Count > 0) {
                    var rest = keyList.Except(removed);

                    await _cache.SetStringAsync(DefaultKey, JsonConvert.SerializeObject(rest), _dceo);
                }
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type) {
            var keys = await _cache.GetStringAsync(DefaultKey);
            if (!string.IsNullOrEmpty(keys)) {
                var keyList = JsonConvert.DeserializeObject<List<string>>(keys);
                var removed = new List<string>();
                foreach (var key in keyList) {
                    var grant = await GetAsync(key);
                    if(grant?.SubjectId == subjectId && grant?.ClientId == clientId && grant?.Type == type) {
                        await RemoveAsync(key);
                        removed.Add(key);
                    }
                }
                if(removed.Count > 0) {
                    var rest = keyList.Except(removed);

                    await _cache.SetStringAsync(DefaultKey, JsonConvert.SerializeObject(rest), _dceo);
                }
            }
        }

        public async Task RemoveAsync(string key) {
            await _cache.RemoveAsync(key);
        }

        public async Task StoreAsync(PersistedGrant grant) {
            var data = JsonConvert.SerializeObject(grant);
            await _cache.SetStringAsync(grant.Key, data, _dceo);

            var keys = await _cache.GetStringAsync(DefaultKey);
            if (!string.IsNullOrEmpty(keys)) {
                var keyList = JsonConvert.DeserializeObject<List<string>>(keys);
                keyList.Add(grant.Key);

                await _cache.SetStringAsync(DefaultKey, JsonConvert.SerializeObject(keyList), _dceo);
            }
            else {
                await _cache.SetStringAsync(DefaultKey, JsonConvert.SerializeObject(new List<string> { grant.Key }), _dceo);
            }
        }
    }
}
