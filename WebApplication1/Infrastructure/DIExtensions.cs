using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace WebApplication1.Infrastructure
{
    public static class DIExtensions
    {
        /// <summary>
        /// Adds caching for mediator query.
        /// </summary>
        /// <typeparam name="TCache">Query to cache</typeparam>
        /// <typeparam name="TResult">Result of cached query</typeparam>
        /// <param name="builder">DI builder</param>
        /// <param name="absoluteCacheDuration">Absolute lifetime of cache</param>
        /// <param name="slidingCacheDuration">Sliding lifetime of cache</param>
        /// <param name="cachePrefix">Used for invalidation. Defaults to query type namespace.name</param>
        /// <param name="keyGenerator">Defaults to json serialsed query</param>
        public static IServiceCollection RegisterMediatorCaching<TCache, TResult>(this IServiceCollection services, TimeSpan? absoluteCacheDuration, TimeSpan? slidingCacheDuration, string cachePrefix = null, Func<TCache, string> keyGenerator = null) where TCache : IRequest<TResult>
        {
            services.AddTransient<IPipelineBehavior<TCache, TResult>>(x =>
            {
                var cache = x.GetRequiredService<MediatorCacheAccessor<TCache, TResult>>();
                return new MediatorCachingBehaviour<TCache, TResult>(cache, absoluteCacheDuration, slidingCacheDuration, cachePrefix, keyGenerator);
            });

            return services;
        }

        /// <summary>
        /// Adds cache invalidation trigger for query to query.
        /// </summary>
        /// <typeparam name="TCache">Query to invalidate</typeparam>
        /// <typeparam name="TCacheResult">Result of query to invalidate</typeparam>
        /// <typeparam name="TTrigger">Query to trigger invalidation.</typeparam>
        /// <typeparam name="TTriggerResult">Result of query to trigger invalidation</typeparam>
        /// <param name="builder">DI builder</param>
        /// <param name="cachePrefix">Used for invalidation. Defaults to query type namespace.name</param>
        public static IServiceCollection RegisterMediatorCacheInvalidation<TCache, TCacheResult, TTrigger, TTriggerResult>(this IServiceCollection services, string cachePrefix = null)
            where TCache : IRequest<TCacheResult> where TTrigger : IRequest<TTriggerResult>
        {
            services.AddTransient<IPipelineBehavior<TTrigger, TTriggerResult>>(x =>
            {
                var cache = x.GetRequiredService<MediatorCacheAccessor<TCache, TCacheResult>>();
                var logger = x.GetRequiredService<ILogger<MediatorCacheAccessor<TCache, TCacheResult>>>();
                return new MediatorCacheInvalidationBehaviour<TCache, TCacheResult, TTrigger, TTriggerResult>(cache, logger, cachePrefix);
            });
            return services;
        }

        // https://sudonull.com/post/72038-Passing-configuration-parameters-to-Autofac-modules-in-ASPNET-Core
        public static void RegisterMediatorCaches<TModule>(this IServiceCollection builder)
        {
            var mediatorCacheConfigurations = typeof(TModule)
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .Where(x => !x.IsGenericType && x.GetInterfaces().Contains(typeof(IMediatorCacheConfiguration)))
                .ToArray();

            foreach (var configuration in mediatorCacheConfigurations)
            {
                var cacheConfiguration = Activator.CreateInstance(configuration) as IMediatorCacheConfiguration;
                cacheConfiguration.Register(builder);
            }
        }
    }
}
