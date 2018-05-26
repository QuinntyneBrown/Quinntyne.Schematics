using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.AngularComponents
{
    public class RegisterAngularComponentCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("ng-list", new Func<IOptions, IRequest>((options) => new GenerateListComponentCommand.Request(options)));
        }
    }
}
