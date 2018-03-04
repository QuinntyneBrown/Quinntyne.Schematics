using MediatR;
using System;
using System.Collections.Generic;

namespace EventSourcing.CodeGenerator.CLI.Features.CodeGenerator
{
    public class RegisterCodeGeneratorCommands
    {
        public static void Register(Dictionary<string, Func<string[], IRequest>> dictionary)
        {
            dictionary.Add("code-gen", new Func<string[], IRequest>((args) => new GenerateCodeGeneratorCommand.Request(args)));
        }
    }
}
