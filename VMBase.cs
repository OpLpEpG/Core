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
        //TODO: set -должен быть с PropertyChanged или виртуальный анализировать AnyData.AnyData
        /// <param name="RootContentID"> RootContentID.AnyData.AnyData...) </param>
        // при загрузки DockManager  ContentID присваивается генерирующейся VM viewмодели AddOrGet(string ContentID);
        // например для монитора СОМ если нет модели MM (COM) то закрыть (удалить) окно
        public string? ContentID { get; set; }
    }
}
