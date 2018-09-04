using System.Windows.Controls;
using System.Windows;
using Controls.ReceiverUnit;
using CPI.ViewModels;

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {

        public DashboardView()
        {
            InitializeComponent();
      
            this.DataContext = new DashboardViewModel();


        }
    }
}
