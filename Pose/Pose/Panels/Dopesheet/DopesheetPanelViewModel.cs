using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using Pose.Controls.Dopesheet;
using Pose.Domain.Animations.Messages;
using Pose.Domain.Editor;
using Pose.Domain.Editor.Messages;
using Pose.Domain.Nodes.Properties;
using Pose.Framework;
using Pose.Framework.Messaging;

namespace Pose.Panels.Dopesheet
{
    public class DopesheetPanelViewModel
    : ViewModel
    {
        /// We don't use databinding for the rows and keys in those rows. I tried but failed. Somehow the databound keys inside the databound rows get cleared after being set correctly.
        /// So I gave up as this is a dedicated control and databinding is not that essential. We create and manipulate the WPF controls for rows and keys directly.

        private Brush _recordButtonBrushOn, _transparentBrush;
        private readonly Editor _editor;
        private readonly Dictionary<ulong, DopesheetRow> _rowsPerPropertyAnimationIdIndex;
        private readonly TwowayIndex<ulong, TimelineKey> _keyIndex;
        private int _currentFrame;
        private int _beginFrame;
        private int _endFrame;
        private Brush _recordButtonBrush;
        private RealtimeAnimationPlayer _realtimeAnimationPlayer;
        private bool _isPlaying;
        private bool _isNotPlaying;
        private bool _isLoop;
        private ObservableCollection<DopesheetRow> _rows;

        public DopesheetPanelViewModel(Editor editor)
        {
            _editor = editor;
            _rowsPerPropertyAnimationIdIndex = new Dictionary<ulong, DopesheetRow>();
            Rows = new ObservableCollection<DopesheetRow>();
            IsNotPlaying = true;
            _keyIndex = new TwowayIndex<ulong, TimelineKey>();

            MessageBus.Default.Subscribe<PropertyAnimationAdded>(OnPropertyAnimationAdded);
            MessageBus.Default.Subscribe<PropertyAnimationRemoved>(OnPropertyAnimationRemoved);
            MessageBus.Default.Subscribe<CurrentAnimationChanged>(OnCurrentAnimationChanged);
            MessageBus.Default.Subscribe<EditorModeChanged>(OnEditorModeChanged);
            MessageBus.Default.Subscribe<AnimationKeyAdded>(OnAnimationKeyAdded);
            MessageBus.Default.Subscribe<AnimationKeyRemoved>(OnAnimationKeyRemoved);
            MessageBus.Default.Subscribe<AnimationCurrentFrameChanged>(OnCurrentAnimationFrameChanged);
            MessageBus.Default.Subscribe<AnimationBeginFrameChanged>(OnAnimationBeginFrameChanged);
            MessageBus.Default.Subscribe<AnimationEndFrameChanged>(OnAnimationEndFrameChanged);
            MessageBus.Default.Subscribe<KeySelected>(OnKeySelected);
            MessageBus.Default.Subscribe<KeyDeselected>(OnKeyDeselected);
            MessageBus.Default.Subscribe<AutoKeyToggled>(OnAutoKeyToggled);
            MessageBus.Default.Subscribe<AnimationIsLoopChanged>(OnAnimationIsLoopChanged);
            MessageBus.Default.Subscribe<NodeSelected>(m => UpdateRowHighlighting());
            MessageBus.Default.Subscribe<NodeDeselected>(m => UpdateRowHighlighting());

            ConfigureButtonBrushes();
        }


        private void UpdateRowHighlighting()
        {
            foreach (var row in Rows)
            {
                row.IsHighlighted = row.NodeId == _editor.NodeSelection.FirstOrDefault(0);
            }
        }

        private void OnAnimationIsLoopChanged(AnimationIsLoopChanged msg)
        {
            if (_editor.IsCurrentAnimation(msg.AnimationId))
            {
                IsLoop = msg.IsLoop;
            }
        }

        private void ConfigureButtonBrushes()
        {
            _recordButtonBrushOn = MakeBrush("#bb2222");
            _transparentBrush = MakeBrush("#00000000");
        }

        private static Brush MakeBrush(string color)
        {
            var brush = new SolidColorBrush(ColorUtils.FromHex(color));
            brush.Freeze();
            return brush;
        }

        private void Clear()
        {
            Rows.Clear();
            _rowsPerPropertyAnimationIdIndex.Clear();
            _keyIndex.Clear();
        }

        private void OnAutoKeyToggled(AutoKeyToggled msg)
        {
            RecordButtonBrush = msg.IsAutoKeying ? _recordButtonBrushOn : _transparentBrush;
        }

        private void OnKeySelected(KeySelected msg)
        {
            _keyIndex[msg.KeyId].IsSelected = true;
        }

        private void OnKeyDeselected(KeyDeselected msg)
        {
            _keyIndex[msg.KeyId].IsSelected = false;
        }

        private void OnPropertyAnimationAdded(PropertyAnimationAdded msg)
        {
            if (!_editor.IsCurrentAnimation(msg.AnimationId))
                return;

            AddPropertyAnimation(msg.PropertyAnimationId, msg.NodeId, msg.Property);
        }

        private void AddPropertyAnimation(ulong propertyAnimationId, ulong nodeId, PropertyType propertyType)
        {
            var node = _editor.CurrentDocument.GetNode(nodeId);
            var header = $"[{node.Name}] {GetPropertyLabel(propertyType)}";
            var row = new DopesheetRow
            {
                Header = header,
                SortingText = header,
                NodeId = nodeId,
                IsHighlighted = _editor.NodeSelection.FirstOrDefault(0) == nodeId
            };
            _rowsPerPropertyAnimationIdIndex.Add(propertyAnimationId, row);
            Rows.SortedInsert(row, (a, b) => string.Compare(a.SortingText, b.SortingText, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetPropertyLabel(PropertyType propertyType)
        {
            return propertyType switch
            {
                PropertyType.TranslationX => "X",
                PropertyType.TranslationY => "Y",
                PropertyType.RotationAngle => "Angle",
                PropertyType.Visibility => "Visible",
                _ => propertyType.ToString()
            };
        }

        private void OnPropertyAnimationRemoved(PropertyAnimationRemoved msg)
        {
            if (!_editor.BelongsToCurrentAnimation(msg.PropertyAnimationId))
                return;

            if (!_rowsPerPropertyAnimationIdIndex.ContainsKey(msg.PropertyAnimationId))
                return;

            Rows.Remove(_rowsPerPropertyAnimationIdIndex[msg.PropertyAnimationId]);
            _rowsPerPropertyAnimationIdIndex.Remove(msg.PropertyAnimationId);
        }

        private void OnAnimationKeyAdded(AnimationKeyAdded msg)
        {
            if (!_editor.BelongsToCurrentAnimation(msg.PropertyAnimationId))
                return;

            if (!_rowsPerPropertyAnimationIdIndex.ContainsKey(msg.PropertyAnimationId))
                throw new Exception($"There's no dopesheet row for PropertyAnimation ({msg.PropertyAnimationId}).");

            AddKey(msg.KeyId, msg.PropertyAnimationId, msg.Frame);
        }

        private void AddKey(ulong keyId, ulong propertyAnimationId, int frame)
        {
            var row = _rowsPerPropertyAnimationIdIndex[propertyAnimationId];
            var timelineKey = new TimelineKey { Frame = frame };
            timelineKey.MouseDown += TimelineKeyOnMouseDown;
            row.Items.Add(timelineKey);
            _keyIndex.Add(keyId, timelineKey);
        }

        private void TimelineKeyOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                _editor.KeySelection.ToggleSelection(_keyIndex[sender as TimelineKey]);
            }
            else
            {
                _editor.KeySelection.SelectSingle(_keyIndex[sender as TimelineKey]);
            }
        }

        private void OnEditorModeChanged(EditorModeChanged msg)
        {
            if (msg.Mode == EditorMode.Animate)
            {
                LoadCurrentAnimation();
            }
        }

        private void OnCurrentAnimationChanged(CurrentAnimationChanged msg)
        {
            LoadCurrentAnimation();
        }

        /// <summary>
        /// Updates the entire dopesheet panel with the domain's current animation.
        /// </summary>
        private void LoadCurrentAnimation()
        {
            Clear();
            var animation = _editor.GetCurrentAnimation();
            CurrentFrame = animation.CurrentFrame;
            BeginFrame = animation.BeginFrame;
            EndFrame = animation.EndFrame;
            IsLoop = animation.IsLoop;
            // add rows and keys to dopesheet:
            foreach (var propertyAnimation in animation.PropertyAnimations)
            {
                AddPropertyAnimation(propertyAnimation.Id, propertyAnimation.NodeId, propertyAnimation.Property);
                foreach (var key in propertyAnimation.Keys)
                {
                    AddKey(key.Id, key.PropertyAnimationId, key.Frame);
                }
            }
        }

        private void OnAnimationKeyRemoved(AnimationKeyRemoved msg)
        {
            if (!_rowsPerPropertyAnimationIdIndex.ContainsKey(msg.PropertyAnimationId))
                return;

            var row = _rowsPerPropertyAnimationIdIndex[msg.PropertyAnimationId];
            var key = _keyIndex[msg.KeyId];
            key.MouseDown -= TimelineKeyOnMouseDown;
            _keyIndex.Remove(msg.KeyId);
            row.Items.Remove(key);
        }

        private void OnCurrentAnimationFrameChanged(AnimationCurrentFrameChanged msg)
        {
            if (!_editor.IsCurrentAnimation(msg.AnimationId))
                return;

            CurrentFrame = msg.Frame;
        }

        private void OnAnimationBeginFrameChanged(AnimationBeginFrameChanged msg)
        {
            if (!_editor.IsCurrentAnimation(msg.AnimationId))
                return;

            BeginFrame = msg.BeginFrame;
        }

        private void OnAnimationEndFrameChanged(AnimationEndFrameChanged msg)
        {
            if (!_editor.IsCurrentAnimation(msg.AnimationId))
                return;

            EndFrame = msg.EndFrame;
        }

        public void OnAnimationBeginFrameCommitted(int initialValue, int newValue, bool isTransient)
        {
            if (isTransient)
            {
                _editor.ChangeCurrentAnimationBeginFrameTransient(newValue);
            }
            else
            {
                _editor.ChangeCurrentAnimationBeginFrame(initialValue, newValue);
            }
        }

        public void OnAnimationEndFrameCommitted(int initialValue, int newValue, bool isTransient)
        {
            if (isTransient)
            {
                _editor.ChangeCurrentAnimationEndFrameTransient(newValue);
            }
            else
            {
                _editor.ChangeCurrentAnimationEndFrame(initialValue, newValue);
            }
        }

        public void TogglePlay()
        {
            IsPlaying = !IsPlaying;
            IsNotPlaying = !IsPlaying;

            if (IsPlaying)
            {
                JumpToBeginIfAtEnd(); // this allows repeatedly clicking play when !isLoop.
                _realtimeAnimationPlayer = new RealtimeAnimationPlayer(_editor);
                _realtimeAnimationPlayer.EndReached += TogglePlay;
                _realtimeAnimationPlayer.Play();
            }
            else
            {
                _realtimeAnimationPlayer.Dispose();
                _realtimeAnimationPlayer = null;
            }
        }

        private void JumpToBeginIfAtEnd()
        {
            var animation = _editor.GetCurrentAnimation();
            if (animation.CurrentFrame == animation.EndFrame)
            {
                _editor.ChangeCurrentAnimationCurrentFrameTransient(animation.BeginFrame);
            }
        }

        public void JumpToAnimationBegin()
        {
            if (_realtimeAnimationPlayer == null)
            {
                _editor.ChangeCurrentAnimationCurrentFrameTransient(_editor.GetCurrentAnimation().BeginFrame);
            }
            else
            {
                _realtimeAnimationPlayer.JumpToAnimationBegin();
            }
        }

        public void RowClicked(DopesheetRow row)
        {
            _editor.NodeSelection.SelectSingle(row.NodeId);
        }

        public void ToggleAutoKeying()
        {
            _editor.ToggleAutoKeying();
        }

        public bool IsLoop
        {
            get => _isLoop;
            set
            {
                if (value == _isLoop) return;
                _isLoop = value;
                OnPropertyChanged();

                if (_editor.GetCurrentAnimation().IsLoop != value)
                    _editor.ToggleCurrentAnimationIsLoop();
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (value == _isPlaying) return;
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotPlaying
        {
            get => _isNotPlaying;
            set
            {
                if (value == _isNotPlaying) return;
                _isNotPlaying = value;
                OnPropertyChanged();
            }
        }

        public int CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (value == _currentFrame) return;
                _currentFrame = value;
                OnPropertyChanged();
                _editor.ChangeCurrentAnimationCurrentFrameTransient(value);
            }
        }

        public int BeginFrame
        {
            get => _beginFrame;
            set
            {
                if (value == _beginFrame) return;
                _beginFrame = value;
                OnPropertyChanged();
            }
        }

        public int EndFrame
        {
            get => _endFrame;
            set
            {
                if (value == _endFrame) return;
                _endFrame = value;
                OnPropertyChanged();
            }
        }

        public Brush RecordButtonBrush
        {
            get => _recordButtonBrush;
            set
            {
                if (value == _recordButtonBrush) return;
                _recordButtonBrush = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<DopesheetRow> Rows
        {
            get => _rows;
            set
            {
                if (Equals(value, _rows)) return;
                _rows = value;
                OnPropertyChanged();
            }
        }
    }
}
