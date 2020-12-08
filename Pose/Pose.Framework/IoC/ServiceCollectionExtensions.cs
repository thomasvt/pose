using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pose.Framework.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWithFactory<TService>(this IServiceCollection services)
            where TService : class
        {
            AddWithFactory<TService, TService>(services);
        }

        public static void AddWithFactory<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddSingleton<Func<TService>>(x => x.GetService<TService>);
        }

        public static void RegisterAssemblyModules(this ServiceCollection serviceCollection, Assembly assembly, IConfigurationRoot configuration)
        {
            foreach (var moduleType in assembly.GetExportedTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t)))
            {
                var module = (IModule)Activator.CreateInstance(moduleType);
                module.Register(serviceCollection, configuration);
            }
        }
    }
}
