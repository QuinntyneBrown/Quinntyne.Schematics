using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.CodeGenerator.CLI.Features.EventSourcing
{
    public class RegisterEventSourcingCommands
    {
        public static void Register(Dictionary<string, Func<string[], IRequest>> dictionary)
        {
            dictionary.Add("api-model", new Func<string[], IRequest>((args) => new GenerateApiModelCommand.Request(args)));
            dictionary.Add("model", new Func<string[], IRequest>((args) => new GenerateModelCommand.Request(args)));
            dictionary.Add("save-command", new Func<string[], IRequest>((args) => new GenerateSaveCommand.Request(args)));
            dictionary.Add("get-query", new Func<string[], IRequest>((args) => new GenerateGetQueryCommand.Request(args)));
            dictionary.Add("getbyid-query", new Func<string[], IRequest>((args) => new GenerateGetByIdQueryCommand.Request(args)));
            dictionary.Add("feature", new Func<string[], IRequest>((args) => new GenerateFeatureCommand.Request(args)));
            dictionary.Add("controller", new Func<string[], IRequest>((args) => new GenerateControllerCommand.Request(args)));
        }
    }
}
