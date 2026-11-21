using System.Windows;
using Aureo.ViewModels;

namespace Aureo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // Builds the XAML visual tree.
            DataContext = new MainViewModel(); // Binds expressions to this ViewModel.
        }
    }
}