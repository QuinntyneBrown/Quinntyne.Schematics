using FluentValidation;
using MediatR;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution
{
    public class AddDashboardCommand
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

            public Handler(
                IMediator mediator
                )
            {
                _mediator = mediator;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken) {
                var models = new List<string>() { };
                var events = new List<string>() { };
                var cardApi = new List<string>() { };
                var dashboardCardApi = new List<string>() { };
                var dashboardApi = new List<string>() { };

                var classes = string.Join(",",models.Concat(events).Concat(cardApi).Concat(dashboardApi).Concat(dashboardCardApi).ToArray());

                var cardClient = new List<string>() { };
                var dashboardCardClient = new List<string>() { };
                var dashboardClient = new List<string>() { };

                var files = string.Join(",",cardClient.Concat(dashboardCardClient).Concat(dashboardClient).ToArray());

                await _mediator.Send(new GenerateClassCommand.Request(request.Options)
                {
                    ClassName = classes,
                    SolutionDirectory = request.SolutionDirectory
                });

                await _mediator.Send(new GenerateFileCommand.Request(request.Options)
                {
                    Name = files,
                    SolutionDirectory = request.SolutionDirectory
                });
            }
        }
    }
}
