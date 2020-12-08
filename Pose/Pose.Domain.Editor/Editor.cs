using System;
using Pose.Domain.Animations.Messages;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework.Messaging;
using Pose.Persistence;
using Pose.Persistence.Editor;
using Document = Pose.Domain.Documents.Document;

namespace Pose.Domain.Editor
{
    /// <summary>
    /// The single-access API of the editor. ViewModels throughout the app can only modify the document content by calling methods here and will receive state changes through <see cref="MessageBus"/> events.
    /// </summary>
    public partial class Editor
    {
        private readonly IMessageBus _messageBus;
        private Document _currentDocument;
        private bool _isAutoKeying;
        private ulong _currentAnimationId;
        
        public Document CurrentDocument => _currentDocument ?? throw new Exception("There's no open document.");

        public Editor(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            NodeSelection = new Selection<ulong>
            {
                DeselectAction = id => _messageBus.Publish(new NodeDeselected(id)),
                SelectAction = id => _messageBus.Publish(new NodeSelected(id))
            };
            KeySelection = new Selection<ulong>
            {
                DeselectAction = item => _messageBus.Publish(new KeyDeselected(item)),
                SelectAction = item => _messageBus.Publish(new KeySelected(item)),
            };

            _messageBus.Subscribe<NodeRemoving>(msg => NodeSelection.Remove(msg.NodeId));
            _messageBus.Subscribe<AnimationKeyRemoving>(msg => KeySelection.Remove(msg.KeyId));
            _messageBus.Subscribe<AnimationRemoved>(OnAnimationRemoved);
            // todo AnimationRemoving -> change CurrentAnimation to an existing animation.
            _messageBus.Subscribe<MetaDataChanged>(OnMetaDataChanged);
            _messageBus.Subscribe<NodePropertyValueChanged>(OnNodePropertyValueChanged);
            _messageBus.Subscribe<AnimationCurrentFrameChanged>(OnAnimationCurrentFrameChanged);
            _messageBus.Subscribe<AnimationKeyValueChanged>(OnKeyValueChanged);
            _messageBus.Subscribe<AnimationKeyRemoved>(OnKeyRemoved);
            _messageBus.Subscribe<AnimationKeyAdded>(OnKeyAdded);
        }

        private void OnAnimationCurrentFrameChanged(AnimationCurrentFrameChanged msg)
        {
            if (!msg.NoSceneUpdate && msg.AnimationId == _currentAnimationId)
                CurrentDocument.ApplyCurrentAnimationFrameToScene(_currentAnimationId);
        }

        private void OnKeyValueChanged(AnimationKeyValueChanged msg)
        {
            CurrentDocument.ApplyCurrentAnimationFrameToScene(_currentAnimationId);
        }

        private void OnKeyAdded(AnimationKeyAdded obj)
        {
            CurrentDocument.ApplyCurrentAnimationFrameToScene(_currentAnimationId);
        }

        private void OnKeyRemoved(AnimationKeyRemoved msg)
        {
            CurrentDocument.ApplyCurrentAnimationFrameToScene(_currentAnimationId);
        }

        private void OnNodePropertyValueChanged(NodePropertyValueChanged msg)
        {
            if (msg.PropertyType == PropertyType.Visibility)
            {
                if (!GetNodePropertyAsBool(msg.NodeId, PropertyType.Visibility) && NodeSelection.Contains(msg.NodeId))
                {
                    NodeSelection.Remove(msg.NodeId);
                }
            }
        }

        private void OnAnimationRemoved(AnimationRemoved msg)
        {
            if (_currentAnimationId == msg.AnimationId)
            {
                ChangeCurrentAnimation(CurrentDocument.GetFirstAnimationId());
            }
        }

        private void OnMetaDataChanged(MetaDataChanged msg)
        {
            if (msg.Key == "mode")
            {
                DoChangeEditorMode((EditorMode) msg.Value);
            }
        }

        public void CreateNewDocument()
        {
            var document = Document.CreateNew(_messageBus);
            LoadDocument(document);
        }

        /// <summary>
        /// Loads a <see cref="Document"/> into the editor.
        /// </summary>
        public void LoadDocument(Document document)
        {
            CloseCurrentDocument();

            _currentDocument = document;
            _currentDocument.Modified += CurrentDocumentOnModified;
            
            // todo probably replace this with a FullReload() concept to do clean switch to different data
            ChangeCurrentAnimationInternal(_currentDocument.GetFirstAnimationId()); // documents always have at least 1 animation
            _messageBus.Publish(new DocumentLoaded(_currentDocument));
            DoChangeEditorMode(EditorMode.Design);
            DoChangeEditorTool(EditorTool.Modify);
        }

        private void CurrentDocumentOnModified()
        {
            _messageBus.Publish(new CurrentDocumentModified());
        }

        private void CloseCurrentDocument()
        {
            if (_currentDocument == null)
                return;

            _currentDocument.Modified -= CurrentDocumentOnModified;
            ClearEditor();
        }

        private void ClearEditor()
        {
            ChangeEditorMode(EditorMode.Design);
            _currentAnimationId = 0;
            NodeSelection.Clear();
            KeySelection.Clear();
            _currentDocument = null;
        }

        public void SaveDocument()
        {
            if (!CurrentDocument.HasFilename)
                throw new Exception("No filepath set for document yet.");

            new DocumentSaver().SaveToFile(CurrentDocument);
            CurrentDocument.MarkSaved();
            _messageBus.Publish(new CurrentDocumentSaved());
        }

        public void ChangeCurrentAnimation(ulong animationId)
        {
            RequireMode(EditorMode.Animate);
            ChangeCurrentAnimationInternal(animationId);
        }

        private void ChangeCurrentAnimationInternal(ulong animationId)
        {
            _currentAnimationId = animationId;

            if (Mode != EditorMode.Animate)
                return;

            CurrentDocument.UpdateSceneForAnimationCurrentFrame(_currentAnimationId);

            // todo probably replace this with a FullReload() concept to do clean switch to different data
            if (_isAutoKeying)
                ToggleAutoKeying();
            KeySelection.Clear();

            _messageBus.Publish(new CurrentAnimationChanged(_currentAnimationId));
        }

        public void ToggleAutoKeying()
        {
            RequireMode(EditorMode.Animate);

            SetAutoKeying(!_isAutoKeying);
        }

        public bool IsAutoKeying => _isAutoKeying;

        private void SetAutoKeying(bool value)
        {
            if (_isAutoKeying == value)
                return;

            _isAutoKeying = value;
            _messageBus.Publish(new AutoKeyToggled(_isAutoKeying));
        }

        /// <summary>
        /// Checks if the given animation is the currenlty selected one in the editor.
        /// </summary>
        public bool IsCurrentAnimation(ulong animationId)
        {
            return _currentAnimationId == animationId;
        }

        public void ChangeEditorMode(EditorMode mode)
        {
            if (Mode == mode)
                return;

            // undo/redo is weird when you don't include Mode switches in history too. Else you can undo things that you currently can't see.
            using var uow = CurrentDocument.StartUnitOfWork($"Enter mode {mode}");
            CurrentDocument.SetMetaData(uow, "mode", Mode, mode);
        }

        public void ChangeEditorTool(EditorTool tool)
        {
            if (Tool == tool)
                return;

            DoChangeEditorTool(tool);
        }

        private void DoChangeEditorMode(EditorMode mode)
        {
            Mode = mode;
            
            switch (mode)
            {
                case EditorMode.Design:
                    ChangeEditorTool(EditorTool.Modify);
                    SetAutoKeying(false);
                    KeySelection.Clear();
                    break;
                case EditorMode.Animate:
                    ChangeEditorTool(EditorTool.Pose);
                    CurrentDocument.ApplyCurrentAnimationFrameToScene(_currentAnimationId);
                    break;
            }

            _messageBus.Publish(new EditorModeChanged(mode));
        }

        private void DoChangeEditorTool(EditorTool tool)
        {
            Tool = tool;
            _messageBus.Publish(new EditorToolChanged(tool));
        }

        private void RequireMode(EditorMode mode)
        {
            if (Mode != mode)
                throw new UserActionException($"Can only do that in {mode} mode.");
        }

        public Animations.Animation GetCurrentAnimation()
        {
            return CurrentDocument.GetAnimation(_currentAnimationId);
        }

        public bool BelongsToCurrentAnimation(in ulong propertyAnimationId)
        {
            var propertyAnimation = CurrentDocument.GetPropertyAnimation(propertyAnimationId);
            return propertyAnimation.AnimationId == _currentAnimationId;
        }

        public Transformation GetNodeTransformation(in ulong nodeId)
        {
            var node = CurrentDocument.GetNode(nodeId);
            switch (Mode)
            {
                case EditorMode.Animate:
                    return node.GetAnimateTransformation();
                case EditorMode.Design:
                    return node.GetDesignTransformation();
                default: throw new NotSupportedException($"Unknown editor mode: {Mode}");
            }
        }

        public void SelectNodeAndChangeToModifyTool(in ulong nodeId)
        {
            if (NodeSelection.Contains(nodeId) && NodeSelection.Count == 1)
                return;

            if (Mode == EditorMode.Design)
                ChangeEditorTool(EditorTool.Modify); // revert to modify tool, you just selected something. eg. keep a current drawing tool feels wrong.
            NodeSelection.SelectSingle(nodeId);
        }

        public float GetNodeProperty(in ulong nodeId, PropertyType propertyType)
        {
            var node = CurrentDocument.GetNode(nodeId);
            var property = node.GetProperty(propertyType);

            switch (Mode)
            {
                case EditorMode.Design:
                {
                    return property.DesignVisualValue;
                }
                case EditorMode.Animate:
                {
                    return property.AnimateVisualValue;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool GetNodePropertyAsBool(in ulong nodeId, PropertyType propertyType)
        {
            return Property.ValueToBool(GetNodeProperty(nodeId, propertyType));
        }

        public void ChangeToDefaultEditorTool()
        {
            ChangeEditorTool(GetDefaultEditorTool());
            
        }

        private EditorTool GetDefaultEditorTool()
        {
            switch (Mode)
            {
                case EditorMode.Design:
                    return EditorTool.Modify;
                case EditorMode.Animate:
                    return EditorTool.Pose;
                default:
                    throw new NotSupportedException();
            }
        }

        public bool IsInDefaultEditorTool => Tool == GetDefaultEditorTool();

        public ISelection<ulong> NodeSelection { get; }

        public ISelection<ulong> KeySelection { get; }

        public bool HasDocument => _currentDocument != null;

        public EditorMode Mode { get; private set; }

        public EditorTool Tool { get; private set; }
        public ulong CurrentAnimationId => _currentAnimationId;

        /// <summary>
        /// Clones the current <see cref="Document"/>, returning a completely isolated but operational copy that publishes messages to the provided <see cref="IMessageBus"/> instead of the default application messagebus.
        /// </summary>
        public Document CloneDocument(IMessageBus messageBus)
        {
            var protoDocument = ProtoModelBuilder.CreateProtobufDocument(CurrentDocument);
            return DomainModelBuilder.CreateDocument(messageBus, protoDocument, CurrentDocument.Filename);
        }
    }
}