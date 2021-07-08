using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Queries
{
    public class GetSomethingQueryHandler : IRequestHandler<GetSomethingQuery, string[]>
    {
        public async Task<string[]> Handle(GetSomethingQuery request, CancellationToken cancellationToken)
        {
            // This would be a long running action like a database access, external services, etc
            return new[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
        }
    }
}
