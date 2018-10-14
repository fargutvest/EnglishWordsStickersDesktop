using System.Windows;
using EnglishWordsPrintUtility.ViewModels;

namespace EnglishWordsPrintUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM _vm => DataContext as MainWindowVM;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.OpenViberCsvFile();
        }

        private void CreateButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.CreateExcelFile();
        }

    }
}
