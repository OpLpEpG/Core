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
        public static string ROOT => "ROOT";
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
        public static string NDebugs => "NDebugs";
        public static string NHidden => "NHidden";

    }
}
