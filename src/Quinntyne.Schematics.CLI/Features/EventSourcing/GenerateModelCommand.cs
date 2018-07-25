using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;
using Quinntyne.Schematics.CLI.DomainEvents;

namespace Quinntyne.Schematics.CLI.Features.EventSourcing
{
    public class GenerateModelCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {                
                Entity = options.Entity;
                Directory = options.Directory;
                Namespace = options.Namespace;
                RootNamespace = options.RootNamespace;
            }

            public dynamic Settings { get; set; }
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
            private readonly IFileWriter _fileWriter;
            private readonly ITemplateLocator _templateLocator;
            private readonly ITemplateProcessor _templateProcessor;
            private readonly INamingConventionConverter _namingConventionConverter;
            private readonly IMediator _mediator;
            public Handler(
                IFileWriter fileWriter,
                INamingConventionConverter namingConventionConverter,
                ITemplateLocator templateLocator, 
                ITemplateProcessor templateProcessor,
                IMediator mediator
                )
            {
                _fileWriter = fileWriter;
                _namingConventionConverter = namingConventionConverter;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
                _mediator = mediator;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {                
                var entityNamePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Entity);
                var entityNameCamelCase = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Entity);

                var template = _templateLocator.Get("GenerateModelCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ entityNamePascalCase }}", entityNamePascalCase },
                    { "{{ entityNameCamelCase }}", entityNameCamelCase },
                    { "{{ namespace }}", request.Namespace },
                    { "{{ rootNamespace }}", request.RootNamespace }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);
                
                _fileWriter.WriteAllLines($"{request.Directory}//{entityNamePascalCase}.cs", result);

                var notification = new EventSourcingModelCreated()
                {
                    Entity = request.Entity,
                    SolutionDirectory = request.SolutionDirectory,
                    RootNamespace= request.RootNamespace
                };

                await _mediator.Publish(notification);
               
            }
        }
    }
}
