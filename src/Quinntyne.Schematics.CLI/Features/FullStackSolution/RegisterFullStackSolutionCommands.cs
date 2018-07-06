using MediatR;
using Quinntyne.Schematics.CLI.Features.FullStackSolution.Section1;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution
{
    public class RegisterFullStackSolutionCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("sln", new Func<IOptions, IRequest>((options) => new GenerateSolutionCommand.Request(options)));
            dictionary.Add("class", new Func<IOptions, IRequest>((options) => new GenerateClassCommand.Request(options)));

            RegisterSection1Commands.Register(dictionary);
        }
    }
}
