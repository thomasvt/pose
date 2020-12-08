using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pose.Domain.Documents.Messages;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Framework.Messaging;

namespace Pose.Panels.Animations
{
    public class AnimationsPanelViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private AnimationViewModel _selectedAnimation;
        private readonly Dictionary<ulong, AnimationViewModel> _index;

        private bool _canDeleteAnimation;

        public AnimationsPanelViewModel(Editor editor)
        {
            _index = new Dictionary<ulong, AnimationViewModel>();
            _editor = editor;
            Animations = new ObservableCollection<AnimationViewModel>();
            CanDeleteAnimation = false;

            MessageBus.Default.Subscribe<DocumentLoaded>(OnDocumentLoaded);
            MessageBus.Default.Subscribe<CurrentAnimationChanged>(OnCurrentAnimationChanged);
            MessageBus.Default.Subscribe<AnimationAdded>(OnAnimationAdded);
            MessageBus.Default.Subscribe<AnimationRemoved>(OnAnimationRemoved);
        }

        public void AddNewAnimation()
        {
            _editor.AddNewAnimation();
        }

        public void RemoveSelectedAnimation()
        {
            if (!CanDeleteAnimation)
                return; 

            _editor.RemoveSelectedAnimation();
        }

        private void OnDocumentLoaded(DocumentLoaded msg)
        {
            Animations.Clear();
            _index.Clear();
            foreach (var animation in msg.Document.GetAnimations())
            {
                AddAnimation(animation.Id, animation.Name);
            }
            _selectedAnimation = null;
        }

        private void OnCurrentAnimationChanged(CurrentAnimationChanged msg)
        {
            SelectedAnimation = _index[msg.AnimationId];
        }

        private void OnAnimationAdded(AnimationAdded msg)
        {
            AddAnimation(msg.AnimationId, msg.Name);
        }

        private void AddAnimation(ulong animationId, string name)
        {
            var animationViewModel = MapAnimationViewModel(animationId, name);
            var i = 0;
            for (; i < Animations.Count; i++)
            {
                var compare = Animations[i].Label.ToLower().CompareTo(animationViewModel.Label.ToLower());
                if (compare > 0)
                    break;
            }
            Animations.Insert(i, animationViewModel);
            _index.Add(animationId, animationViewModel);
            UpdateCanDeleteAnimation();
        }

        private void OnAnimationRemoved(AnimationRemoved msg)
        {
            Animations.Remove(_index[msg.AnimationId]);
            _index.Remove(msg.AnimationId);
            UpdateCanDeleteAnimation();
        }

        private static AnimationViewModel MapAnimationViewModel(ulong animationId, string name)
        {
            return new AnimationViewModel(animationId, name);
        }

        private void UpdateCanDeleteAnimation()
        {
            CanDeleteAnimation = Animations.Count > 1;
        }

        public AnimationViewModel SelectedAnimation
        {
            get => _selectedAnimation;
            set
            {
                if (_selectedAnimation == value)
                    return;

                _selectedAnimation = value;
                OnPropertyChanged();

                _editor.ChangeCurrentAnimation(value.AnimationId);
            }
        }

        public bool CanDeleteAnimation
        {
            get => _canDeleteAnimation;
            set
            {
                if (_canDeleteAnimation == value)
                    return;

                _canDeleteAnimation = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AnimationViewModel> Animations { get; set; }
    }
}
