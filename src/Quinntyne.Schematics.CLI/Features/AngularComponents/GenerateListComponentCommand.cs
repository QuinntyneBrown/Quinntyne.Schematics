using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.AngularComponents
{
    public class GenerateListComponentCommand
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
                var entityNameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Entity);
                var entityNameCamelCasePlural = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Entity, true);

                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name);
                var nameCamelCasePlural = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Name, true);
                var namePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Name);

                var template = _templateLocator.Get("GenerateListComponentCommand");
                var templateCss = _templateLocator.Get("GenerateListComponentCssCommand");
                var templateHtml = _templateLocator.Get("GenerateListComponentHtmlCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ entityNamePascalCase }}", entityNamePascalCase },
                    { "{{ entityNameCamelCase }}", entityNameCamelCase },
                    { "{{ entityNameSnakeCase }}", entityNameSnakeCase },
                    { "{{ entityNameCamelCasePlural }}", entityNameCamelCasePlural },

                    { "{{ nameSnakeCase }}", nameSnakeCase },
                    { "{{ namePascalCase }}", namePascalCase },
                    { "{{ nameCamelCasePlural }}", nameCamelCasePlural },

                    { "{{ namespace }}", request.Namespace },
                    { "{{ rootNamespace }}", request.RootNamespace }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);
                var resultCss = _templateProcessor.ProcessTemplate(templateCss, tokens);
                var resultHtml = _templateProcessor.ProcessTemplate(templateHtml, tokens);

                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.ts", result);
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.css", resultCss);
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.html", resultHtml);

                return Task.CompletedTask;
            }
        }
    }
}
