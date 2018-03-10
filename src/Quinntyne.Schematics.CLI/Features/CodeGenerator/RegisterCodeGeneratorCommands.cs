using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.CodeGenerator
{
    public class RegisterCodeGeneratorCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("code-gen", new Func<IOptions, IRequest>((options) => new GenerateCodeGeneratorCommand.Request(options)));
        }
    }
}
