using EventSourcing.CodeGenerator.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventSourcing.CodeGenerator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseEventSourcingCodeGenerator(this IServiceCollection services)
        {
            services.AddSingleton<INamingConventionConverter, NamingConventionConverter>();
        }
    }
}
