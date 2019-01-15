using MediatR;
using System;
using System.Collections.Generic;

namespace Quinntyne.Schematics.CLI.Features.BasicApi
{
    public class RegisterBasicApiCommands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            //dictionary.Add("dto", new Func<IOptions, IRequest>((options) => new GenerateApiModelCommand.Request(options)));
            //dictionary.Add("model", new Func<IOptions, IRequest>((options) => new GenerateModelCommand.Request(options)));
            //dictionary.Add("create", new Func<IOptions, IRequest>((options) => new GenerateCreateCommand.Request(options)));
            //dictionary.Add("update", new Func<IOptions, IRequest>((options) => new GenerateUpdateCommand.Request(options)));
            //dictionary.Add("saved", new Func<IOptions, IRequest>((options) => new GenerateSavedEventCommand.Request(options)));
            //dictionary.Add("get", new Func<IOptions, IRequest>((options) => new GenerateGetQueryCommand.Request(options)));
            //dictionary.Add("getbyid", new Func<IOptions, IRequest>((options) => new GenerateGetByIdQueryCommand.Request(options)));
            //dictionary.Add("controller", new Func<IOptions, IRequest>((options) => new GenerateControllerCommand.Request(options)));

        }
    }
}
