using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;
using Pose.Shell;

namespace Pose.Startup.Application
{
    public class ShellModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddWithFactory<ShellViewModel>();
        }
    }
}
