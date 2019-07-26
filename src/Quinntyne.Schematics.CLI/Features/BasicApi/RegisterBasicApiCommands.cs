using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.BasicApi
{
    public class RegisterBasicApiCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("dto", new Func<IOptions, IRequest>((options) => new GenerateDtoCommand.Request(options)));
            dictionary.Add("model", new Func<IOptions, IRequest>((options) => new GenerateModelCommand.Request(options)));
            dictionary.Add("feature", new Func<IOptions, IRequest>((options) => new GenerateFeatureCommand.Request(options)));
            dictionary.Add("upsert", new Func<IOptions, IRequest>((options) => new GenerateUpsertCommand.Request(options)));
            dictionary.Add("get", new Func<IOptions, IRequest>((options) => new GenerateGetQueryCommand.Request(options)));
            dictionary.Add("remove", new Func<IOptions, IRequest>((options) => new GenerateRemoveCommand.Request(options)));
            dictionary.Add("getbyid", new Func<IOptions, IRequest>((options) => new GenerateGetByIdQueryCommand.Request(options)));
            dictionary.Add("controller", new Func<IOptions, IRequest>((options) => new GenerateControllerCommand.Request(options)));
            dictionary.Add("command", new Func<IOptions, IRequest>((options) => new GenerateCommandCommand.Request(options)));
            dictionary.Add("tests", new Func<IOptions, IRequest>((options) => new GenerateCommandTestsCommand.Request(options)));
        }
    }
}
