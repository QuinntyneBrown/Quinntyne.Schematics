using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class GenerateOverlayServiceCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {                
                Entity = options.Entity;
                Directory = options.Directory;
                Namespace = options.Namespace;
                RootNamespace = options.RootNamespace;
                Name = options.Name;
                Options = options;
            }

            public dynamic Settings { get; set; }
            public IOptions Options;
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
                var namePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Name);
                var nameCamelCase = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Name);
                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name);
                var entityNameCamelCase = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Entity);

                var template = _templateLocator.Get("GenerateOverlayServiceCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ namePascalCase }}", namePascalCase },
                    { "{{ nameCamelCase }}", nameCamelCase },
                    { "{{ nameSnakeCase }}", nameSnakeCase },
                    { "{{ namespace }}", request.Namespace },
                    { "{{ rootNamespace }}", request.RootNamespace },
                    { "{{ entityNameCamelCase }}", entityNameCamelCase }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);                
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.ts", result);
                await _mediator.Send(new GenerateComponentCommand.Request(request.Options));
            }
        }
    }
}
