using System.Windows;

namespace AimTrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _vm = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
            _vm.NextGrid.Execute(null);
        }
    }
}
