using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Core
{
    public class ToolTemplateSelector : DataTemplateSelector
    {
        private static ResourceDictionary _dictionary;
        public static ResourceDictionary Dictionary => _dictionary;
        static ToolTemplateSelector()
        {
            _dictionary = new ResourceDictionary();

            _dictionary.Source = new Uri("Core;component/ViewResourceTools.xaml", UriKind.Relative);
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject parentItemsControl)
        {
            var name = item == null ? null : item.GetType().Name;
            if (name != null && _dictionary.Contains(name))
            {
                return (DataTemplate)_dictionary[name];
            }
            return null!;//error
        }
    }
    public class MenuTemplateSelector : ItemContainerTemplateSelector
    {
        private static ResourceDictionary _dictionary;
        public static ResourceDictionary Dictionary => _dictionary;
        static MenuTemplateSelector()
        {
            _dictionary = new ResourceDictionary();

            _dictionary.Source = new Uri("Core;component/ViewResourceMenus.xaml", UriKind.Relative);
        }
        public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
        {
            var name = item == null ? null : item.GetType().Name;
            if (name != null && _dictionary.Contains(name))
            {
                return (DataTemplate)_dictionary[name];
            }
            return null!;//error
        }
    }

}
