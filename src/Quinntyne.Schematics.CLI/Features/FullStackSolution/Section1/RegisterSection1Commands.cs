using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quinntyne.Schematics.CLI.Features.FullStackSolution.Section1
{
    public static class RegisterSection1Commands
    {
        public static void Register(Dictionary<string, Func<IOptions, IRequest>> dictionary)
        {
            dictionary.Add("1-1", new Func<IOptions, IRequest>((options) => new Section1Step01Command.Request(options)));
            dictionary.Add("1-2", new Func<IOptions, IRequest>((options) => new Section1Step02Command.Request(options)));
            dictionary.Add("1-3", new Func<IOptions, IRequest>((options) => new Section1Step03Command.Request(options)));
            dictionary.Add("1-4", new Func<IOptions, IRequest>((options) => new Section1Step04Command.Request(options)));
            dictionary.Add("1-5", new Func<IOptions, IRequest>((options) => new Section1Step05Command.Request(options)));
            dictionary.Add("1-6", new Func<IOptions, IRequest>((options) => new Section1Step06Command.Request(options)));
            dictionary.Add("1-7", new Func<IOptions, IRequest>((options) => new Section1Step07Command.Request(options)));
            dictionary.Add("1-8", new Func<IOptions, IRequest>((options) => new Section1Step08Command.Request(options)));
            dictionary.Add("1-9", new Func<IOptions, IRequest>((options) => new Section1Step09Command.Request(options)));
            dictionary.Add("1-10", new Func<IOptions, IRequest>((options) => new Section1Step10Command.Request(options)));            
        }
    }
}
