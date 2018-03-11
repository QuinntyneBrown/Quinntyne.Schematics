using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.Testing
{
    public class RegisterTestingCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("scenarios", new Func<IOptions, IRequest>((options) => new GenerateTestScenariosCommand.Request(options)));
            dictionary.Add("scenario-base", new Func<IOptions, IRequest>((options) => new GenerateTestScenarioBaseCommand.Request(options)));
        }
    }
}
