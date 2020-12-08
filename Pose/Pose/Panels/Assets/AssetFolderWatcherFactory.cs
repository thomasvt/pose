using System;
using System.IO;

namespace Pose.Panels.Assets
{
    public class AssetFolderWatcherFactory : IAssetFolderWatcherFactory
    {
        public FileSystemWatcher Create(string sourceFolder, Action onChange)
        {
            var watcher = new FileSystemWatcher(sourceFolder)
            {
                Filters = { "*.png" },
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.FileName |
                               NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Size
            };

            watcher.Changed += (sender, args) => onChange();
            watcher.Created += (sender, args) => onChange();
            watcher.Deleted += (sender, args) => onChange();
            watcher.Renamed += (sender, args) => onChange();

            watcher.EnableRaisingEvents = true;

            return watcher;
        }
    }
}
