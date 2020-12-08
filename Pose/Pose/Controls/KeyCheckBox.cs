using System.Windows;
using System.Windows.Controls;

namespace Pose.Controls
{
    
    public class KeyCheckBox : CheckBox
    {
        static KeyCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyCheckBox), new FrameworkPropertyMetadata(typeof(KeyCheckBox)));
        }

        public static readonly DependencyProperty IsKeyableProperty = DependencyProperty.Register(
            "IsKeyable", typeof(bool), typeof(KeyCheckBox), new PropertyMetadata(default(bool)));

        public bool IsKeyable
        {
            get => (bool) GetValue(IsKeyableProperty);
            set => SetValue(IsKeyableProperty, value);
        }

        public static readonly DependencyProperty MatchesKeyValueProperty = DependencyProperty.Register(
            "MatchesKeyValue", typeof(bool?), typeof(KeyCheckBox), new PropertyMetadata(default(bool?)));

        public bool? MatchesKeyValue
        {
            get => (bool?) GetValue(MatchesKeyValueProperty);
            set => SetValue(MatchesKeyValueProperty, value);
        }
    }
}
