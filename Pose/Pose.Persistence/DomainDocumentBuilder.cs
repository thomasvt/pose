using System;
using System.Collections.Generic;
using System.IO;
using Pose.Domain;
using Pose.Domain.Animations;
using Pose.Domain.Curves;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Persistence
{
    public static class DomainDocumentBuilder
    {
        public static Domain.Documents.Document CreateDocument(IMessageBus messageBus, Document doc, string filePath)
        {
            var data = CreateDocumentData(messageBus, doc, filePath);
            var document = Domain.Documents.Document.CreateFrom(messageBus, data);
            document.InternalSetFilename(filePath);
            document.PreviousSaveFilename = doc.LastFilename;
            return document;
        }

        public static DocumentData CreateDocumentData(IMessageBus messageBus, Document document, string filename)
        {
            var relativeAssetFolder = string.IsNullOrEmpty(document.AssetFolder) ? null : document.AssetFolder; // protobuf cannot represent null string, we use ""
            var data = new DocumentData(messageBus, document.IdSequence)
            {
                RelativeAssetFolder = relativeAssetFolder,
                AbsoluteAssetFolder = relativeAssetFolder != null ? GetAbsoluteAssetFolder(document.AssetFolder, filename) : null
            };
            AddNodes(data, document);
            AddAnimations(data, document.Animations);
            AddDrawOrder(data, document.DrawOrder);

            return data;
        }

        private static string GetAbsoluteAssetFolder(string assetFolder, string filename)
        {
            if (Path.IsPathRooted(assetFolder))
                return assetFolder;
            return Path.GetFullPath(assetFolder, Path.GetDirectoryName(filename));
        }

        private static void AddNodes(DocumentData data, Document doc)
        {
            // add nodes in same order as they appear in domain.
            foreach (var node in doc.Nodes)
            {
                var domainNode = node.Type switch
                {
                    Node.Types.NodeType.Spritenode => BuildSpriteNode(data, node),
                    Node.Types.NodeType.Bonenode => BuildBoneNode(data, node),
                    _ => throw new NotSupportedException("Unknown Node type: " + node.Type)
                };
                data.Nodes.Add(domainNode);
            }
        }

        private static Domain.Nodes.Node BuildBoneNode(DocumentData data, Node node)
        {
            var domainNode = new BoneNode(data.MessageBus, node.Id, node.Name)
            {
                Parent = node.ParentId == 0
                    ? null
                    : data.Nodes[node.ParentId] // node list is in hierarchical order, so parents always are available when their children are added.
            };

            if (domainNode.Parent == null)
            {
                data.RootNodes.InternalAdd(domainNode);
            }
            else
            {
                domainNode.Parent?.InternalAdd(domainNode);
            }

            domainNode.GetProperty(PropertyType.TranslationX).SetDesignValueInternal(node.Position.X);
            domainNode.GetProperty(PropertyType.TranslationY).SetDesignValueInternal(node.Position.Y);
            domainNode.GetProperty(PropertyType.RotationAngle).SetDesignValueInternal(node.Angle);
            domainNode.GetProperty(PropertyType.BoneLength).SetDesignValueInternal(node.BoneLength);

            return domainNode;
        }

        private static Domain.Nodes.Node BuildSpriteNode(DocumentData data, Node node)
        {
            var spriteRef = new SpriteReference(node.SpriteKey);
            var domainNode = new SpriteNode(data.MessageBus, node.Id, node.Name, spriteRef)
            {
                Parent = node.ParentId == 0
                    ? null
                    : data.Nodes[node.ParentId] // node list is in hierarchical order, so parents always are available when their children are added.
            };

            if (domainNode.Parent == null)
            {
                data.RootNodes.InternalAdd(domainNode);
            }
            else
            {
                domainNode.Parent?.InternalAdd(domainNode);
            }

            domainNode.GetProperty(PropertyType.TranslationX).SetDesignValueInternal(node.Position.X);
            domainNode.GetProperty(PropertyType.TranslationY).SetDesignValueInternal(node.Position.Y);
            domainNode.GetProperty(PropertyType.RotationAngle).SetDesignValueInternal(node.Angle);

            return domainNode;
        }

        private static void AddAnimations(DocumentData data, IEnumerable<Animation> animations)
        {
            foreach (var animation in animations)
            {
                var domainAnimation = new Domain.Animations.Animation(data.MessageBus, animation.Id, animation.Name)
                {
                    BeginFrame = animation.BeginFrame,
                    EndFrame = animation.EndFrame,
                    FramesPerSecond = animation.FramesPerSecond,
                    IsLoop = animation.IsLoop
                };

                foreach (var nodeAnimation in animation.NodeAnimations)
                {
                    var nodeId = nodeAnimation.NodeId;
                    foreach (var propertyAnimation in nodeAnimation.PropertyAnimations)
                    {
                        var domainPropertyAnimation = new Domain.Animations.PropertyAnimation(data.MessageBus, propertyAnimation.Id, animation.Id, nodeId, (PropertyType)propertyAnimation.Property, propertyAnimation.Vertex);
                        domainAnimation.InternalAdd(data.Nodes[nodeId], domainPropertyAnimation);
                        data.PropertyAnimations.Add(domainPropertyAnimation);
                        AddAnimationKeys(data, propertyAnimation, domainPropertyAnimation);
                    }
                }

                data.Animations.Add(domainAnimation);
            }
        }

        private static void AddAnimationKeys(DocumentData data, PropertyAnimation propertyAnimation, Domain.Animations.PropertyAnimation domainPropertyAnimation)
        {
            foreach (var key in propertyAnimation.Keys)
            {
                var interpolation = MapInterpolationData(key.InterpolationType, key.Curve);
                var domainKey = new Domain.Animations.Key(data.MessageBus, key.Id, propertyAnimation.Id, key.Frame, key.Value, interpolation);
                domainPropertyAnimation.InternalAdd(domainKey);
                data.Keys.Add(domainKey);
            }
        }

        private static InterpolationData MapInterpolationData(Key.Types.InterpolationTypeEnum type, BezierCurve curve)
        {
            switch (type)
            {
                case Key.Types.InterpolationTypeEnum.Bezier:
                    return new InterpolationData(CurveType.Bezier, new Domain.Curves.BezierCurve(MapPoint(curve.P0), MapPoint(curve.P1), MapPoint(curve.P2), MapPoint(curve.P3)));
                case Key.Types.InterpolationTypeEnum.Hold:
                    return new InterpolationData(CurveType.Hold);
                case Key.Types.InterpolationTypeEnum.Linear:
                    return new InterpolationData(CurveType.Linear);
                default:
                    throw new NotSupportedException($"Unknown interpolation type [{type}] found in file.");
            }
        }

        private static Vector2 MapPoint(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        private static void AddDrawOrder(DocumentData data, DrawOrder drawOrder)
        {
            data.DrawOrder.InternalSet(drawOrder.NodeIds);
        }

        
    }
}
