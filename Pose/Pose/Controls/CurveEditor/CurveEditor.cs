using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Pose.Common;
using Pose.Common.Curves;
using Pose.Domain;

namespace Pose.Controls.CurveEditor
{
    [TemplatePart(Name = "Path", Type = typeof(Path))]
    [TemplatePart(Name = "HandleALine", Type = typeof(Line))]
    [TemplatePart(Name = "HandleBLine", Type = typeof(Line))]
    [TemplatePart(Name = "HandleA", Type = typeof(BezierHandle))]
    [TemplatePart(Name = "HandleB", Type = typeof(BezierHandle))]
    [TemplatePart(Name = "Canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "CurveOptionsComboBox", Type= typeof(ComboBox))]
    public class CurveEditor : Control
    {
        private const double GraphMarginV = 32; // the vertical margin outside [0,1] domain of Y axis, to support curve overshooting.
        private const double GraphWidth = 128;
        private const double GraphHeigth = 160 - GraphMarginV * 2;

        private Path _path;
        private Canvas _canvas;
        private BezierHandle _handleA;
        private BezierHandle _handleB;
        private Line _handleALine;
        private Line _handleBLine;
        private BezierHandle _draggedHandle;
        private Point _dragStartMousePosition;
        private Point _initialPosition;
        private ComboBox _curveOptionsComboBox;

        static CurveEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CurveEditor), new FrameworkPropertyMetadata(typeof(CurveEditor)));
        }

        public CurveEditor()
        {
            BezierCurve = new BezierCurve(Vector2.Zero, new Vector2(0.5f, 0), new Vector2(0.5f, 1), Vector2.One);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _path = GetTemplateChild("Path") as Path;
            _canvas = GetTemplateChild("Canvas") as Canvas;

            _handleA = GetTemplateChild("HandleA") as BezierHandle;
            

            _handleB = GetTemplateChild("HandleB") as BezierHandle;

            _handleALine = GetTemplateChild("HandleALine") as Line;
            _handleBLine = GetTemplateChild("HandleBLine") as Line;

            ConfigureBezierHandle(_handleA, _handleALine);
            ConfigureBezierHandle(_handleB, _handleBLine);

            _curveOptionsComboBox = GetTemplateChild("CurveOptionsComboBox") as ComboBox;
            _curveOptionsComboBox.ItemsSource = new List<CurveType> {CurveType.Linear, CurveType.Hold, CurveType.Bezier};

            UpdateForCurveType();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateCurveControl();
        }

        private void ConfigureBezierHandle(BezierHandle handle, Line handleLine)
        {
            handle.MouseLeftButtonDown += (sender, args) =>
            {
                if (IsReadOnly)
                    return;

                _initialPosition = new Point((double)handle.GetValue(Canvas.LeftProperty) + 4d, (double)handle.GetValue(Canvas.TopProperty) + 4d);
                _dragStartMousePosition = args.GetPosition(this);
                _draggedHandle = handle;
                Mouse.Capture(this);
            };
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_draggedHandle == null)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var mousePosition = e.GetPosition(this);

            var x = (float)GetGridValue(_initialPosition.X + mousePosition.X - _dragStartMousePosition.X, _canvas.ActualWidth - 1);
            var y = (float)GetGridValue(_initialPosition.Y + mousePosition.Y - _dragStartMousePosition.Y, _canvas.ActualHeight - 1);

            // convert to [0,1] and update BezierCurve property.
            var p = new Vector2((float)(x / GraphWidth), (float)(1f - (y - GraphMarginV) / GraphHeigth));
            var isHandleA = _draggedHandle == _handleA;
            
            BezierCurve = new BezierCurve(
                BezierCurve.P0,
                isHandleA ? p : BezierCurve.P1,
                isHandleA ? BezierCurve.P2 : p,
                BezierCurve.P3);
        }

        /// <summary>
        /// Coerces the value within bounds and snaps to a grid of 4px cells.
        /// </summary>
        private static double GetGridValue(double value, double max)
        {
            if (value < 0) value = 0;
            if (value > max) value = max;
            return Math.Round(value / 4, MidpointRounding.AwayFromZero) * 4;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_draggedHandle == null)
                return;

            ReleaseMouseCapture();
            _draggedHandle = null;
            BezierHandleReleased?.Invoke();
        }

        private void UpdateForCurveType()
        {
            EnableBezierHandles(CurveType == CurveType.Bezier);
            UpdateCurveControl();
        }

        private void EnableBezierHandles(bool isEnabled)
        {
            var visibility = isEnabled ? Visibility.Visible : Visibility.Collapsed;
            
            _handleA.IsEnabled = isEnabled;
            _handleA.Visibility = visibility;
            _handleALine.Visibility = visibility;

            _handleB.IsEnabled = isEnabled;
            _handleB.Visibility = visibility;
            _handleBLine.Visibility = visibility;
        }

        private void UpdateCurveControl()
        {
            if (_path == null)
                return;

            switch (CurveType)
            {
                case CurveType.Linear:
                    _path.Data = CurveGeometryBuilder.BuildLinear(GraphMarginV, _canvas.ActualWidth, _canvas.ActualHeight - GraphMarginV * 2);
                    break;
                case CurveType.Bezier:
                    if (BezierCurve.P0 != Vector2.Zero || BezierCurve.P3 != Vector2.One)
                        return; // don't draw if curve is invalid.

                    _path.Data = CurveGeometryBuilder.BuildBezier(BezierCurve, BezierCurveSegmentCount, GraphMarginV, _canvas.ActualWidth, _canvas.ActualHeight - GraphMarginV * 2);
                    UpdateBezierHandle(_handleA, _handleALine, BezierCurve.P1);
                    UpdateBezierHandle(_handleB, _handleBLine, BezierCurve.P2);
                    break;
                case CurveType.Hold:
                    _path.Data = CurveGeometryBuilder.BuildHold(GraphMarginV, _canvas.ActualWidth, _canvas.ActualHeight - GraphMarginV * 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateBezierHandle(BezierHandle handle, Line handleLine, in Vector2 location)
        {
            var x = location.X * GraphWidth;
            var y = GraphMarginV + (1 - location.Y) * GraphHeigth;

            handle.SetValue(Canvas.LeftProperty, x - 4d);
            handle.SetValue(Canvas.TopProperty, y - 4d);
            handleLine.X2 = x;
            handleLine.Y2 = y;
        }

        public static readonly DependencyProperty BezierCurveProperty = DependencyProperty.Register(
            "BezierCurve", typeof(BezierCurve), typeof(CurveEditor), new FrameworkPropertyMetadata(default(BezierCurve), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            {
                PropertyChangedCallback = (o, args) => (o as CurveEditor).UpdateCurveControl()
            });

        public BezierCurve BezierCurve
        {
            get => (BezierCurve) GetValue(BezierCurveProperty);
            set => SetValue(BezierCurveProperty, value);
        }

        public static readonly DependencyProperty LabelColumnWidthProperty = DependencyProperty.Register(
            "LabelColumnWidth", typeof(GridLength), typeof(CurveEditor), new PropertyMetadata(default(GridLength)));


        public GridLength LabelColumnWidth
        {
            get => (GridLength) GetValue(LabelColumnWidthProperty);
            set => SetValue(LabelColumnWidthProperty, value);
        }

        public static readonly DependencyProperty BezierCurveSegmentCountProperty = DependencyProperty.Register(
            "BezierCurveSegmentCount", typeof(int), typeof(CurveEditor), new PropertyMetadata(default(int)));

        public int BezierCurveSegmentCount
        {
            get => (int) GetValue(BezierCurveSegmentCountProperty);
            set => SetValue(BezierCurveSegmentCountProperty, value);
        }

        public static readonly DependencyProperty CurveTypeProperty = DependencyProperty.Register(
            "CurveType", typeof(CurveType), typeof(CurveEditor), new FrameworkPropertyMetadata(default(CurveType), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            {
                PropertyChangedCallback = (o, args) =>
                {
                    var curveEditor = (o as CurveEditor);
                    curveEditor.UpdateForCurveType();
                }
            });

        public CurveType CurveType
        {
            get => (CurveType) GetValue(CurveTypeProperty);
            set => SetValue(CurveTypeProperty, value);
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(CurveEditor), new PropertyMetadata(default(bool)));

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public event Action BezierHandleReleased;
    }
}
