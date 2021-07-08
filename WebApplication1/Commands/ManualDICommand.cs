using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Infrastructure;
using WebApplication1.Queries;

namespace WebApplication1.Commands
{
    // Demonstrating the manual DI method
    public class ManualDICommand : IRequest<string>
    {

    }

    public class ManualDICommandHandler : IRequestHandler<ManualDICommand, string>
    {
        private readonly MediatorCacheAccessor<GetSomethingQuery, string[]> _cache;

        public ManualDICommandHandler(MediatorCacheAccessor<GetSomethingQuery, string[]> cache)
        {
            _cache = cache;
        }

        public async Task<string> Handle(ManualDICommand request, CancellationToken cancellationToken)
        {
            // Do some work

            // Invalidate cache
            _cache.RemoveItem(nameof(GetSomethingQuery));
            return string.Empty;
        }
    }
}
