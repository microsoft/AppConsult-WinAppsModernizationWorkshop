using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


[assembly: System.Windows.Media.DisableDpiAwareness]

namespace ContosoExpenses
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHostBase).TypeHandle);

        }
    }
}
