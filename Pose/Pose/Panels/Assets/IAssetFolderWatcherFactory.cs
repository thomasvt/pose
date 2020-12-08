using System;
using System.IO;

namespace Pose.Panels.Assets
{
    public interface IAssetFolderWatcherFactory
    {
        FileSystemWatcher Create(string sourceFolder, Action onChange);
    }
}