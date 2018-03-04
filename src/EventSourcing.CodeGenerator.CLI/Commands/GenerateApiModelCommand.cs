using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EventSourcing.CodeGenerator.CLI.Commands
{
    public class GenerateApiModelCommand
    {
        public class Options
        {

        }

        public class Request: IRequest
        {
            public Request(string[] args)
            {

            }
        }

        public class Handler : IRequestHandler<Request>
        {
            public Task Handle(Request message, CancellationToken cancellationToken)
            {
                Console.WriteLine("works?");

                Console.ReadLine();

                return Task.CompletedTask;
            }
        }
    }
}
