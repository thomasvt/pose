using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pose.Framework.IoC
{
    public interface IModule
    {
        void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration);
    }
}
