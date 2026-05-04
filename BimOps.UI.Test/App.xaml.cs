using System.Windows;
using AddinMainWindow = BimOps.UI.MainWindow;

namespace BimOps.UI.Test
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = new AddinMainWindow();
            window.Show();
        }
    }
}