using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Core
{
    public interface IMenuItemClient
    {
        public void AddStaticMenus(IMenuItemServer s);
        //  public void RemoveAddStaticMenus();
    }
    ///// <summary>
    ///// основа меню сепараторы и меню
    ///// </summary>
    //public abstract class MenuItemBaseVM: VMBase
    //{
    //    public int Priority { get; set; } = 100;
    //    public int Group => Priority / 100;
    //}
    ///// <summary>
    ///// Separator появляется автоматически между разными группами 
    ///// (см Group Priority) (использовать IMenuItemServer)
    ///// </summary>
    //public class SeparatorVM : MenuItemBaseVM { }
    /// <summary>
    /// простое меню для подменю
    /// </summary>
    public class MenuItemVM : PriorityItemBase
    {
        
        #region Properties
        public ObservableCollection<PriorityItem> Items { get; private set; }
        public string? InputGestureText { get; set; }
        public bool IsCheckable { get; set; }

        #region Header
        private string _header = string.Empty;
        public string Header
        {
            get => _header; 
            set
            {
                if (value != _header)
                {
                    _header = value;
                    OnPropertyChanged(nameof(Header));
                }
            }
        }
        #endregion

        #region Icon

        private Image? _icon;
        public Image? Icon
        {
              
            get
            {
                if (_icon == null && !String.IsNullOrWhiteSpace(IconSource))
                {
                    _icon = new Image { Source = new BitmapImage(new Uri(IconSource)) };
                }
                return _icon;
            }
        }
        #endregion

        #region IsChecked
        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }
        #endregion

        #endregion

        public MenuItemVM():base() 
        {
            this.Items = new ObservableCollection<PriorityItem>();            
        }
    }
    /// <summary>
    ///  МЕНЮ команда
    /// </summary>
    public class CommandMenuItemVM : MenuItemVM
    {
        public ICommand? Command { get; set; }
    }
    /// <summary>
    /// меню с перехватом открытия подменю
    /// </summary>
    public class OnSubMenuOpenMenuItemVM : MenuItemVM
    {
        #region OnSubMenuAction
        public Action? OnSubMenuAction = null;

        private bool _isSubmenuOpen = false;
        public bool IsSubmenuOpen
        { 
            get { return _isSubmenuOpen; }
            set
            {
                if (!_isSubmenuOpen && value)
                {
                    OnSubMenuAction?.Invoke();
                }
                _isSubmenuOpen = value;
            }
        }
        #endregion
    }
    /// <summary>
    /// привазка моделей к представлению: 
    /// 
    /// SeparatorVM
    /// MenuItemVM
    /// CommandMenuItemVM
    /// OnSubMenuOpenMenuItemVM
    /// 
    /// имя модели = ключ представления в словаре
    /// 
    /// к Dictionary добавляем пользовательские представления  Dictionary.MergedDictionaries.Add(....) 
    /// TODO: написать сервис
    /// 
    /// </summary>
    //public class ItemContainerStyleSelectorVM : StyleSelector
    //{
    //    private static ResourceDictionary _dictionary;
    //    static ItemContainerStyleSelectorVM()
    //    {
    //        _dictionary = new ResourceDictionary();
    //        _dictionary.Source = new Uri("Core;component/MenusResource.xaml", UriKind.Relative);
    //    }
    //    public override Style SelectStyle(object item, DependencyObject container)
    //    {

    //        var name = item == null ? null : item.GetType().Name;
    //        if (name != null && _dictionary.Contains(name))
    //        {
    //            return (Style)_dictionary[name];
    //        }
    //        return null!; 
    //    }
    //}

    //public class ExMenuItem: MenuItem
    //{
    //    public ExMenuItem() { }
    //    protected override DependencyObject GetContainerForItemOverride()=> new ExMenuItem(); // Required to preserve the item type in all the hierarchy
    //    protected override bool IsItemItsOwnContainerOverride(object item)=> item is ExMenuItem;
    //    #region Dependency Properties: Command Target Parameter
    //    public static readonly DependencyProperty SubmenuOpenedCommandProperty =
    //        DependencyProperty.Register(
    //            "SubmenuOpenedCommand",
    //            typeof(ICommand),
    //            typeof(ExMenuItem));
    //    public ICommand SubmenuOpenedCommand
    //    {
    //        get=> (ICommand)GetValue(SubmenuOpenedCommandProperty);
    //        set=> SetValue(SubmenuOpenedCommandProperty, value);
    //    }
    //    public static readonly DependencyProperty SubmenuOpenedTargetProperty =
    //        DependencyProperty.Register(
    //            "SubmenuOpenedTarget",
    //            typeof(IInputElement),
    //            typeof(ExMenuItem));
    //    public IInputElement SubmenuOpenedTarget
    //    {
    //        get => (IInputElement)GetValue(SubmenuOpenedTargetProperty);
    //        set=> SetValue(SubmenuOpenedTargetProperty, value);
    //    }
    //    public static readonly DependencyProperty SubmenuOpenedParameterProperty =
    //        DependencyProperty.Register(
    //            "SubmenuOpenedParameter",
    //            typeof(object),
    //            typeof(ExMenuItem));
    //    public object SubmenuOpenedParameter
    //    {
    //        get => GetValue(SubmenuOpenedParameterProperty);
    //        set=> SetValue(SubmenuOpenedParameterProperty, value);
    //    }
    //    #endregion
    //    protected override void OnSubmenuOpened(RoutedEventArgs e)
    //    {
    //        base.OnSubmenuOpened(e);

    //        if (this.SubmenuOpenedCommand != null)
    //        {
    //            RoutedCommand? command = SubmenuOpenedCommand as RoutedCommand;

    //            if (command != null) command.Execute(SubmenuOpenedParameter, SubmenuOpenedTarget);                

    //            else SubmenuOpenedCommand.Execute(SubmenuOpenedParameter);                
    //        }
    //    }
    //}
    //public class MenuEX: Menu
    //{
    //    public MenuEX():base() { }
    //    protected override DependencyObject GetContainerForItemOverride()=> new ExMenuItem();
    //    protected override bool IsItemItsOwnContainerOverride(object item)=> item is ExMenuItem;
    //}

}
