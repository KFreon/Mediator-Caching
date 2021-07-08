using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Infrastructure
{
    public class MediatorCacheInvalidationBehaviour<TCache, TCacheResult, TTrigger, TTriggerResult> : IPipelineBehavior<TTrigger, TTriggerResult> where TCache : IRequest<TCacheResult> where TTrigger : IRequest<TTriggerResult>
    {
        private readonly MediatorCacheAccessor<TCache, TCacheResult> _cache;
        private readonly ILogger<MediatorCacheAccessor<TCache, TCacheResult>> _logger;
        private readonly string _keyPrefix;

        public MediatorCacheInvalidationBehaviour(MediatorCacheAccessor<TCache, TCacheResult> cache, ILogger<MediatorCacheAccessor<TCache, TCacheResult>> logger, string cachePrefix = null)
        {
            _cache = cache;
            _logger = logger;
            _keyPrefix = cachePrefix ?? typeof(TCache).GetTypeInfo().FullName;
        }

        public async Task<TTriggerResult> Handle(TTrigger command, CancellationToken cancellationToken, RequestHandlerDelegate<TTriggerResult> next)
        {
            try
            {
                return await next();
            }
            finally
            {
                var fullKeysCount = _cache.RemoveItem(_keyPrefix);
                _logger.LogDebug("Invalidating Mediator cache {Cache} for trigger {Trigger} and {Count} full keys based on partial.", _keyPrefix, typeof(TTrigger).GetTypeInfo().FullName, fullKeysCount);
            }
        }
    }
}
