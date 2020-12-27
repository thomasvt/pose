using System.IO;
using Microsoft.Xna.Framework.Content;
using Pose.Persistence;

namespace Pose.Runtime.MonoGameDotNetCore
{
    public static class ContentManagerExtensions
    {
        /// <summary>
        /// Loads a Pose document containing a skeleton and its animation(s) from a file created the Pose editor. Also automatically loads necessary resources (sprites).
        /// </summary>
        public static Document LoadPoseDocument(this ContentManager content, string filename)
        {
            var poseFile = Path.Combine(Path.GetFullPath(content.RootDirectory), filename);
            if (!File.Exists(poseFile))
                throw new FileNotFoundException($"Pose file not found: \"{poseFile}\".");

            return Document.Parser.ParseFrom(File.ReadAllBytes(poseFile));
        }
    }
}
