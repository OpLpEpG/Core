# Core
Описывает глобальные переменные, классы, интерфейсы, стили, привязки
## VM
VMBase.cs - определяет класс от которого происходят все VM
```
    public class VMBase: ObservableObject
    {        
        public static IServiceProvider ServiceProvider => (IServiceProvider) Application.Current;
        public string? ContentID { get; set; }
    }
```
## Priority 
### PriorityItems.cs
 Главное окно приложения имеет меню и ToolBarы, 
 VM их элементов потомки PriorityItem,
```
    public abstract class PriorityItem: VMBase
    {
        public int Priority { get; set; } = 100;
        public int Group => Priority / 100;
    }
    public class Separator: PriorityItem { }

    public abstract class PriorityItemBase : PriorityItem 
    {
        #region Visibility
        #region IsEnable

        public ToolTip? ToolTip { get; set; }
        public bool IconSourceEnable => IconSource != null;
        public string? IconSource { get; set; }
    }
```
 Priority - сортировка положения в меню
 Group - разные группы выделяются сепараторами
 ### VMBaseMenus, VMBaseToolBar
 VMBaseMenus.cs, VMBaseToolBar.cs - реализации VM основных меню и кнопок 
 ViewResourceTools.xaml, ViewResourceMenus.xaml - ResourceDictionary сстветствующие стили
 и DataTemplate

 ### MenuServer ToolServer
 добавлениe удалениe MenuItem, ToolItem должно происходить через 
 серверы (для сортировки и создания сепараторов)
 ```
     public interface IMenuItemServer
    {
        public void Add(string ParentContentID, IEnumerable<MenuItemVM> Menus);
        public void Remove(string ContentID);
        public void Remove(IEnumerable<MenuItemVM> Menus);
        public bool Contains(string ContentID);
        public void UpdateSeparatorGroup(string ParentContentID);
        public void UpdateSeparatorGroup(MenuItemVM? ParentContentID);
    }

    public interface IToolServer
    {
        public void AddBar(ToolBarVM toolBar);
        public void DelBar(string BarContentID);
        public void Add(string BarContentID, IEnumerable<ToolButton> Tools);
        public void Remove(string ContentID);
        public void Remove(IEnumerable<ToolButton> Tools);
        public bool Contains(string ContentID);
        public bool Contains(ToolButton Tool);
        public void UpdateSeparatorGroup(string BarContentID);
    }
 ```
 доступ к ним через сервисы
 ```
     public static class ServicesRoot
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMenuItemServer, MenuServer>();
            services.AddSingleton<IToolServer, ToolServer>();
        }
    }
 ```
 ### View ViewResource.cs
 В основном окне меню и тулы обявлены так
 ```
    <Menu x:Name="menu" Style="{StaticResource ResourceKey={x:Static ToolBar.MenuStyleKey}}"
             ..........
             DataContext="{Binding MenuVM}"
             ItemsSource="{Binding RootItems}"   
             ...................
             ------------------------------------
             ItemContainerTemplateSelector="{StaticResource MenuTemplateSelectorVMKey}">
             --------------------------------------
       <Menu.Resources>
           <HierarchicalDataTemplate DataType="{x:Type c:MenuItemVM}"                                                      
                                     ItemsSource="{Binding Items}"/>
       </Menu.Resources>
   </Menu>

  <ToolBar Band="2" 
           .....
          ------------------------------------------------------------------
          ItemTemplateSelector="{StaticResource ToolTemplateSelectorVMKey}"
          ------------------------------------------------------------------
          DataContext="{Binding ToolGlyphVM}"
          ItemsSource="{Binding Items}">
  </ToolBar>
 ```
 ##### логическое древо привязка к модели
 меню:  
    DataContext = Binding MenuVM   
    ItemsSource = Binding RootItems      
 тулбар:  
    DataContext=Binding ToolGlyphVM  
    ItemsSource=Binding Items  

 ##### визуальное древо
 меню:  
    ItemContainerTemplateSelector = MenuTemplateSelector   
 тулбар:  
    ItemTemplateSelector = ToolTemplateSelector   
MenuTemplateSelector, ToolTemplateSelector объявлены в ***ViewResource.cs***
```
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

```
имя класса VM == ключ DataTemplate в словаре ресурсов (ViewResourceMenus,ViewResourceTools)   
***var name = item == null ? null : item.GetType().Name;***   
словари ресурсов могут объеденяться
```
  MenuTemplateSelector.Dictionary.MergedDictionaries.Add(new ResourceDictionary()
  {
      Source = new Uri("pack://application:,,,/Views/MenusResource.xaml")
  });

```
при создании уникальных VM меню, toolBar  и словарей стилей к ним 
