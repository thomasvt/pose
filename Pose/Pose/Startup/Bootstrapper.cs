using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;

namespace Pose.Startup
{
    internal class Bootstrapper
    {
        public IServiceProvider Start()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();

            serviceCollection.RegisterAssemblyModules(Assembly.GetExecutingAssembly(), configuration);

            return serviceCollection.BuildServiceProvider();
        }
    }
}
