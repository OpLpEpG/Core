using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

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

    public class FormResource
    {
        private static ResourceDictionary _dictionary;
        public static ResourceDictionary Dictionary => _dictionary;
        static FormResource()
        {
            _dictionary = new ResourceDictionary();

            _dictionary.Source = new Uri("Core;component/ViewResourceForms.xaml", UriKind.Relative);
        }
        public static object? Get(object item, string suffix) 
        {
            if (item != null)
            {
                Type? type = item.GetType();

                while (type != null)
                {
                    if (_dictionary.Contains(type.Name + suffix))
                    {
                        return _dictionary[type.Name + suffix];
                    }
                    type = type.BaseType;
                }
            }
            return null;
        }
    }

    public class PanesStyleSelector : StyleSelector
    {
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            var res = FormResource.Get(item, "Style");
            return (res != null)?(Style) res : base.SelectStyle(item, container);
        }
    }
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var res = FormResource.Get(item, "Template");
            return (res != null) ? (DataTemplate)res : base.SelectTemplate(item, container);
        }
    }

    public abstract class FileMenuItemView : MenuItem
    {
        public FileDialog? FileDialog { get; set; }
        public FileMenuItemView()
        {
            Click += (o, e) =>
            {
                SetupDialog();
                if (FileDialog?.ShowDialog() == true)
                {
                    (this.DataContext as MenuFileVM)?.OnSelectFileAction?.Invoke(FileDialog.FileName);
                }
            };
        }
        protected virtual void SetupDialog()
        {
            if (FileDialog != null && DataContext is MenuFileVM mf)
            {
                FileDialog.Title = mf.Title ?? FileDialog.Title;
                FileDialog.InitialDirectory = mf.InitialDirectory ?? FileDialog.InitialDirectory;
                FileDialog.AddExtension = mf.AddExtension;
                FileDialog.CheckFileExists = mf.CheckFileExists;
                FileDialog.CheckPathExists = mf.CheckPathExists;
                FileDialog.DefaultExt = mf.DefaultExt ?? FileDialog.DefaultExt;
                FileDialog.Filter = mf.Filter ?? FileDialog.Filter;
                FileDialog.ValidateNames = mf.ValidateNames;
                if (mf.CustomPlaces != null)
                {
                    FileDialog.CustomPlaces = Array.ConvertAll(mf.CustomPlaces.ToArray(), (o) =>
                    {
                        if (o is string s) return new FileDialogCustomPlace(s);
                        else if (o is Guid g) return new FileDialogCustomPlace(g);
                        else return new FileDialogCustomPlace("no name");
                    });
                }
            }
        }
    }
    public class FileOpenMenuItemView : FileMenuItemView
    {
        protected override void SetupDialog()
        {
            this.FileDialog = new OpenFileDialog();
            base.SetupDialog();
            if (DataContext is MenuOpenFileVM m && FileDialog is OpenFileDialog f)
            {
                f.ReadOnlyChecked = m.ReadOnlyChecked;
                f.ShowReadOnly = m.ShowReadOnly;
            }
        }
    }
    public class FileSaveMenuItemView : FileMenuItemView
    {
        protected override void SetupDialog()
        {
            this.FileDialog = new SaveFileDialog();
            base.SetupDialog();
            if (DataContext is MenuSaveFileVM m && FileDialog is SaveFileDialog f)
            {
                f.CreatePrompt = m.CreatePrompt;
                f.OverwritePrompt = m.OverwritePrompt;
            }
        }
    }

}
