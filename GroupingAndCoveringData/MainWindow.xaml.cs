using System.Windows;
using Models.ViewModels;

namespace GroupingAndCoveringData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(
            MainViewModel mainModel )
        {
            InitializeComponent();
            DataContext = mainModel;
        }
    }
}