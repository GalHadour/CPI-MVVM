using CPI.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for RecordsView.xaml
    /// </summary>
    public partial class RecordsView : UserControl
    {
        RecordsViewModel ViewModel = new RecordsViewModel(DialogCoordinator.Instance);
        public RecordsView()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.Initialized();
        }
    }
}
