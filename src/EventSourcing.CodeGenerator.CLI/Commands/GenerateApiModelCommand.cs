using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using EventSourcing.CodeGenerator.Infrastructure.Services;
using MediatR;

namespace EventSourcing.CodeGenerator.CLI.Commands
{
    public class GenerateApiModelCommand
    {
        public class Options
        {
            [Option("entity", Required = false, HelpText = "Entity")]
            public string Entity { get; set; }

            public string Directory = System.Environment.CurrentDirectory;
        }

        public class Request: IRequest
        {
            public Request(string[] args)
            {                
                Parser.Default.ParseArguments<Options>(args)
                    .MapResult(x => { Options = x; return 1; }, x => 0);
            }

            public Options Options { get; set; }

        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly IFileWriter _fileWriter;
            private readonly ITemplateRepository _templateRepository;
            private readonly ITemplateProcessor _templateProcessor;
            private readonly INamingConventionConverter _namingConventionConverter;

            public Handler(
                IFileWriter fileWriter,
                INamingConventionConverter namingConventionConverter,
                ITemplateRepository templateRepository, 
                ITemplateProcessor templateProcessor)
            {
                _fileWriter = fileWriter;
                _namingConventionConverter = namingConventionConverter;
                _templateProcessor = templateProcessor;
                _templateRepository = templateRepository;
            }

            public Task Handle(Request request, CancellationToken cancellationToken)
            {
                var entityNamePascalCase = _namingConventionConverter.Convert(NamingConvention.PascalCase, request.Options.Entity);

                var template = _templateRepository.Get("GenerateApiModelCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ entityNamePascalCase }}", entityNamePascalCase }
                };

                var result = _templateProcessor.ProcessTemplate(template, tokens);
                
                _fileWriter.WriteAllLines($"{request.Options.Directory}//{entityNamePascalCase}ApiModel.cs", result);
               
                return Task.CompletedTask;
            }
        }
    }
}
