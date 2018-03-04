using EventSourcing.CodeGenerator.Infrastructure.Behaviours;
using EventSourcing.CodeGenerator.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.CodeGenerator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseEventSourcingCodeGenerator(this IServiceCollection services)
        {
            services.AddSingleton<INamingConventionConverter, NamingConventionConverter>();
            services.AddSingleton<IFileWriter, FileWriter>();
            services.AddSingleton<ITemplateProcessor, TemplateProcessor>();
            services.AddSingleton<ITemplateRepository, TemplateRepository>();
            services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
            services.AddSingleton<INamespaceProvider, NamespaceProvider>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NamespaceResolverBehaviour<,>));
        }
    }
}
