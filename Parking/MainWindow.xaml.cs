using System.Windows;
using Parking.ViewModels;

namespace Parking
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new CollectionsViewModel();
        }
    }
}