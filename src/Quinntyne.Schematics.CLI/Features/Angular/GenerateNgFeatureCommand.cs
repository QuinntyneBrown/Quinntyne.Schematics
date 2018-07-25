using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quinntyne.Schematics.Infrastructure.Interfaces;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using FluentValidation;
using System;
using System.Runtime.Serialization;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class GenerateNgFeatureCommand
    {
        public class Request: Options, IRequest, ICodeGeneratorCommandRequest
        {
            public Request(IOptions options)
            {
                Name = options.Name;
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
            private readonly IFileWriter _fileWriter;
            private readonly ITemplateLocator _templateLocator;
            private readonly ITemplateProcessor _templateProcessor;
            private readonly INamingConventionConverter _namingConventionConverter;
            private readonly IMediator _mediator;

            public Handler(
                IFileWriter fileWriter,
                INamingConventionConverter namingConventionConverter,
                ITemplateLocator templateLocator, 
                ITemplateProcessor templateProcessor,
                IMediator mediator
                )
            {
                _fileWriter = fileWriter;
                _namingConventionConverter = namingConventionConverter;
                _templateProcessor = templateProcessor;
                _templateLocator = templateLocator;
                _mediator = mediator;
            }

            public async Task Handle(Request request, CancellationToken cancellationToken)
            {

                await _mediator.Send(CreateRequest<GenerateModuleCommand.Request>(
                    request.Name,
                    request.Directory,
                    request.Namespace,
                    request.RootNamespace,
                    request.Entity));

                await _mediator.Send(CreateRequest<GenerateModelCommand.Request>(
                    request.Name,
                    request.Directory,
                    request.Namespace,
                    request.RootNamespace,
                    request.Name));

                await _mediator.Send(CreateRequest<GenerateServiceCommand.Request>(
                    request.Name,
                    request.Directory,
                    request.Namespace,
                    request.RootNamespace,
                    request.Name));
            }

            public T CreateRequest<T>(string name, string directory, string @namespace, string rootNamespace, string entity) where T : IOptions
            {
                var request = (T)FormatterServices.GetUninitializedObject(typeof(T));
                request.Entity = entity;                
                request.Name = name;
                request.Directory = directory;
                request.Namespace = @namespace;
                request.RootNamespace = rootNamespace;
                return request;
            }
        }
    }
}
