using EventSourcing.CodeGenerator.Infrastructure.Services;
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
        }
    }
}
