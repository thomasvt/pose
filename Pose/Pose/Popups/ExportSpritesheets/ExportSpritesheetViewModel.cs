using System.Windows;
using System.Windows.Media;
using Pose.Domain.Editor;
using Pose.SceneEditor.Viewport;

namespace Pose.Popups.ExportSpritesheets
{
    public class ExportSpritesheetViewModel
        : ViewModel
    {
        private readonly ISpriteProducer _spriteProducer;
        private readonly Editor _editor;
        private ExportSpritesheetWindow _window;
        private ImageSource _previewImage;
        private double _scale;
        private string _dpiLabel;
        private double _bitmapScaleX;
        private double _bitmapScaleY;
        private double _dpiX;
        private double _dpiY;
        private bool _isHighDpiCorrected;

        public ExportSpritesheetViewModel(ISpriteProducer spriteProducer, Editor editor)
        {
            _spriteProducer = spriteProducer;
            _editor = editor;
            BitmapScaleX = 1d;
            BitmapScaleY = 1d;
        }

        public void Initialize(SceneViewport sceneViewport)
        {
            
            var source = PresentationSource.FromVisual(sceneViewport);
            _dpiX = 96f * source.CompositionTarget.TransformToDevice.M11;
            _dpiY = 96f * source.CompositionTarget.TransformToDevice.M22;
            DpiLabel = $"{_dpiX}x{_dpiY}";

            _spriteProducer.SceneViewport = sceneViewport;
            _spriteProducer.PrepareDocument();
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            PreviewImage = _spriteProducer.ProduceFirstAnimationFrame(_editor.CurrentAnimationId);
        }

        public void ShowModal()
        {
            _window = new ExportSpritesheetWindow
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };
            var result = _window.ShowDialog();
            if (result == true)
            {
                ExportSpritesheet();
            }
        }

        public void SetScale(in float scale)
        {
            Scale = scale;
            _spriteProducer.Scale = scale;
            UpdatePreview();
        }

        private void ExportSpritesheet()
        {
            _spriteProducer.ProduceAnimationFrames(_editor.CurrentAnimationId);
        }

        public ImageSource PreviewImage
        {
            get => _previewImage;
            set
            {
                if (Equals(value, _previewImage)) return;
                _previewImage = value;
                OnPropertyChanged();
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                if (value.Equals(_scale)) return;
                _scale = value;
                OnPropertyChanged();
            }
        }

        public double BitmapScaleX
        {
            get => _bitmapScaleX;
            set
            {
                if (value.Equals(_bitmapScaleX)) return;
                _bitmapScaleX = value;
                OnPropertyChanged();
            }
        }

        public double BitmapScaleY
        {
            get => _bitmapScaleY;
            set
            {
                if (value.Equals(_bitmapScaleY)) return;
                _bitmapScaleY = value;
                OnPropertyChanged();
            }
        }

        public string DpiLabel
        {
            get => _dpiLabel;
            set
            {
                if (value == _dpiLabel) return;
                _dpiLabel = value;
                OnPropertyChanged();
            }
        }

        public bool IsHighDpiCorrected 
        {
            get => _isHighDpiCorrected;
            set
            {
                if (value == _isHighDpiCorrected) return;
                _isHighDpiCorrected = value;
                OnPropertyChanged();
                BitmapScaleX = value ? 96d / _dpiX : 1f;
                BitmapScaleY = value ? 96d / _dpiY : 1f;
            }
        }
    }
}
