using System;
using System.Collections.Generic;
using System.IO;
using Pose.Domain;
using Pose.Domain.Curves;
using Pose.Domain.Nodes;
using Pose.Domain.Nodes.Properties;

namespace Pose.Persistence
{
    public static class ProtoModelBuilder
    {
        public static Document CreateProtobufDocument(Domain.Documents.Document document)
        {
            var doc = new Document
            {
                LastFilename = document.Filename ?? string.Empty,
                IdSequence = document.Data.IdSequence.GetCurrentValue(),
                AssetFolder = GetRelativeAssetFolder(document.AssetFolder, document.Filename)
            };
            AddNodes(document.Data.RootNodes, doc);
            AddAnimations(document.Data.Animations, doc);
            AddDrawOrder(document.Data.DrawOrder, doc);

            return doc;
        }

        private static string GetRelativeAssetFolder(string assetFolder, string filename)
        {
            if (assetFolder == null)
            {
                return string.Empty;  // protobuf cannot represent null string
            }

            if (string.IsNullOrEmpty(filename))
                return assetFolder;

            return Path.GetRelativePath(Path.GetDirectoryName(filename), assetFolder);
        }

        private static void AddNodes(NodeCollection nodes, Document doc)
        {
            // add nodes in same order as they appear in domain.
            foreach (var domainNode in nodes)
            {
                var node = new Node
                {
                    Id = domainNode.Id,
                    Name = domainNode.Name,
                    ParentId = domainNode.Parent?.Id ?? 0,
                    Position = new Point
                    {
                        X = domainNode.GetProperty(PropertyType.TranslationX).DesignValue,
                        Y = domainNode.GetProperty(PropertyType.TranslationY).DesignValue
                    },
                    Angle = domainNode.GetProperty(PropertyType.RotationAngle).DesignValue,

                };
                doc.Nodes.Add(node);
                switch (domainNode)
                {
                    case SpriteNode spriteNode:
                        node.Type = Node.Types.NodeType.Spritenode;
                        node.SpriteFile = spriteNode.SpriteRef.RelativePath;
                        break;
                    case BoneNode boneNode:
                        node.Type = Node.Types.NodeType.Bonenode;
                        node.BoneLength = boneNode.GetProperty(PropertyType.BoneLength).DesignValue;
                        break;
                    default:
                        throw new NotSupportedException($"Unknown node type: {domainNode.GetType().Name}");
                }
                AddNodes(domainNode.Nodes, doc);
            }
        }

        private static void AddAnimations(IEnumerable<Domain.Animations.Animation> animations, Document doc)
        {
            foreach (var animation in animations)
            {
                var anim = new Animation
                {
                    Id = animation.Id,
                    Name = animation.Name,
                    BeginFrame = animation.BeginFrame,
                    EndFrame = animation.EndFrame,
                    FramesPerSecond = animation.FramesPerSecond,
                    IsLoop = animation.IsLoop
                };

                foreach (var nodeAnimation in animation.AnimationsPerNode.Values)
                {
                    var nodeAnim = new NodeAnimation
                    {
                        NodeId = nodeAnimation.Node.Id
                    };
                    foreach (var propertyAnimation in nodeAnimation.PropertyAnimations.Values)
                    {
                        var propAnim = new PropertyAnimation
                        {
                            Id = propertyAnimation.Id,
                            Property = (uint)propertyAnimation.Property,
                            Vertex = propertyAnimation.Vertex
                        };
                        AddAnimationKeys(propertyAnimation.Keys, propAnim);
                        nodeAnim.PropertyAnimations.Add(propAnim);
                    }
                    anim.NodeAnimations.Add(nodeAnim);
                }
                doc.Animations.Add(anim);
            }
        }

        private static void AddAnimationKeys(IEnumerable<Domain.Animations.Key> keys, PropertyAnimation propertyAnimation)
        {
            foreach (var key in keys)
            {
                propertyAnimation.Keys.Add(new Key
                {
                    Id = key.Id,
                    Frame = key.Frame,
                    Value = key.Value,
                    InterpolationType = GetCurveType(key.Interpolation.Type),
                    Curve = MapCurve(key.Interpolation.BezierCurve)
                });
            }
        }

        private static BezierCurve MapCurve(Domain.Curves.BezierCurve? curve)
        {
            if (curve == null)
                return null;

            return new BezierCurve
            {
                P0 = MapPoint(curve.Value.P0),
                P1 = MapPoint(curve.Value.P1),
                P2 = MapPoint(curve.Value.P2),
                P3 = MapPoint(curve.Value.P3)
            };
        }

        private static Point MapPoint(in Vector2 p)
        {
            return new Point
            {
                X = p.X,
                Y = p.Y
            };
        }

        private static Key.Types.InterpolationTypeEnum GetCurveType(CurveType type)
        {
            switch (type)
            {
                case CurveType.Linear:
                    return Key.Types.InterpolationTypeEnum.Linear;
                case CurveType.Bezier:
                    return Key.Types.InterpolationTypeEnum.Bezier;
                case CurveType.Hold:
                    return Key.Types.InterpolationTypeEnum.Hold;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void AddDrawOrder(Domain.Documents.DrawOrder drawOrder, Document doc)
        {
            doc.DrawOrder = new DrawOrder();
            doc.DrawOrder.NodeIds.AddRange(drawOrder.GetNodeIdsInOrder());
        }
    }
}
