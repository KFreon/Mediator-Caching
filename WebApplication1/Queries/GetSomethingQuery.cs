using MediatR;
using System;

namespace WebApplication1.Queries
{
    public class GetSomethingQuery : IRequest<string[]>
    {
        public Guid Id { get;set;  }
    }
}
