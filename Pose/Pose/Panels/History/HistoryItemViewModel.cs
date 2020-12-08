using System;

namespace Pose.Panels.History
{
    public class HistoryItemViewModel
    : ViewModel
    {
        private bool _isSelected;
        public ulong Version { get; }
        public string Label { get; }

        public HistoryItemViewModel(ulong version, string label)
        {
            Version = version;
            Label = label;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected)
                    return;
                _isSelected = value;
                OnPropertyChanged();
                IsSelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsSelectedChanged;
    }
}
