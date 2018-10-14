using System.Windows;
using EnglishWordsToPrint.ViewModels;

namespace EnglishWordsToPrint
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.OpenViberCsvFile();
        }

        private void ButtonBase_OnClick1(object sender, RoutedEventArgs e)
        {
            _vm.CreateExcelFile();

        }

    }
}
