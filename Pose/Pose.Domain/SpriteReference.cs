namespace Pose.Domain
{
    /// <summary>
    /// reference info to a sprite file.
    /// </summary>
    public readonly struct SpriteReference
    {
        public SpriteReference(string relativePath)
        {
            RelativePath = relativePath;
        }

        /// <summary>
        /// The file relative to the working folder.
        /// </summary>
        public string RelativePath { get; }
    }
}
