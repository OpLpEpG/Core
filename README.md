# Core
��������� ���������� ����������, ������, ����������, �����, ��������
## VM
VMBase.cs - ���������� ����� �� �������� ���������� ��� VM
```
    public class VMBase: ObservableObject
    {        
        public static IServiceProvider ServiceProvider => (IServiceProvider) Application.Current;
        public string? ContentID { get; set; }
    }
```
## Priority 
### PriorityItems.cs
 ������� ���� ���������� ����� ���� � ToolBar�, 
 VM �� ��������� ������� PriorityItem,
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
 Priority - ���������� ��������� � ����
 Group - ������ ������ ���������� ������������
 ### VMBaseMenus, VMBaseToolBar
 VMBaseMenus.cs, VMBaseToolBar.cs - ���������� VM �������� ���� � ������ 
 ViewResourceTools.xaml, ViewResourceMenus.xaml - ResourceDictionary �������������� �����
 � DataTemplate

 ### MenuServer ToolServer
 ���������e �������e MenuItem, ToolItem ������ ����������� ����� 
 ������� (��� ���������� � �������� �����������)
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
 ������ � ��� ����� �������
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
 � �������� ���� ���� � ���� �������� ���
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
 ##### ���������� ����� �������� � ������
 ����:
    DataContext = Binding MenuVM 
    ItemsSource = Binding RootItems    
 ������:
    DataContext=Binding ToolGlyphVM
    ItemsSource=Binding Items

 ##### ���������� �����
 ����:
    ItemContainerTemplateSelector = MenuTemplateSelector
 ������:
    ItemTemplateSelector = ToolTemplateSelector
MenuTemplateSelector, ToolTemplateSelector ��������� � ***ViewResource.cs***
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
��� ������ VM == ���� DataTemplate � ������� �������� (ViewResourceMenus,ViewResourceTools)
***var name = item == null ? null : item.GetType().Name;***
������� �������� ����� ������������
```
  MenuTemplateSelector.Dictionary.MergedDictionaries.Add(new ResourceDictionary()
  {
      Source = new Uri("pack://application:,,,/Views/MenusResource.xaml")
  });

```
��� �������� ���������� VM ����� ���� � toolBar
