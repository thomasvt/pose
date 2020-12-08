using System.IO;
using Microsoft.Xna.Framework.Content;
using Pose.Persistence;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public static class ContentManagerExtensions
    {
        /// <summary>
        /// Loads a Pose skeleton from a .pose file and all its required resources (sprites) for making <see cref="Skeleton"/> instances of the definition.
        /// </summary>
        public static Document LoadPoseDocument(this ContentManager content, string name)
        {
            var poseFile = Path.Combine(Path.GetFullPath(content.RootDirectory), name + ".pose");
            if (!File.Exists(poseFile))
                throw new FileNotFoundException($"Pose file not found: \"{poseFile}\".");

            return Document.Parser.ParseFrom(File.ReadAllBytes(poseFile));
        }
    }
}
