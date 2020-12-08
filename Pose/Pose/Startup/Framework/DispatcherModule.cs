using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework;
using Pose.Framework.IoC;

namespace Pose.Startup.Framework
{
    public class DispatcherModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddSingleton<IUiThreadDispatcher>(new UiThreadDispatcher());
        }
    }
}
