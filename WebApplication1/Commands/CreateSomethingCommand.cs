using MediatR;

namespace WebApplication1.Commands
{
    public class CreateSomethingCommand : IRequest
    {
        public int Thing { get; set; }
    }
}
