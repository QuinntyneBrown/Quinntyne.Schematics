using Microsoft.Extensions.DependencyInjection;
using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using Quinntyne.Schematics.Extensions;
using Quinntyne.Schematics.CLI.Features.EventSourcing;
using Quinntyne.Schematics.CLI.Features.CodeGenerator;
using Microsoft.Extensions.Logging;
using CommandLine;
using Quinntyne.Schematics.CLI.Features.Testing;
using Quinntyne.Schematics.CLI.Features.Angular;
using Quinntyne.Schematics.CLI.Features.AngularComponents;
using AutoMapper;
using Quinntyne.Schematics.Infrastructure.Services;
using Quinntyne.Schematics.CLI.Features.FullStackSolution;
using Quinntyne.Schematics.CLI.Features.CliSolution;
using Quinntyne.Schematics.CLI.Features.CustomElements;
using Quinntyne.Schematics.CLI.Features.BasicApi;

namespace Quinntyne.Schematics.CLI
{
    public class Program
    {
        private readonly IDictionary<string, Func<IOptions, IRequest>> _commands;
        private readonly IMediator _mediator;
        private readonly INamespaceProvider _namespaceProvider;

        static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();
            var commands = BuildCommandRequestDictionary();

            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            var mediator = serviceProvider.GetService<IMediator>();

            var namespaceProvider = serviceProvider.GetService<INamespaceProvider>();

            new Program(mediator, commands, namespaceProvider).ProcessArgs(args);
        }

        public Program(IMediator mediator, IDictionary<string, Func<IOptions, IRequest>> commands, INamespaceProvider namespaceProvider)
        {
            _mediator = mediator;
            _commands = commands;
            _namespaceProvider = namespaceProvider;
        }

        public int ProcessArgs(string[] args)
        {
            int lastArg = 0;

            var command = args[lastArg];

            var appArgs = (lastArg + 1) >= args.Length ? Enumerable.Empty<string>() : args.Skip(lastArg + 1).ToArray();

            Func<IOptions, IRequest> requestFunc;

            var retrieveRequest = _commands.TryGetValue(command, out requestFunc);

            if (retrieveRequest)
            {
                IOptions options = new Options();

                Parser.Default.ParseArguments<Options>(args)
                    .MapResult(x =>
                    {
                        options.Name = x.Name;
                        options.Entity = x.Entity;
                        options.Domain = x.Domain;
                        options.Directory = x.Directory;
                        options.Namespace = x.Namespace;
                        options.RootNamespace = x.RootNamespace;
                        options.ServiceName = x.ServiceName;
                        options.ClassName = x.ClassName;
                        return 1;
                    }, x => 0);

                var request = requestFunc(options);

                (request as IOptions).ClassName = options.ClassName;
                (request as IOptions).Name = options.Name;
                (request as IOptions).SolutionDirectory = System.IO.Path.GetDirectoryName(_namespaceProvider.GetSolutionPath(options.Directory));
                
                _mediator.Send(request).Wait();
            }

            return 1;
        }

        private static bool IsArg(string candidate, string longName) => IsArg(candidate, shortName: null, longName: longName);

        private static bool IsArg(string candidate, string shortName, string longName)
        {
            return (shortName != null && candidate.Equals("-" + shortName)) || (longName != null && candidate.Equals("--" + longName));
        }

        static ServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();

            services.UseCodeGenerator();

            services.UseCodeGeneratorFeature();

            services.AddMediatR(typeof(Program));
            
            return services.BuildServiceProvider();            
        }


        public static Dictionary<string, Func<IOptions, IRequest>> BuildCommandRequestDictionary()
        {
            var dictionary = new Dictionary<string, Func<IOptions, IRequest>>();

            RegisterBasicApiCommands.Register(dictionary);
            RegisterCodeGeneratorCommands.Register(dictionary);
            RegisterEventSourcingCommands.Register(dictionary);
            RegisterTestingCommands.Register(dictionary);
            RegisterAngularCommands.Register(dictionary);
            RegisterAngularComponentCommands.Register(dictionary);
            RegisterFullStackSolutionCommands.Register(dictionary);
            RegisterCliSolutionCommands.Register(dictionary);
            RegisterCustomElementsCommands.Register(dictionary);
            return dictionary;
        }
    }

    public interface IOptions
    {
        [Option("domain", Required = false, HelpText = "Domain")]
        string Domain { get; set; }
        [Option("name", Required = false, HelpText = "Name")]
        string Name { get; set; }
        [Option("entity", Required = false, HelpText = "Entity")]
        string Entity { get; set; }
        [Option("class", Required = false, HelpText = "Class")]
        string ClassName { get; set; }
        string Directory { get; set; }
        string SolutionDirectory { get; set; }
        string Namespace { get; set; }
        string RootNamespace { get; set; }
        [Option("serviceName", Required = false, HelpText = "Service Name")]
        string ServiceName { get; set; }
    }

    public class Options : IOptions
    {
        public string Domain { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public string Entity { get; set; }
        public string Namespace { get; set; }
        public string RootNamespace { get; set; }
        public string Directory { get; set; } = System.Environment.CurrentDirectory;
        public string SolutionDirectory { get; set; }
        public string ServiceName { get; set; }
    }
}
