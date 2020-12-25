using System;

namespace Pose.Panels.Animations
{
    public class AnimationViewModel
    : ViewModel
    {
        private string _name;
        private bool _isUpdating;

        public AnimationViewModel(ulong animationId, string name)
        {
            AnimationId = animationId;
            SetName(name);
        }

        public void SetName(string name)
        {
            _isUpdating = true;
            try
            {
                Name = name;
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public ulong AnimationId { get; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();

                if (_isUpdating)
                    return;

                NameChanged?.Invoke(value);
            }
        }

        public event Action<string> NameChanged;
    }
}
