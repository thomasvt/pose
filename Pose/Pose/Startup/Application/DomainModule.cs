using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Domain.Editor;
using Pose.Framework.IoC;

namespace Pose.Startup.Application
{
    public class DomainModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddSingleton<Editor, Editor>();
        }
    }
}
