using CPI.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for ScannerView.xaml
    /// </summary>
    public partial class ScannerView : UserControl
    {

       private ScannerViewModel ViewModel = new ScannerViewModel(DialogCoordinator.Instance);
        public ScannerView()
        {
            InitializeComponent();
            DataContext = ViewModel;
          
            ViewModel.Initialized();
        }

    
    }
}
