using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
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
            public Task Handle(Request message, CancellationToken cancellationToken)
            {
                Console.WriteLine("works?");

                Console.ReadLine();

                return Task.CompletedTask;
            }
        }
    }
}
