using Quinntyne.Schematics.Infrastructure.Behaviours;
using Quinntyne.Schematics.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Quinntyne.Schematics.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseCodeGenerator(this IServiceCollection services)
        {
            services.AddSingleton<INamingConventionConverter, NamingConventionConverter>();
            services.AddSingleton<IFileWriter, FileWriter>();
            services.AddSingleton<ITemplateProcessor, TemplateProcessor>();
            services.AddSingleton<ITemplateLocator, TemplateLocator>();
            services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
            services.AddSingleton<INamespaceProvider, NamespaceProvider>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NamespaceResolverBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }
    }
}
