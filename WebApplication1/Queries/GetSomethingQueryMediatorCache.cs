using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApplication1.Commands;
using WebApplication1.Infrastructure;

namespace WebApplication1.Queries
{
    public class GetSomethingQueryMediatorCache : MediatorCacheConfiguration<GetSomethingQuery, string[]>
    {
        protected override MediatorCacheOptions<GetSomethingQuery> ConfigureCaching()
        {
            return new MediatorCacheOptions<GetSomethingQuery>
            {
                SlidingDuration = TimeSpan.FromHours(1),
                AbsoluteDuration = TimeSpan.FromHours(10),  // Slide for 1h, but only to a max of 10h.
                //CachePrefix = ""           // DEFAULTS to the namespace + query, but it's here so you could override for some reason.
                //KeyGenerator = () => null  // Would normally default to json serialisation of the query (in this case, just the ID)
            };
        }

        protected override Action<IServiceCollection>[] ConfigureCacheInvalidation()
        {
            return new[]
            {
                RegisterInvalidator<CreateSomethingCommand, Unit>()
            };
        }
    }
}
