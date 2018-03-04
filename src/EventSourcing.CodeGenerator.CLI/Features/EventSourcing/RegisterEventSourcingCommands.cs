using MediatR;
using System;
using System.Collections.Generic;

namespace EventSourcing.CodeGenerator.CLI.Features.EventSourcing
{
    public class RegisterEventSourcingCommands
    {
        public static void Register(Dictionary<string, Func<string[], IRequest>> dictionary)
        {
            dictionary.Add("api-model", new Func<string[], IRequest>((args) => new GenerateApiModelCommand.Request(args)));
            dictionary.Add("model", new Func<string[], IRequest>((args) => new GenerateModelCommand.Request(args)));
        }
    }
}
