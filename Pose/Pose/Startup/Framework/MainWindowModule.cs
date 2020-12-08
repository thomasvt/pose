using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;
using Pose.Shell;

namespace Pose.Startup.Framework
{
    public class MainWindowModule
    : IModule
    {
        public void Register(ServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddTransient(typeof(ShellWindow));
        }
    }
}
