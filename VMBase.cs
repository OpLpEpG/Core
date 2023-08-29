using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace Core
{
    public class VMBase: ObservableObject
    {        
        public static IServiceProvider ServiceProvider => (IServiceProvider) Application.Current;
        public string? ContentID { get; set; }
    }
}
