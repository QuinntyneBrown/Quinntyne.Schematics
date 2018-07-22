using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;
using System.Linq;
using System;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution
{
    public class GenerateFileCommand
    {
        public class Request : Options, IRequest, ICodeGeneratorCommandRequest
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
                foreach (var name in request.Name.Split(","))
                {
                    var templateName = name;
                    var template = _templateLocator.Get(templateName.Replace(".",""));

                    var tokens = new Dictionary<string, string>
                    {
                        { "{{ rootNamespace }}", request.RootNamespace }
                    };

                    var result = _templateProcessor.ProcessTemplate(template, tokens);

                    var relativePath = $"{result[0]}{name.Split("_")[0]}";

                    relativePath.Replace(@"\", "//");

                    Console.WriteLine(relativePath);

                    result = result.Skip(1).ToArray();

                    _fileWriter.WriteAllLines($"{request.SolutionDirectory}//{relativePath}", result);

                }

                return Task.CompletedTask;
            }
        }
    }
}
