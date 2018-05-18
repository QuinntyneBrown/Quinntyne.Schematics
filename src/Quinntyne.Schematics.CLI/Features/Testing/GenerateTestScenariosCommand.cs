using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.Testing
{
    public class GenerateTestScenariosCommand
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
                var entityNamePascalCasePlural = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Entity, true);
                var entityNameCamelCasePlural = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Entity, true);

                var templateScenarios = _templateLocator.Get("GenerateTestScenariosCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ entityNamePascalCase }}", entityNamePascalCase },
                    { "{{ entityNameCamelCase }}", entityNameCamelCase },
                    { "{{ entityNamePascalCasePlural }}", entityNamePascalCasePlural },
                    { "{{ entityNameCamelCasePlural }}", entityNameCamelCasePlural },
                    { "{{ namespace }}", request.Namespace },
                    { "{{ rootNamespace }}", request.RootNamespace }
                };

                var resultScenarios = _templateProcessor.ProcessTemplate(templateScenarios, tokens);

                _fileWriter.WriteAllLines($"{request.Directory}//{entityNamePascalCase}Scenarios.cs", resultScenarios);

                await _mediator.Send(new GenerateTestScenarioBaseCommand.Request(request.Options));                
            }
        }
    }
}
