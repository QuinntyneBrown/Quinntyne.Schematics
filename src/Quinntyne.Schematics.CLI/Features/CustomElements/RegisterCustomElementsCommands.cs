using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.CustomElements
{
    public class RegisterCustomElementsCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("ce", new Func<IOptions, IRequest>((options) => new GenerateCustomElementCommand.Request(options)));
        }
    }
}
