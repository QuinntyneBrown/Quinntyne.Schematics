using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class GenerateResponsiveComponentCommand
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

            public Handler(
                IFileWriter fileWriter,
                INamingConventionConverter namingConventionConverter,
                ITemplateLocator templateLocator, 
                ITemplateProcessor templateProcessor
                )
            {
                _fileWriter = fileWriter;
                _namingConventionConverter = namingConventionConverter;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
            }

            public Task Handle(Request request, CancellationToken cancellationToken)
            {                
                var entityNamePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Entity);
                var entityNameCamelCase = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Entity);
                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name);
                var namePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Name);

                var template = _templateLocator.Get("GenerateResponsiveComponentCommand");
                var templateCss = _templateLocator.Get("GenerateResponsiveComponentCommand-css");
                var templateHtml = _templateLocator.Get("GenerateResponsiveComponentCommand-html");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ entityNamePascalCase }}", entityNamePascalCase },
                    { "{{ entityNameCamelCase }}", entityNameCamelCase },
                    { "{{ namespace }}", request.Namespace },
                    { "{{ rootNamespace }}", request.RootNamespace },
                    { "{{ nameSnakeCase }}", nameSnakeCase },
                    { "{{ namePascalCase }}", namePascalCase }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);
                var resultHtml = _templateProcessor.ProcessTemplate(templateHtml, tokens);
                var resultCss = _templateProcessor.ProcessTemplate(templateCss, tokens);

                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.ts", result);
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.html", resultHtml);
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.css", resultCss);

                return Task.CompletedTask;
            }
        }
    }
}
