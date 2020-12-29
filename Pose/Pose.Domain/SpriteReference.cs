using System;

namespace Pose.Domain
{
    /// <summary>
    /// reference info to a sprite file.
    /// </summary>
    public readonly struct SpriteReference
    : IEquatable<SpriteReference>
    {
        public SpriteReference(string relativePath)
        {
            RelativePath = relativePath;
        }

        /// <summary>
        /// The file relative to the working folder.
        /// </summary>
        public string RelativePath { get; }

        public bool Equals(SpriteReference other)
        {
            return RelativePath == other.RelativePath;
        }

        public override bool Equals(object obj)
        {
            return obj is SpriteReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (RelativePath != null ? RelativePath.GetHashCode() : 0);
        }
    }
}
