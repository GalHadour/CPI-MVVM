using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using CPI.ViewModels;

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        MainWinViewModel viewModel = new MainWinViewModel(DialogCoordinator.Instance);
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.Localize = App.AppLocalize;
            viewModel.Initialized();
        }
    }
}
