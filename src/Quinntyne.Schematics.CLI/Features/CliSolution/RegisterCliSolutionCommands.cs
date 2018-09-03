using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.CliSolution
{
    public class RegisterCliSolutionCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("cli", new Func<IOptions, IRequest>((options) => new GenerateCliSolutionCommand.Request(options)));
        }
    }
}
