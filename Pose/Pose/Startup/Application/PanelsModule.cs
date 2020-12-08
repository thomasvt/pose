using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pose.Framework.IoC;
using Pose.Panels.Animations;
using Pose.Panels.Assets;
using Pose.Panels.Dopesheet;
using Pose.Panels.DrawOrder;
using Pose.Panels.Hierarchy;
using Pose.Panels.History;
using Pose.Panels.ModeSwitching;
using Pose.Panels.Properties;

namespace Pose.Startup.Application
{
    public class PanelsModule
    : IModule
    {
        public void Register(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            RegisterAssetPanel(serviceCollection);
            RegisterAnimationsPanel(serviceCollection);
            RegisterHierarchyPanel(serviceCollection);
            RegisterDrawOrderPanel(serviceCollection);
            RegisterPropertyPanel(serviceCollection);
            RegisterDopeSheetPanel(serviceCollection);
            RegisterHistoryPanel(serviceCollection);
            RegisterModeSwitchPanel(serviceCollection);
        }

        private static void RegisterAssetPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<AssetPanelViewModel>();

            serviceCollection.AddTransient<IAssetScanner, AssetScanner>();
            serviceCollection.AddTransient<IAssetFolderWatcherFactory, AssetFolderWatcherFactory>();
            serviceCollection.AddTransient<IAssetViewModelBuilder, AssetViewModelBuilder>();
            serviceCollection.AddTransient<IThumbnailLoader, ThumbnailLoader>();
        }

        private static void RegisterAnimationsPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<AnimationsPanelViewModel>();
        }

        private static void RegisterHierarchyPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<HierarchyPanelViewModel>();
        }

        private void RegisterDrawOrderPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<DrawOrderPanelViewModel>();
        }

        private void RegisterPropertyPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<PropertiesPanelViewModel>();
        }

        private void RegisterDopeSheetPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<DopesheetPanelViewModel>();
        }

        private void RegisterHistoryPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<HistoryPanelViewModel>();
        }

        private static void RegisterModeSwitchPanel(ServiceCollection serviceCollection)
        {
            serviceCollection.AddWithFactory<ModeSwitchPanelViewModel>();
        }
    }
}
