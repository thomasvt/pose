using System;
using System.Collections.Generic;
using Pose.Common;
using Pose.Common.Curves;
using Pose.Domain.Animations;
using Pose.Domain.Documents;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Editor
{
    /// <summary>
    /// All possible user-initiated changes on the document. All changes get recorded in History (for undo/redo) except when they have the Transient suffix.
    /// </summary>
    public partial class Editor
    {
        /// <summary>
        /// Moves a node to another place in the scene Hierarchy.
        /// </summary>
        public void MoveNodeInHierarchy(ulong nodeId, ulong targetNodeId, InsertPosition insertPosition)
        {
            RequireMode(EditorMode.Design);

            var node = CurrentDocument.GetNode(nodeId);
            using var uow = CurrentDocument.StartUnitOfWork($"{node} Move in scene tree");
            _currentDocument.MoveNodeInHierarchy(uow, nodeId, targetNodeId, insertPosition);
        }

        /// <summary>
        /// Adjust the draworder of a <see cref="SpriteNode"/>.
        /// </summary>
        public void MoveSpriteNodeInFrontOfTarget(ulong nodeId, int destinationIndex)
        {
            RequireMode(EditorMode.Design);

            using var uow = CurrentDocument.StartUnitOfWork("Change draworder");
            _currentDocument.MoveSpriteInDrawOrder(uow, nodeId, destinationIndex);
        }

        public void AddOrUpdateKeyAtCurrentFrame(ulong nodeId, PropertyType property)
        {
            RequireMode(EditorMode.Animate);

            var node = CurrentDocument.GetNode(nodeId);
            var animation = CurrentDocument.GetAnimation(_currentAnimationId);
            using var uow = CurrentDocument.StartUnitOfWork($"{node} Set Key {property} @ [F:{animation.CurrentFrame}]");
            animation.KeyCurrentPropertyValue(uow, nodeId, property);
        }

        /// <summary>
        /// Adds an animation key in the current animation, at the current timeline frame.
        /// </summary>
        public void RemoveKeyAtCurrentFrame(ulong nodeId, PropertyType property)
        {
            RequireMode(EditorMode.Animate);

            var node = CurrentDocument.GetNode(nodeId);
            var animation = CurrentDocument.GetAnimation(_currentAnimationId);
            using var uow = CurrentDocument.StartUnitOfWork($"{node} Remove Key {property} @ [F:{animation.CurrentFrame}]");
            animation.RemoveKeyAtCurrentFrame(uow, nodeId, property);
        }

        /// <summary>
        /// Applies the animation values for a certain non-integer frame to the scene. This is used to create a smooth realtime animation in the editor, that has fluctuating frame intervals that don't map to the animation's frames.
        /// </summary>
        /// <param name="isFirstFrameOfRealTimeSequence">When you're playing the animation realtime, set this to False on all non-first-of-play-sequence frames.</param>
        public void ApplyFrameToScene(in float frame, bool isFirstFrameOfRealTimeSequence = true)
        {
            RequireMode(EditorMode.Animate);
            CurrentDocument.ApplyAnimationToScene(_currentAnimationId, frame, isFirstFrameOfRealTimeSequence);
        }

        public void ToggleCurrentAnimationIsLoop()
        {
            RequireMode(EditorMode.Animate);

            var animation = GetCurrentAnimation();
            using var uow = CurrentDocument.StartUnitOfWork($"{animation} toggle Loop");
            animation.ToggleIsLoop(uow);
        }

        /// <summary>
        /// Changes the current frame of the current animation without recording the change in History.
        /// </summary>
        public void ChangeCurrentAnimationCurrentFrameTransient(in int newFrame, bool noSceneUpdate = false)
        {
            RequireMode(EditorMode.Animate);

            var animation = GetCurrentAnimation();
            animation.ChangeCurrentFrame(newFrame, noSceneUpdate);
        }

        /// <summary>
        /// Changed the begin frame of the current animation.
        /// </summary>
        public void ChangeCurrentAnimationBeginFrame(in int undoValue, in int value)
        {
            RequireMode(EditorMode.Animate);

            var animation = GetCurrentAnimation();
            using var uow = CurrentDocument.StartUnitOfWork($"{animation} Change StartFrame");
            animation.ChangeBeginFrame(uow, undoValue, value);
        }

        /// <summary>
        /// Changed the begin frame of the current animation without recording the change in History.
        /// </summary>
        public void ChangeCurrentAnimationBeginFrameTransient(in int beginFrame)
        {
            RequireMode(EditorMode.Animate);

            var animation = GetCurrentAnimation();
            animation.ChangeBeginFrameTransient(beginFrame);
        }

        /// <summary>
        /// Changed the end frame of the current animation.
        /// </summary>
        public void ChangeCurrentAnimationEndFrame(in int undoValue, in int value)
        {
            RequireMode(EditorMode.Animate);

            var animation = GetCurrentAnimation();
            using var uow = CurrentDocument.StartUnitOfWork($"{animation} Change EndFrame");
            animation.ChangeEndFrame(uow, undoValue, value);
        }

        /// <summary>
        /// Changed the end frame of the current animation without recording the change in History.
        /// </summary>
        public void ChangeCurrentAnimationEndFrameTransient(in int endFrame)
        {
            RequireMode(EditorMode.Animate);

            var animation = GetCurrentAnimation();
            animation.ChangeEndFrameTransient(endFrame);
        }

        /// <summary>
        /// Adds a spritenode for the given sprite as a new rootnode.
        /// </summary>
        public void AddSpriteNode(string name, SpriteReference spriteRef)
        {
            RequireMode(EditorMode.Design);

            using var uow = CurrentDocument.StartUnitOfWork($"Create {SpriteNode.GetLabel(name)}");
            var nodeId = CurrentDocument.AddSpriteNode(uow, name, spriteRef);
            NodeSelection.SelectSingle(nodeId);
        }


        /// <summary>
        /// Adds a bone node at the given position, and tail.
        /// </summary>
        /// <param name="tailLength">The vector from the start of the bone to the tail tip.</param>
        public void AddBoneNode(in Vector2 position, float angle, in float tailLength)
        {
            RequireMode(EditorMode.Design);

            var name = CurrentDocument.GetUniqueBoneName();
            using var uow = CurrentDocument.StartUnitOfWork($"Create {BoneNode.GetLabel(name)}");
            var nodeId = CurrentDocument.AddBoneNode(uow, name, position, angle, tailLength);
            NodeSelection.SelectSingle(nodeId);
        }

        public void RenameNode(in ulong nodeId, string name)
        {
            var node = CurrentDocument.GetNode(nodeId);
            using var uow = CurrentDocument.StartUnitOfWork($"Rename {node} to [{name}]");
            node.Rename(uow, name);
        }

        /// <summary>
        /// Deletes all selected nodes, their childnodes and animation keyframes.
        /// </summary>
        public void RemoveSelectedNodes()
        {
            RequireMode(EditorMode.Design);

            if (NodeSelection.Count == 0)
                return;

            var list = NodeSelection.ToList();
            var label = list.Count == 1 ? $"{CurrentDocument.GetNode(list[0])} Remove" : $"[{list.Count} Nodes] Remove";
            using var uow = CurrentDocument.StartUnitOfWork(label);
            foreach (var nodeId in list)
            {
                CurrentDocument.RemoveNode(uow, nodeId);
            }
        }

        /// <summary>
        /// Renoves all selected animation keys and cleans up animation rows.
        /// </summary>
        public void RemoveSelectedAnimationKeys()
        {
            RequireMode(EditorMode.Animate);

            if (KeySelection.Count == 0)
                return;

            var list = KeySelection.ToList();
            var label = list.Count == 1
                ? $"Key @ [F:{CurrentDocument.GetKey(list[0]).Frame}] Remove"
                : $"[{list.Count} Keys] Remove";
            using var uow = CurrentDocument.StartUnitOfWork(label);
            var currentAnimation = CurrentDocument.GetAnimation(_currentAnimationId);
            currentAnimation.RemoveKeys(uow, new HashSet<ulong>(list));
        }

        public void SetDocumentFilename(string filename)
        {
            CurrentDocument.SetFilename(filename);
            MessageBus.Default.Publish(new CurrentDocumentFilenameChanged(filename));
        }

        public void SetDocumentAssetFolder(string path)
        {
            RequireMode(EditorMode.Design);

            using var uow = CurrentDocument.StartUnitOfWork("Change asset-folder");
            CurrentDocument.SetAssetFolder(uow, path);
        }

        /// <summary>
        /// Adds a new animation and selects it.
        /// </summary>
        public void AddNewAnimation()
        {
            RequireMode(EditorMode.Animate);

            var i = 0;
            while (CurrentDocument.AnimationExists($"animation-{++i}"))
            {
            }
            var name = $"animation-{i}";

            using var uow = CurrentDocument.StartUnitOfWork($"Add animation [{name}]");
            var animationId = CurrentDocument.AddAnimation(uow, name, 1, 60, 60, true);
            ChangeCurrentAnimation(animationId);
        }

        public void RemoveSelectedAnimation()
        {
            RequireMode(EditorMode.Animate);

            if (CurrentDocument.AnimationCount < 2)
                throw new Exception("Cannot remove the last animation.");

            var animation = CurrentDocument.GetAnimation(_currentAnimationId);
            using var uow = CurrentDocument.StartUnitOfWork($"Remove animation [{animation.Name}]");
            CurrentDocument.RemoveAnimation(uow, _currentAnimationId);
        }

        public void ChangeKeyValue(in ulong keyId, in float value)
        {
            RequireMode(EditorMode.Animate);

            using var uow = CurrentDocument.StartUnitOfWork("Change Key value");
            var key = CurrentDocument.GetKey(keyId);
            key.ChangeValue(uow, value);
        }

        public void ChangeKeyInterpolation(in ulong keyId, CurveType type, BezierCurve? bezierCurve)
        {
            RequireMode(EditorMode.Animate);

            if (bezierCurve == null && type == CurveType.Bezier)
                throw new Exception("BezierCurve cannot be null when CurveType is Bezier.");
            if (bezierCurve != null && type != CurveType.Bezier)
                throw new Exception("BezierCurve must be null when CurveType is not Bezier.");

            var key = CurrentDocument.GetKey(keyId);
            var interpolationData = new InterpolationData(type, bezierCurve);
            if (interpolationData.Equals(key.Interpolation))
                return;

            using var uow = CurrentDocument.StartUnitOfWork($"Change Key {type.ToString()} curve");
            key.ChangeInterpolation(uow, interpolationData);
        }

        public void RenameAnimation(in ulong animationId, string newName)
        {
            RequireMode(EditorMode.Animate);

            var animation = CurrentDocument.GetAnimation(animationId);
            using var uow = CurrentDocument.StartUnitOfWork($"Rename {animation} to [{newName}]");
            animation.Rename(uow, newName);
        }

        #region NodeProperties

        public void SetNodeProperty(ulong nodeId, PropertyType propertyType, float newValue, bool recursive = false)
        {
            var node = CurrentDocument.GetNode(nodeId);
            var label = _isAutoKeying ? $"{node} Change value {propertyType} + Key" : $"{node} Change value {propertyType}";
            using var uow = CurrentDocument.StartUnitOfWork(label);
            SetNodePropertyInternal(uow, node, propertyType, newValue, recursive);
        }

        public void SetNodeProperty(ulong nodeId, PropertyType propertyType, bool newValue, bool recursive = false)
        {
            SetNodeProperty(nodeId, propertyType, newValue ? Property.TrueValue : Property.FalseValue, recursive);
        }

        private void SetNodePropertyInternal(IUnitOfWork uow, Node node, PropertyType propertyType, float newValue, bool recursive = false)
        {
            var property = node.GetProperty(propertyType);
            switch (Mode)
            {
                case EditorMode.Design:
                {
                    property.SetDesignValue(uow, newValue);
                    break;
                }
                case EditorMode.Animate:
                {
                    property.SetAnimateValue(uow, newValue);
                    if (_isAutoKeying)
                    {
                        var animation = CurrentDocument.GetAnimation(_currentAnimationId);
                        animation.KeyCurrentPropertyValue(uow, node.Id, propertyType);
                    }
                    break;
                }
            }

            if (recursive)
            {
                foreach (var childNode in node.Nodes)
                {
                    SetNodePropertyInternal(uow, childNode, propertyType, newValue, recursive);
                }
            }
        }

        /// <summary>
        /// Sets the translation of a Node.
        /// </summary>
        public void SetNodeTranslation(in ulong nodeId, in Vector2 translation)
        {
            var node = CurrentDocument.GetNode(nodeId);
            var label = _isAutoKeying ? $"{node} Translate + Key" : $"{node} Translate";
            using var uow = CurrentDocument.StartUnitOfWork(label);

            SetNodePropertyInternal(uow, node, PropertyType.TranslationX, translation.X);
            SetNodePropertyInternal(uow, node, PropertyType.TranslationY, translation.Y);
        }

        public void SetNodeTranslationVisual(in ulong nodeId, in Vector2 translation)
        {
            SetNodePropertyVisual(nodeId, PropertyType.TranslationX, translation.X);
            SetNodePropertyVisual(nodeId, PropertyType.TranslationY, translation.Y);
        }

        public void ResetNodeTranslationVisual(in ulong nodeId)
        {
            ResetNodePropertyVisual(nodeId, PropertyType.TranslationX);
            ResetNodePropertyVisual(nodeId, PropertyType.TranslationY);
        }

        /// <summary>
        /// Sets the rotation of a Node.
        /// </summary>
        public void SetNodeRotation(ulong nodeId, float angle)
        {
            var node = CurrentDocument.GetNode(nodeId);
            var label = _isAutoKeying ? $"{node} Rotate + Key" : $"{node} Rotate";
            using var uow = CurrentDocument.StartUnitOfWork(label);
            SetNodePropertyInternal(uow, node, PropertyType.RotationAngle, angle);
        }

        public void SetNodeRotationVisual(in ulong nodeId, in float angle)
        {
            SetNodePropertyVisual(nodeId, PropertyType.RotationAngle, angle);
        }

        public void ResetNodeRotationVisual(in ulong nodeId)
        {
            ResetNodePropertyVisual(nodeId, PropertyType.RotationAngle);
        }

        /// <summary>
        /// Sets the visual value of a node property. Visual is what is directly used by the Viewport, including during mousedragging.
        /// Visual values are temporary and will not create a History entry. To permanently store a value, call SetNodeProperty().
        /// </summary>
        public void SetNodePropertyVisual(ulong nodeId, PropertyType propertyType, in float value)
        {
            var node = CurrentDocument.GetNode(nodeId);
            var propertyAngle = node.GetProperty(propertyType);

            switch (Mode)
            {
                case EditorMode.Design:
                {
                    propertyAngle.SetDesignVisualValue(value);
                    break;
                }
                case EditorMode.Animate:
                {
                    propertyAngle.SetAnimateVisualValue(value);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Reverts the Visual Value of the Property to the value last set by a call to SetNodeProperty() (or equivalent)
        /// </summary>
        public void ResetNodePropertyVisual(ulong nodeId, PropertyType propertyType)
        {
            var node = CurrentDocument.GetNode(nodeId);
            var propertyAngle = node.GetProperty(propertyType);

            switch (Mode)
            {
                case EditorMode.Design:
                {
                    propertyAngle.ResetDesignVisualValue();
                    break;
                }
                case EditorMode.Animate:
                {
                    propertyAngle.ResetAnimateVisualValue();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        #endregion

    }
}
