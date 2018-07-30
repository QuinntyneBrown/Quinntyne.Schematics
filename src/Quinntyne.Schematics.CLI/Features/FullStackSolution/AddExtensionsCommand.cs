using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution
{
    public class AddExtensionsCommand
    {
        public class Request : Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {
                Entity = options.Entity;
                Directory = options.Directory;
                Namespace = options.Namespace;
                RootNamespace = options.RootNamespace;
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

            public Handler(IMediator mediator) => _mediator = mediator;

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var classes = string.Join(",", new List<string>() {
                    "IntegrationEventsHub",
                    "SignalRContractResolver",
                    "HttpClientExtensions",
                    "ServiceCollectionExtensions",
                    "InfrastructureServiceCollectionExtensions",
                    "Startup",
                    "Program",
                    "ValidationBehaviour"
                });

                await _mediator.Send(new GenerateClassCommand.Request(request.Options)
                {
                    ClassName = classes,
                    SolutionDirectory = request.SolutionDirectory
                });
            }
        }
    }
}
