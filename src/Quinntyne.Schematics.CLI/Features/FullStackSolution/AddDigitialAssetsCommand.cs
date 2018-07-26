using FluentValidation;
using MediatR;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution
{
    public class AddDigitialAssetsCommand
    {
        public class Request : Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {
                Entity = options.Entity;
                Directory = options.Directory;
                Namespace = options.Namespace;
                RootNamespace = options.RootNamespace;
                Options = options;
            }

            public dynamic Settings { get; set; }
            public IOptions Options { get; set; }
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
            private readonly IMediator _mediator;

            public Handler(
                IMediator mediator
                )
            {
                _mediator = mediator;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var classDir = $@"{request.SolutionDirectory}\src\{request.RootNamespace}.API\Features\DigitalAssets\";
                var fileDir = $@"{request.SolutionDirectory}src\{request.RootNamespace}.SPA\ClientApp\src\app\digital-assets\";

                
                if (!Directory.Exists(classDir)) Directory.CreateDirectory(classDir);

                if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
                

                var classes = string.Join(",", new List<string>() {
                    "DigitalAsset",
                    "DigitalAssetCreated",
                    "MultipartRequestHelper",
                    "StreamHelper",
                    "DigitalAssetDto",
                    "DigitalAssetsController",
                    "GetDigitalAssetByIdQuery",
                    "GetDigitalAssetsByIdsQuery",
                    "UploadDigitalAssetCommand"
                }.ToArray());

                var files = string.Join(",", new List<string>() {
                    "digital-asset.model.ts",
                    "digital-asset.service.ts",
                    "digital-assets.module.ts",
                    "digital-asset-url-input.component.css",
                    "digital-asset-url-input.component.ts",
                    "digital-asset-url-input.component.html"
                }.ToArray());

                await _mediator.Send(new GenerateClassCommand.Request(request.Options)
                {
                    ClassName = classes,
                    SolutionDirectory = request.SolutionDirectory
                });

                await _mediator.Send(new GenerateFileCommand.Request(request.Options)
                {
                    Name = files,
                    SolutionDirectory = request.SolutionDirectory
                });
            }
        }
    }
}
