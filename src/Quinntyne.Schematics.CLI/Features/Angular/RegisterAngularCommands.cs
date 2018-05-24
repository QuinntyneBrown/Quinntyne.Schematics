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
            dictionary.Add("ng-module", new Func<IOptions, IRequest>((options) => new GenerateModuleCommand.Request(options)));
            dictionary.Add("ng-model", new Func<IOptions, IRequest>((options) => new GenerateModelCommand.Request(options)));
            dictionary.Add("ng-overlay-ref", new Func<IOptions, IRequest>((options) => new GenerateOverlayRefCommand.Request(options)));
            dictionary.Add("ng-service", new Func<IOptions, IRequest>((options) => new GenerateServiceCommand.Request(options)));
            dictionary.Add("ng-overlay", new Func<IOptions, IRequest>((options) => new GenerateOverlayServiceCommand.Request(options)));
        }
    }
}
