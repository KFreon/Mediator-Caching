using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication1.Commands;
using WebApplication1.Queries;

namespace WebApplication1.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<string[]> GetThing()
        {
            return await _mediator.Send(new GetSomethingQuery { Id = Guid.NewGuid() });
        }

        [HttpPost]
        public async Task CreateSomething()
        {
            await _mediator.Send(new CreateSomethingCommand());
        }
    }
}
