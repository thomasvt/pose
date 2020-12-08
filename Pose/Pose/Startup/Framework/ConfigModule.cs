using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Application;
using Pose.Framework.IoC;

namespace Pose.Startup.Framework
{
    public class ConfigModule
    : IModule
    {
        public void Register(ServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        }
    }
}
