using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.HexViewer.ViewModels;

namespace ZoDream.HexViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static readonly MainViewModel ViewModel = new();
    }
}
