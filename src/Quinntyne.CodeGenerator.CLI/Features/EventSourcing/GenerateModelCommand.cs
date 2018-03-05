using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Quinntyne.CodeGenerator.Infrastructure.Interfaces;
using Quinntyne.CodeGenerator.Infrastructure.Services;
using MediatR;

namespace Quinntyne.CodeGenerator.CLI.Features.EventSourcing
{
    public class GenerateModelCommand
    {
        public class Options
        {
            [Option("entity", Required = false, HelpText = "Entity")]
            public string Entity { get; set; }

            public string Directory = System.Environment.CurrentDirectory;

            public string Namespace { get; set; }

            public string RootNamespace { get; set; }
        }

        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(string[] args)
            {                
                Parser.Default.ParseArguments<Options>(args)
                    .MapResult(x => {
                        Entity = x.Entity;
                        Directory = x.Directory;
                        Namespace = x.Namespace;
                        RootNamespace = x.RootNamespace;
                        return 1;
                    }, x => 0);
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
                
                _fileWriter.WriteAllLines($"{request.Directory}//GenerateModelCommand.cs", result);
               
                return Task.CompletedTask;
            }
        }
    }
}
