using MediatR;
using Quinntyne.Schematics.CLI.Features.Angular;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinntyne.Schematics.CLI
{
    public class GenerateBarralBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        IRequestHandler<GenerateIndexCommand.Request> _handler;

        public GenerateBarralBehavior(IRequestHandler<GenerateIndexCommand.Request> handler)
        {
            _handler = handler;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            if (request.GetType().FullName.Contains("Features.Angular"))
                await _handler.Handle(new GenerateIndexCommand.Request(),default);

            return response;
        }
    }
}
