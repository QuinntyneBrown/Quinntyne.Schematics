using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.Angular
{
    public class RegisterAngularCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("ng", new Func<IOptions, IRequest>((options) => new GenerateComponentCommand.Request(options)));
        }
    }
}
