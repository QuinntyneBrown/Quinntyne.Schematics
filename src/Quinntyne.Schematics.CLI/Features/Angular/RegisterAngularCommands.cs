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
            dictionary.Add("ng-feature", new Func<IOptions, IRequest>((options) => new GenerateNgFeatureCommand.Request(options)));
            dictionary.Add("ng-validator", new Func<IOptions, IRequest>((options) => new GenerateValidatorCommand.Request(options)));
            dictionary.Add("ng-scenario", new Func<IOptions, IRequest>((options) => new GenerateNgScenarioCommand.Request(options)));
            dictionary.Add(".", new Func<IOptions, IRequest>((options) => new GenerateIndexCommand.Request(options)));
            dictionary.Add("ng-i", new Func<IOptions, IRequest>((options) => new GenerateInterfaceCommand.Request(options)));
        }
    }
}
