using System.Windows;
using ListsTestTask.ViewModels;
using ListsTestTask.Views;

namespace ListsTestTask
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();

            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
