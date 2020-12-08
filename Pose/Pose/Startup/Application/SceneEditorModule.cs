using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;
using Pose.SceneEditor;
using Pose.SceneEditor.ToolBar;

namespace Pose.Startup.Application
{
    public class SceneEditorModule
        : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddTransient<SceneEditorViewModel>();
            serviceCollection.AddTransient<ViewportToolBarViewModel>();

            serviceCollection.AddSingleton<SpriteBitmapStore>();
        }
    }
}
