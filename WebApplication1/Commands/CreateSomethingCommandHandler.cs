using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1.Commands
{
    public class CreateSomethingCommandHandler : IRequestHandler<CreateSomethingCommand>
    {
        public async Task<Unit> Handle(CreateSomethingCommand request, CancellationToken cancellationToken)
        {
            // This would be creating something that we want to invalidate the associated cache for.
            return Unit.Value;
        }
    }
}
