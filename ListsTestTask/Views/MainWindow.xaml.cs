using ListsTestTask.ViewModels;
using System.Windows;

namespace ListsTestTask.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
