using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;
using System.Diagnostics;
using static System.Diagnostics.ProcessWindowStyle;
using static System.Environment;
using System;
using System.Linq;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution.Section1
{
    public class Section1Step01Command
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
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

        public class Handler : IRequestHandler<Request>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator) => _mediator = mediator;

            public async Task Handle(Request request, CancellationToken cancellationToken)
                => await _mediator.Send(new GenerateSolutionCommand.Request(request.Options));
        }
    }
}
