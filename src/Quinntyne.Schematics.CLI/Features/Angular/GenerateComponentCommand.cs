using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class GenerateComponentCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {                
                Name = options.Name;
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
                var namePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Name);
                var nameCamelCase = _namingConventionConverter.Convert(NamingConvention.CamelCase, request.Name);
                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name);

                var template = _templateLocator.Get("GenerateComponentCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ namespace }}", request.Namespace },
                    { "{{ rootNamespace }}", request.RootNamespace },
                    { "{{ nameSnakeCase }}", nameSnakeCase },
                    { "{{ prefix }}", "app" },
                    {"{{ namePascalCase }}",namePascalCase }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);

                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.ts", result);
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.html", new string[0]);
                _fileWriter.WriteAllLines($"{request.Directory}//{nameSnakeCase}.component.css", new string[0]);

                return Task.CompletedTask;
            }
        }
    }
}
