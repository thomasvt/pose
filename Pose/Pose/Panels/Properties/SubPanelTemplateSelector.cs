using System;
using System.Windows;
using System.Windows.Controls;

namespace Pose.Panels.Properties
{
    public class SubPanelTemplateSelector
    : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            var element = container as FrameworkElement;

            if (!(element.TryFindResource(item.GetType().Name) is DataTemplate template))
                throw new Exception($"To use a SubPanel of type \"{item.GetType().Name}\", a DataTemplate with that exact typename as key should exist.");

            return template;
        }
    }
}
