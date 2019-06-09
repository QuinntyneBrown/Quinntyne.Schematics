using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;
using static System.IO.File;
using System.IO;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class GenerateIndexCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
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
                var lines = new List<string>();
                var fileName = $"{request.Directory}//index.ts";

                if (Exists(fileName)) Delete(fileName);

                foreach (var directory in Directory.GetDirectories(request.Directory))
                    lines.Add($"export * from \"./{directory.Split("\\")[directory.Split("\\").Length - 1]}\";");

                foreach (var file in Directory.GetFiles(request.Directory, "*.ts"))
                    if (!file.Contains(".spec.ts"))
                        lines.Add($"export * from \"./{Path.GetFileNameWithoutExtension(file)}\";");

                _fileWriter.WriteAllLines(fileName, lines.ToArray());
               
                return Task.CompletedTask;
            }
        }
    }
}
