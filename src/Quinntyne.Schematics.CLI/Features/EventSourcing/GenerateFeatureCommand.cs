using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.EventSourcing
{
    public class GenerateFeatureCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options) => Options = options;

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
            private IMediator _mediator;
            public Handler(IMediator mediator) => _mediator = mediator;

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                await _mediator.Send(new GenerateApiModelCommand.Request(request.Options));
                await _mediator.Send(new GenerateRemoveCommand.Request(request.Options));             
                await _mediator.Send(new GenerateGetByIdQueryCommand.Request(request.Options));
                await _mediator.Send(new GenerateGetQueryCommand.Request(request.Options));
                await _mediator.Send(new GenerateCreateCommand.Request(request.Options));
                await _mediator.Send(new GenerateUpdateCommand.Request(request.Options));
                await _mediator.Send(new GenerateControllerCommand.Request(request.Options));

                await Task.CompletedTask;
            }
        }
    }
}
