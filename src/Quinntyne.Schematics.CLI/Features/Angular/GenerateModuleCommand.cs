using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class GenerateModuleCommand
    {
        public class Request : Options, IRequest, ICodeGeneratorCommandRequest
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
                RuleFor(request => request.Name).NotNull();
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
                var nameSnakeCase = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name);
                var namePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Name);
                var nameSnakeCasePlural = _namingConventionConverter.Convert(NamingConvention.SnakeCase, request.Name, true);
                var namePascalCasePlural = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Name, true);
                var template = request.Name.ToLower() == "material" ?
                    _templateLocator.Get("GenerateMaterialModuleCommand")
                    : _templateLocator.Get("GenerateModuleCommand");


                var tokens = new Dictionary<string, string>
                {
                    { "{{ namePascalCase }}", namePascalCase },
                    { "{{ namePascalCasePlural }}", namePascalCasePlural },
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);
                var filename = request.Name.ToLower() == "material" ? "material" : namePascalCasePlural;
                _fileWriter.WriteAllLines($"{request.Directory}//{filename}.module.ts", result);

                return Task.CompletedTask;
            }
        }
    }
}
