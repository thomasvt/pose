using System.Collections.Generic;
using Pose.Domain.Animations;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Domain.Documents
{
    internal interface IEditableDocument : IDocument
    {
        void MoveNode(ulong nodeId, ulong? targetNodeId, int destinationIndex);
        void RemoveAnimation(in ulong id);
        void RemoveKey(in ulong keyId);
        void RemoveNode(ulong nodeId);
        void RemovePropertyAnimation(in ulong propertyAnimationId);
        Animation AddAnimation(in ulong animationId, string name, int beginFrame, int endFrame, uint framesPerSecond, bool isLoop);
        Key AddKey(ulong keyId, ulong propertyAnimationId, int frame, float value, InterpolationData interpolation);
        PropertyAnimation AddPropertyAnimation(in ulong propertyAnimationId, ulong animationId, ulong nodeId, PropertyType property);
        SpriteNode AddSpriteNode(ulong nodeId, string name, SpriteReference spriteRef, ulong? parentNodeId, int index, List<PropertyValueSet> propertyValues);
        BoneNode AddBoneNode(in ulong nodeId, string name, ulong? parentNodeId, in int index, List<PropertyValueSet> propertyValues);

        /// <summary>
        /// Returns an id for a new entity that's unique within the document.
        /// </summary>
        ulong GetNextEntityId();

        void AddNodeAnimationCollection(in ulong animationId, ulong nodeId);
        void RemoveNodeAnimationCollection(in ulong animationId, in ulong nodeId);
        void MoveSpriteInFrontOf(in ulong nodeId, int index);
        void MarkDirty();
        void SetAssetFolder(string path);
        void SetMetaDataValue(string key, object value);
    }
}
