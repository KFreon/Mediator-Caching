using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Infrastructure
{
    public class MediatorCacheOptions<TCache>
    {
        public TimeSpan? AbsoluteDuration { get; set; }
        public TimeSpan? SlidingDuration { get; set; }

        /// <summary>
        /// Optional. Defaults to Query fullname.
        /// Must be set on invalidator as well.
        /// </summary>
        public string CachePrefix { get; set; }
        public Func<TCache, string> KeyGenerator { get; set; }
    }
}
