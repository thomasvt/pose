namespace Pose.DomainModel
{
    /// <summary>
    /// reference info to a sprite file.
    /// </summary>
    public readonly struct SpriteReference
    {
        public SpriteReference(string absolutePath, string relativePath)
        {
            AbsolutePath = absolutePath;
            RelativePath = relativePath;
        }

        /// <summary>
        /// The current absolute file path. (while open in editor only)
        /// </summary>
        public string AbsolutePath { get; }
        /// <summary>
        /// The file relative to the working folder.
        /// </summary>
        public string RelativePath { get; }
    }
}
