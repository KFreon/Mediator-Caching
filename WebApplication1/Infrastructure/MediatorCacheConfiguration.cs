using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApplication1.Infrastructure
{
    // NOTE this would normally take in AppSettings configuration.

    public interface IMediatorCacheConfiguration
    {
        public void Register(IServiceCollection services);
    }

    public abstract class MediatorCacheConfiguration<TCache, TResult> : IMediatorCacheConfiguration where TCache : IRequest<TResult>
    {
        protected abstract MediatorCacheOptions<TCache> ConfigureCaching(/* AppSettings config here */);
        protected virtual Action<IServiceCollection>[] ConfigureCacheInvalidation() => Array.Empty<Action<IServiceCollection>>();

        public void Register(IServiceCollection services)
        {
            services.AddSingleton<MediatorCacheAccessor<TCache, TResult>>();

            // Pretend config is being taken here...
            var cachingConfig = ConfigureCaching();
            services.RegisterMediatorCaching<TCache, TResult>(cachingConfig.AbsoluteDuration, cachingConfig.SlidingDuration, cachingConfig.CachePrefix, cachingConfig.KeyGenerator);

            var invalidators = ConfigureCacheInvalidation();
            foreach (var invalidator in invalidators)
            {
                invalidator(services);
            }
        }

        /// <summary>
        /// Registers trigger query to invalidate caches based on their partial key(prefix)
        /// </summary>
        /// <typeparam name="TTrigger">Query to listen for</typeparam>
        /// <typeparam name="TTriggerResult">Result of trigger query. Use Unit for IRequest with no params</typeparam>
        /// <param name="cachePrefix">Partial key of caches to invalidate. Defaults to name of query.</param>
        /// <returns>List of configuration actions</returns>
        public Action<IServiceCollection> RegisterInvalidator<TTrigger, TTriggerResult>(string cachePrefix = null) where TTrigger : IRequest<TTriggerResult>
        {
            return builder => builder.RegisterMediatorCacheInvalidation<TCache, TResult, TTrigger, TTriggerResult>(cachePrefix);
        }
    }
}
