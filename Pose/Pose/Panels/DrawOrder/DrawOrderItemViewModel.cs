using System;

namespace Pose.Panels.DrawOrder
{
    public class DrawOrderItemViewModel
    : ViewModel
    {
        private string _name;
        private bool _isSelected;

        public ulong NodeId { get; internal set; }

        public string Name
        {
            get => _name;
            internal set
            {

                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                if (value)
                {
                    Selected?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Deselected?.Invoke(this, EventArgs.Empty);
                }
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsNodeVisible { get; set; }

        public event EventHandler Selected;
        public event EventHandler Deselected;
    }
}
