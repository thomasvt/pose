using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Domain.Editor;
using Pose.Framework.IoC;
using Pose.Popups.ExportSpritesheets;
using Pose.Spritesheets;

namespace Pose.Startup.Application
{
    public class SpritesheetModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            // for exporting animations to spritesheet.
            serviceCollection.AddWithFactory<ExportSpritesheetViewModel>();
            serviceCollection.AddTransient<ISpriteProducer, SpriteProducer>();

            // for exporting scene sprites onto spritesheets.
            serviceCollection.AddTransient<ISceneSpritesheetExporter, SceneSpritesheetExporter>();
        }
    }
}
