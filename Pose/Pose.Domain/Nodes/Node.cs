using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Nodes.Events;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;

namespace Pose.Domain.Nodes
{
    public abstract partial class Node
        : Entity, IEditableNode
    {
        private readonly Dictionary<PropertyType, Property> _properties;
        
        protected Node(IMessageBus messageBus, ulong nodeId, string name)
            : base(messageBus, nodeId)
        {
            _properties = new Dictionary<PropertyType, Property>
            {
                { PropertyType.TranslationX, new Property(messageBus, this, PropertyType.TranslationX) },
                { PropertyType.TranslationY, new Property(messageBus,this, PropertyType.TranslationY) },
                { PropertyType.RotationAngle, new Property(messageBus,this, PropertyType.RotationAngle) },
            };
            Name = name;
            Nodes = new NodeCollection(messageBus, this);
            AddProperty(PropertyType.Visibility, Property.TrueValue);

            foreach (var (propertyType, property) in _properties)
            {
                if (PropertyTypes.IsTransformProperty(propertyType))
                {
                    property.ValueChanged += OnTransformChanged;
                }
            }
        }

        protected void AddProperty(PropertyType propertyType, float initialDesignValue)
        {
            var property = new Property(MessageBus, this, propertyType);
            property.SetDesignValueInternal(initialDesignValue);
            _properties.Add(propertyType, property);
        }
        
        private void OnTransformChanged()
        {
            MessageBus.Publish(new NodeTransformChanged(Id, IsBulkUpdate));
            foreach (var child in Nodes)
            {
                child.OnTransformChanged();
            }
        }

        public void BeginUpdate()
        {
            IsBulkUpdate = true;
            foreach (var node in Nodes)
            {
                node.BeginUpdate();
            }
        }

        public void EndUpdate()
        {
            IsBulkUpdate = false;
            foreach (var node in Nodes)
            {
                node.EndUpdate();
            }
        }

        public Transformation GetAnimateTransformation()
        {
            return new Transformation(
                new Vector2(GetProperty(PropertyType.TranslationX).AnimateVisualValue, GetProperty(PropertyType.TranslationY).AnimateVisualValue),
                GetProperty(PropertyType.RotationAngle).AnimateVisualValue,
                Vector2.One,
                Parent?.GetAnimateTransformation());
        }

        public Transformation GetDesignTransformation()
        {
            return new Transformation(
                new Vector2(GetProperty(PropertyType.TranslationX).DesignVisualValue, GetProperty(PropertyType.TranslationY).DesignVisualValue),
                GetProperty(PropertyType.RotationAngle).DesignVisualValue,
                Vector2.One,
                Parent?.GetDesignTransformation());
        }

        /// <summary>
        /// Adjusts the BaseValue of the Properties that form the local transform of this node so their global transform equals the one requested.
        /// </summary>
        public void UpdatePropertiesDesignValuesForGlobalTransform(IUnitOfWork uow, Matrix globalTransform)
        {
            // todo temporary until shear and scale operators are added, maybe change to only correct translate+rotate ? See other apps.
            Matrix correctedLocalTransform;

            if (Parent == null)
            {
                correctedLocalTransform = globalTransform;
            }
            else
            {
                var inverseNewParentTransform = Parent.GetDesignTransformation().GlobalTransform.GetInverse();
                if (!inverseNewParentTransform.HasValue)
                    return; // don't correct transform: it's mathematically impossible.

                correctedLocalTransform = inverseNewParentTransform.Value * globalTransform;
            }

            var angle = GetAngleFromTransform(correctedLocalTransform);
            var translation = GetTranslationFromTransform(correctedLocalTransform);

            GetProperty(PropertyType.TranslationX).SetDesignValue(uow, translation.X);
            GetProperty(PropertyType.TranslationY).SetDesignValue(uow, translation.Y);
            GetProperty(PropertyType.RotationAngle).SetDesignValue(uow, angle);
        }

        private static float GetAngleFromTransform(Matrix transform)
        {
            // todo replace with angle parent-chain addition because reversing the matrix is a dead end when Shear will be added (and has rounding error issues (eg >1 ))
            var m11 = transform.M11;
            if (m11 < -1f)
            {
                m11 = -1f;
            }
            else if (m11 > 1f)
            {
                m11 = 1f;
            }

            var acos = MathF.Acos(m11);
            if (transform.M21 >= 0)
            {
                return acos;
            }

            return MathF.PI * 2f - acos;
        }

        private static Vector2 GetTranslationFromTransform(Matrix transform)
        {
            return new Vector2(transform.M13, transform.M23);
        }

        public Property GetProperty(PropertyType propertyType)
        {
            return _properties[propertyType];
        }

        /// <summary>
        /// Returns all property values in a portable structure.
        /// </summary>
        public List<PropertyValueSet> GetPropertyValueSets()
        {
            return _properties.Values.Select(v => v.GetPropertyValueSet()).ToList();
        }

        /// <summary>
        /// Used by deserializer.
        /// </summary>
        public void InternalAdd(Node node)
        {
            Nodes.InternalAdd(node);
        }

        internal void ResetAnimateIncrementValues()
        {
            foreach (var property in _properties.Values)
            {
                ((IEditableProperty) property).ResetAnimateValueToDesignPose();
            }
        }

        public void Rename(IUnitOfWork uow, string name)
        {
            if (name == Name)
                return;

            uow.Execute(new NodeRenamedEvent(Id, Name, name));
        }
        
        public override string ToString()
        {
            return $"[{Name}]";
        }

        public Node Parent { get; internal set; }

        public string Name { get; private set; }

        public NodeCollection Nodes { get; }
        /// <summary>
        /// See <see cref="BulkSceneUpdateEnded"/> for info.
        /// </summary>
        public bool IsBulkUpdate { get; private set; }
    }
}
