using Microsoft.Xna.Framework;
using Pose.Runtime.MonoGameDotNetCore.QuadRendering;
using Pose.Runtime.MonoGameDotNetCore.Skeletons;

namespace Pose.Runtime.MonoGameDotNetCore.Animations
{
    internal struct RTNode
    {
        public readonly int ParentNodeIdx;
        /// <summary>
        /// The base transformations, these never change. AnimateTransformations are added to this to get the actual transformation.
        /// </summary>
        public Transformation DesignTransformation;
        /// <summary>
        /// The animated transformation properties, added to DesignTransformation to get the visible result.
        /// </summary>
        public Transformation AnimateTransformation;
        public readonly SpriteQuad SpriteQuad;
        internal Matrix GlobalTransform;
        internal Vector2 A, B, C, D;

        public RTNode(int parentNodeIdx, Transformation designTransformation, Transformation animateTransformation, SpriteQuad spriteQuad = null)
        {
            ParentNodeIdx = parentNodeIdx;
            SpriteQuad = spriteQuad;
            DesignTransformation = designTransformation;
            AnimateTransformation = animateTransformation;
            GlobalTransform = Matrix.Identity;
            A = B = C = D = new Vector2();
        }
    }
}
