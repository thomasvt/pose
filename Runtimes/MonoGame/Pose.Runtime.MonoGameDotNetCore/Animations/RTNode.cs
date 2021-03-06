﻿using Microsoft.Xna.Framework;
using Pose.Runtime.MonoGameDotNetCore.Rendering;
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

        public bool IsVisible;
        /// <summary>
        /// Reference to the graphical sprite of this node.
        /// </summary>
        public readonly Sprite Sprite;

        /// <summary>
        /// The draworder index of this node within the skeleton as defined in the Pose editor. Lower is more in front.
        /// </summary>
        public readonly int DrawOrderIndex;

        /// <summary>
        /// The transform of this node at its pivotpoint.
        /// </summary>
        internal Matrix GlobalTransform;

        public RTNode(int parentNodeIdx, bool isVisible, Transformation designTransformation, Transformation animateTransformation, int drawOrderIndex, Sprite sprite = null)
        {
            IsVisible = isVisible;
            ParentNodeIdx = parentNodeIdx;
            Sprite = sprite;
            DesignTransformation = designTransformation;
            AnimateTransformation = animateTransformation;
            GlobalTransform = Matrix.Identity;
            DrawOrderIndex = drawOrderIndex;
        }
    }
}
