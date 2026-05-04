using System.Windows;
using AddinMainWindow = BimOps.UI.MainWindow;

namespace BimOps.UI.Test
{
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            var window = new AddinMainWindow();
            window.Show();
        }
    }
}