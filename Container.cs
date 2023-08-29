using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Xceed.Wpf.AvalonDock;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.AvalonDock.Layout;

namespace Core
{
    //public static class Locator 
    //{
    //    public static IServiceProvider Services => (Application.Current as IServiceProvider)!;
    //}
    public record StdLogg(bool Error, bool Info, bool Trace)
    {
        public StdLogg() : this(true, false, false) { }
    }
    public record StdLoggs(StdLogg Box, StdLogg File)
    {
        public StdLoggs() : this(new(), new()) { }
    }
    public record GlobalSettings(StdLoggs Logging, string Culture)
    {
        public GlobalSettings() : this(new(), "en-US") { }
    }
    public static class RootMenusID
    {
        public static string NFile => "NFile";
        public static string NFile_Create => "NFile_Create";
        public static string NFile_Open => "NFile_Open";
        public static string NFile_Add => "NFile_Add";
        public static string NFile_Close => "NFile_Close";
        public static string NFile_CloseProject => "NFile_CloseProject";
        public static string NFile_Save => "NFile_Save";
        public static string NFile_SaveProject => "NFile_saveProject";
        public static string NFile_SaveAll => "NFile_SaveAll";
        public static string NFile_Last_Projects => "NFile_Last_Projects";
        public static string NFile_Last_File => "NFile_Last_File";
        public static string NShow => "NShow";
        public static string NHidden => "NHidden";

    }
    /// <summary>
    /// Доступ к основному окну View (противоречет MVVM)
    /// </summary>
    //public interface IMainWindow
    //{
    //    //public Window Window { get; }
    //    //public Grid GridRoot { get; }
    //    //public ToolBarTray ToolBarTray { get; }
    //    //public ToolBar ToolBarMenu { get; }
    //    //public ToolBar ToolBarSpeedButtons { get; }
    //    //public ToolBar ToolBarButtons { get; }
    //    //public DockingManager DockManager { get; }
    //    //public LayoutDocumentPane DocumentPane { get; }
    //    //public StatusBar StatusBar { get; }
    //    //public object? FindMenu(string name);
    //    //public T AddToLayout<T>(
    //    //        bool CanClose =true,
    //    //        bool CanHide = false,
    //    //        bool CanAutoHide = true,
    //    //        bool CanFloat = true,
    //    //        bool CanDockAsTabbedDocument = true
    //    //        ) where T: class;
    //    //public void AddToLayout(object content);
    //    //public void AddToLayoutDocument(object content);
    //}  
    //public static class ServiceContainer 
    //{
    //    private readonly static List<Action<IServiceCollection>> _serv = new(); 
    //    public static void Add(Action<IServiceCollection> ServAct) => _serv.Add(ServAct);
    //    public static void Apply(IServiceCollection serviceProvider) => _serv.ForEach((a) => a(serviceProvider));
    //}
}
