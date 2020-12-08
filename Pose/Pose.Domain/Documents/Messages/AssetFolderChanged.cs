using System;
using System.Collections.Generic;
using System.Text;

namespace Pose.Domain.Documents.Messages
{
    public class AssetFolderChanged
    {
        public string Path { get; }

        public AssetFolderChanged(string path)
        {
            Path = path;
        }
    }
}
