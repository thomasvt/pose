using System.Collections.Generic;
using Pose.Domain.Editor;
using Pose.Domain.Nodes.Properties;

namespace Pose.Panels.Properties.SubPanels
{
    public abstract class NodeSubPanelViewModel
    : SubPanelViewModel
    {
        protected readonly Editor Editor;
        protected ulong NodeId;
        private readonly List<PropertyFieldViewModel> _fields;

        protected NodeSubPanelViewModel(Editor editor)
        {
            Editor = editor;
            _fields = new List<PropertyFieldViewModel>();
        }

        public void SetNodeId(ulong nodeId)
        {
            NodeId = nodeId;
        }

        /// <param name="displayValueFactor">The internal value is multiplied by this factor when being displayed</param>
        protected void RegisterField(PropertyFieldViewModel field)
        {
            _fields.Add(field);
            LinkPropertyFieldEvents(field);
        }

        private void LinkPropertyFieldEvents(PropertyFieldViewModel vm, float displayFactor = 1f)
        {
            vm.PropertyKeyed += propertyType =>
            {
                Editor.AddOrUpdateKeyAtCurrentFrame(NodeId, propertyType);
            };
            vm.PropertyUnkeyed += propertyType =>
            {
                Editor.RemoveKeyAtCurrentFrame(NodeId, propertyType);
            };
            vm.PropertyValueChanged += (sender, e) =>
            {
                if (e.IsTransient)
                {
                    Editor.SetNodePropertyVisual(NodeId, e.PropertyType, e.NewValue / displayFactor);
                }
                else
                {
                    Editor.SetNodeProperty(NodeId, e.PropertyType, e.NewValue / displayFactor);
                }
            };
        }

        public void RefreshKeyButtons()
        {
            BeginUpdate();
            try
            {
                foreach (var field in _fields)
                {
                    field.RefreshKeyButtonState(Editor, NodeId);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Refreshes property values and keybutton states.
        /// </summary>
        public override void Refresh()
        {
            BeginUpdate();
            try
            {
                foreach (var field in _fields)
                {
                    field.RefreshPropertyValueAndKeyButton(Editor, NodeId);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        private void BeginUpdate()
        {
            foreach (var field in _fields)
            {
                field.BeginUpdate();
            }
        }

        public void EndUpdate()
        {
            foreach (var field in _fields)
            {
                field.EndUpdate();
            }
        }

        /// <summary>
        /// Finds the field matching the propertytype, and refreshes its value.
        /// </summary>
        public void RefreshPropertyAndKeyButton(Editor editor, PropertyType propertyType)
        {
            foreach (var field in _fields)
            {
                if (field.PropertyType == propertyType)
                {
                    field.RefreshPropertyValueAndKeyButton(editor, NodeId);
                }
            }
        }
    }
}
