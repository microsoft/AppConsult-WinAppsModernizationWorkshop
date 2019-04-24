using System;

namespace ContosoExpenses
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var xamlApp = new Microsoft.Toolkit.Win32.UI.XamlHost.XamlApplication())
            {
                var appOwnedWindowsXamlManager = xamlApp.WindowsXamlManager;

                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
