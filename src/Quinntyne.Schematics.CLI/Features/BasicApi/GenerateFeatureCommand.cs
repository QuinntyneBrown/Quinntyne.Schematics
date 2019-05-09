using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.BasicApi
{
    public class GenerateFeatureCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {                
                Options = options;
            }

            public dynamic Settings { get; set; }
            public IOptions Options { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.Entity).NotNull();
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                await _mediator.Send(new GenerateDtoCommand.Request(request.Options));
                await _mediator.Send(new GenerateRemoveCommand.Request(request.Options));
                await _mediator.Send(new GenerateGetByIdQueryCommand.Request(request.Options));
                await _mediator.Send(new GenerateGetQueryCommand.Request(request.Options));
                await _mediator.Send(new GenerateUpsertCommand.Request(request.Options));                
                await _mediator.Send(new GenerateControllerCommand.Request(request.Options));
            }
        }
    }
}
