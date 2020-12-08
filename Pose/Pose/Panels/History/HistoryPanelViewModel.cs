using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Domain.History.Messages;
using Pose.Framework.Messaging;

namespace Pose.Panels.History
{
    public class HistoryPanelViewModel
    : ViewModel
    {
        private readonly Editor _editor;
        private readonly Dictionary<ulong, HistoryItemViewModel> _index;
        private ObservableCollection<HistoryItemViewModel> _items;
        private ulong? _currentVersion;

        public HistoryPanelViewModel(Editor editor)
        {
            _editor = editor;
            _items = new ObservableCollection<HistoryItemViewModel>();
            _index = new Dictionary<ulong, HistoryItemViewModel>();

            MessageBus.Default.Subscribe<DocumentLoaded>(OnDocumentLoaded);
            MessageBus.Default.Subscribe<HistoryItemCommitted>(OnHistoryItemCommitted);
            MessageBus.Default.Subscribe<HistoryCursorChanged>(OnHistoryCursorChanged);
            MessageBus.Default.Subscribe<HistoryRemovedAfter>(OnHistoryRemovedAfter);
        }

        private void OnDocumentLoaded(DocumentLoaded msg)
        {
            Clear();
        }

        private void Clear()
        {
            _currentVersion = null;
            Items.Clear();
            _index.Clear();
        }

        private void OnHistoryCursorChanged(HistoryCursorChanged msg)
        {
            if (_currentVersion.HasValue && _currentVersion.Value > 0)
            {
                _index[_currentVersion.Value].IsSelected = false;
            }

            _currentVersion = msg.Version;
            if (_currentVersion > 0)
            {
                _index[_currentVersion.Value].IsSelected = true;
            }
        }

        private void OnHistoryItemCommitted(HistoryItemCommitted msg)
        {
            var vm = new HistoryItemViewModel(msg.Version, msg.Label);
            vm.IsSelectedChanged += ItemOnIsSelectedChanged;
            _items.Add(vm);
            _index.Add(msg.Version, vm);
        }

        private void ItemOnIsSelectedChanged(object sender, EventArgs e)
        {
            var vm = sender as HistoryItemViewModel;
            if (vm.Version != _currentVersion)
            {
                _editor.NavigateHistoryTo(vm.Version);
            }
        }

        private void OnHistoryRemovedAfter(HistoryRemovedAfter msg)
        {
            var i = Items.Count - 1;
            while (i >= 0 && Items[i].Version >= msg.Version)
            {
                Items.RemoveAt(i--);
            }
        }

        public ObservableCollection<HistoryItemViewModel> Items
        {
            get => _items;
            set
            {
                if (value == _items) return;
                _items = value;
                OnPropertyChanged();
            }
        } 
    }
}
