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

## Окна (Forms)
 управление AvalonDock 2.0
### VM
 VMBaseForms.cs определяет основные типы VM окон
 ```
 public abstract class VMBaseForms : VMBase
 public class DocumentVM : VMBaseForms
 public class ToolVM: VMBaseForms
 ```
 ViewResource.xaml определяет привязку VM стили Style и заглушки DataTemplate содержимого окон 
 ```
     <Style x:Key="LayoutItemStyle" TargetType="{x:Type adctrl:LayoutItem}">
        <Setter Property="Title" Value="{Binding Model.Title}"/>
        <Setter Property="CloseCommand" Value="{Binding Model.CloseCommand}"/>
        <Setter Property="ContentId" Value="{Binding Model.ContentID}"/>
        ...........................................
    </Style>

    <Style x:Key="ToolVMStyle" 
           TargetType="{x:Type adctrl:LayoutAnchorableItem}" 
           BasedOn="{StaticResource LayoutItemStyle}">
        ..............................................
    </Style>

    <Style x:Key="DocumentVMStyle" 
           TargetType="{x:Type adctrl:LayoutDocumentItem}" 
           BasedOn="{StaticResource LayoutItemStyle}">
           ...........................................................
    </Style>
    
    <DataTemplate x:Key="ToolVMTemplate">
    ................................................
    </DataTemplate>


    <DataTemplate x:Key="DocumentVMTemplate">
    ...........................................
    </DataTemplate>
 ```

 ### определение  AvalonDock DockingManager в основном окне 
 ```
         <a:DockingManager Name="dockManager" 
                          DataContext="{StaticResource DockManagerVMKey}"
                          AnchorablesSource="{Binding Tools}"
                          DocumentsSource="{Binding Docs}"
                          DocumentHeaderTemplate="{StaticResource LayoutDocumentHeaderKey}"
                          DocumentTitleTemplate="{StaticResource LayoutDocumentHeaderKey}"
                          LayoutUpdateStrategy="{StaticResource LayoutInitializerKey}"
                          LayoutItemTemplateSelector="{StaticResource PanesTemplateSelectorKey}"
                          LayoutItemContainerStyleSelector="{StaticResource PanesStyleSelectorKey}"
                          .................................
 ```
 VM окон DockManagerVM  
 Tools - присоединяемые системные окна   
 Docs - присоединяемые окна документов   

 LayoutItemTemplateSelector = PanesTemplateSelector  
 LayoutItemContainerStyleSelector = PanesStyleSelector  

 PanesTemplateSelector, PanesStyleSelector объавлены в ViewResource.cs идея аналогичная для меню и тулбар
 ```
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
        ...............................
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
 ```
только к имени VM добавляюмся в словаре "Style" или "Template"

реальные FormsResource.xaml
```
    <Style x:Key="ProjectsExplorerVMStyle" 
           TargetType="{x:Type adctrl:LayoutAnchorableItem}" 
           BasedOn="{StaticResource LayoutAnchorableItemStyle}"/>

    <DataTemplate x:Key="ProjectsExplorerVMTemplate">
        <v:ProjectsExplorerUC/>
    </DataTemplate>

    <DataTemplate x:Key="TextLogVMTemplate">
        <v:TextLogUC/>
    </DataTemplate>
```

словарь ресурсов может объедениться
```
   FormResource.Dictionary.MergedDictionaries.Add(new ResourceDictionary()
   {
       Source = new Uri("pack://application:,,,/Views/FormsResource.xaml")
   });
```
