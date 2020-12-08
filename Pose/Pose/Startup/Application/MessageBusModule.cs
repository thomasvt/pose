using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;
using Pose.Framework.Messaging;

namespace Pose.Startup.Application
{
    public class MessageBusModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddSingleton(MessageBus.Default); // the app and the domain driving the app use the Default
        }
    }
}
