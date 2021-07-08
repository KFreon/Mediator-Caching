using LazyCache;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Infrastructure
{
    public class MediatorCacheAccessor<TCache, TResult> where TCache : IRequest<TResult>
    {
        private readonly IAppCache _cache;
        private readonly ILogger<MediatorCacheAccessor<TCache, TResult>> _logger;

        public MediatorCacheAccessor(IAppCache cache, ILogger<MediatorCacheAccessor<TCache, TResult>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResult> GetOrCacheItem(
            TCache query,
            Func<Task<TResult>> itemGetter,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            string keyPrefix = null,
            Func<TCache, string> keyGenerator = null)
        {
            var key = "somekey";  // Normally this would fallback to JSON serialisation if not specified (which would be default)
            _logger.LogWarning("Accessing Mediator cache: {Prefix}:{Key}", keyPrefix, key);
            var entryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };
            var result = await _cache.GetOrAddAsync(key, () =>
            {
                // Update partial key
                var partials = _cache.GetOrAdd(keyPrefix, _ => new List<string>());
                if (!partials.Contains(key))
                {
                    partials.Add(key);
                    _cache.Add(keyPrefix, partials, new MemoryCacheEntryOptions { Priority = CacheItemPriority.NeverRemove });
                }
                _logger.LogWarning("Caching Mediator: {Prefix}:{Key}", keyPrefix, key);
                return itemGetter();
            }, entryOptions);

            return result;
        }

        public int RemoveItem(string keyPrefix)
        {
            _logger.LogWarning("Invalidating Mediator cache: {Prefix}", keyPrefix);
            // If key list isn't present, just use empty.
            var fullKeys = _cache.Get<List<string>>(keyPrefix) ?? new List<string>();
            var fullKeysCount = fullKeys.Count;
            foreach (var key in fullKeys)
            {
                _cache.Remove(key);
            }
            _cache.Remove(keyPrefix);
            return fullKeysCount;
        }
    }
}

