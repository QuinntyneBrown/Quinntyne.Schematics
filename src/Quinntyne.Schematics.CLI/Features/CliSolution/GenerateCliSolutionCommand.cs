using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static System.Diagnostics.ProcessWindowStyle;
using static System.Environment;

namespace Quinntyne.Schematics.CLI.Features.CliSolution
{
    public class GenerateCliSolutionCommand
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
                var workingDirectory = System.Environment.CurrentDirectory;

                var template = _templateLocator.Get("GenerateCliSolutionCommand");

                var tokens = new Dictionary<string, string>
                {
                    { "{{ rootNamespace }}", request.RootNamespace },
                    { "{{ currentDirectory }}", CurrentDirectory },
                };

                foreach (var arguments in _templateProcessor.ProcessTemplate(template, tokens))
                {
                    if (arguments.StartsWith("cd"))
                    {
                        var path = arguments.Split(" ")[1];

                        if (path == CurrentDirectory)
                            workingDirectory = CurrentDirectory;
                        else
                            workingDirectory = $"{CurrentDirectory}\\{path}";

                    }
                    else
                    {
                        new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                WindowStyle = Hidden,
                                FileName = "cmd.exe",
                                Arguments = $"/C {arguments}",
                                WorkingDirectory = workingDirectory
                            }
                        }.Start();

                        Thread.Sleep(1000);

                        if (arguments.Contains("classlib") ||
                            arguments.Contains("angular") ||
                            arguments.Contains("xunit") ||
                            arguments.Contains("webapi"))
                        {
                            _fileWriter.WriteAllLines($"{workingDirectory}//codeGeneratorSettings.json", new List<string> { JsonConvert.SerializeObject(new { request.RootNamespace }) }.ToArray());
                        }
                    }
                }

                return Task.CompletedTask;
            }
        }
    }
}
