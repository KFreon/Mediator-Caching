using MediatR;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Infrastructure
{
    public class MediatorCachingBehaviour<TCache, TResult> : IPipelineBehavior<TCache, TResult> where TCache : IRequest<TResult>
    {
        private readonly TimeSpan? _absoluteExpiration;
        private readonly TimeSpan? _slidingExpiration;
        private readonly string _keyPrefix;
        private readonly Func<TCache, string> _keyGenerator;
        private readonly MediatorCacheAccessor<TCache, TResult> _cacheAccessor;

        public MediatorCachingBehaviour(
            MediatorCacheAccessor<TCache, TResult> cacheAccessor,
            TimeSpan? absoluteExpiration,
            TimeSpan? slidingExpiration,
            string cachePrefix = null,
            Func<TCache, string> keyGenerator = null)
        {
            _absoluteExpiration = absoluteExpiration;
            _slidingExpiration = slidingExpiration;
            _keyPrefix = cachePrefix ?? typeof(TCache).GetTypeInfo().FullName;
            _keyGenerator = keyGenerator;
            _cacheAccessor = cacheAccessor;
        }

        public Task<TResult> Handle(TCache query, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            return _cacheAccessor.GetOrCacheItem(query, () => next(), _absoluteExpiration, _slidingExpiration, _keyPrefix, _keyGenerator);
        }
    }
}
