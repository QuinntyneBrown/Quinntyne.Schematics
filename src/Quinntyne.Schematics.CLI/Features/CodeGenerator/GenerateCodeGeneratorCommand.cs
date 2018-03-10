using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;

namespace Quinntyne.Schematics.CLI.Features.CodeGenerator
{
    public class GenerateCodeGeneratorCommand
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.Name).NotNull();
            }
        }

        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request()
            {

            }

            public Request(IOptions options)
            {
                Name = options.Name;
                Namespace = options.Namespace;
                RootNamespace = options.RootNamespace;
            }

            public dynamic Settings { get; set; }
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
                var template = _templateLocator.Get("GenerateCodeGeneratorCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ name }}", request.Name },
                    { "{{ *namespace }}", request.Namespace },
                    { "{{ *rootNamespace }}", request.RootNamespace }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);
                
                _fileWriter.WriteAllLines($"{request.Directory}//{request.Name}.cs", result);
                _fileWriter.WriteAllLines($"{request.Directory}//{request.Name}.txt", new string[0]);

                return Task.CompletedTask;
            }
        }
    }
}
