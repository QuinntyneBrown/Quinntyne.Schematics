using MediatR;
using Quinntyne.Schematics.CLI.DomainEvents;
using Quinntyne.Schematics.CLI.Features.EventSourcing;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quinntyne.Schematics.CLI.ProcessManagers
{
    public class EventSourcingGenerateModelProcessManager: INotificationHandler<EventSourcingModelCreated>
    {
        private readonly IMediator _mediator;
        public EventSourcingGenerateModelProcessManager(IMediator mediator) {
            _mediator = mediator;
        }

        public async Task Handle(EventSourcingModelCreated notification, CancellationToken cancellationToken)
        {            
            foreach(var @event in new string[3] {"Created","NameChanged","Removed" })
            {
                var request = CreateEventRequest<GenerateDomainEventCommand.Request>(notification.SolutionDirectory, notification.Entity, @event,notification.RootNamespace);
                await _mediator.Send(request);
            }
        }

        public T CreateEventRequest<T>(string solutionDirectory, string entity, string eventName, string rootNamespace) where T: IOptions {
            var request = (T)FormatterServices.GetUninitializedObject(typeof(T));
            request.Directory = $"{solutionDirectory}//src//{rootNamespace}.Core//DomainEvents";
            request.Entity = entity;
            request.RootNamespace = request.RootNamespace;
            request.Namespace = $"{rootNamespace}.Core.DomainEvents";
            request.Name = $"{entity}{eventName}";
            return request;
        }
    }
}
