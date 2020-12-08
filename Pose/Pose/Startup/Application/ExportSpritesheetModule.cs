using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;
using Pose.Popups.ExportSpritesheets;

namespace Pose.Startup.Application
{
    public class ExportSpritesheetModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddWithFactory<ExportSpritesheetViewModel>();
            serviceCollection.AddTransient<ISpriteProducer, SpriteProducer>();
        }
    }
}
