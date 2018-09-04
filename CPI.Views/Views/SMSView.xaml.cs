using CPI.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for SMSView.xaml
    /// </summary>
    public partial class SMSView : UserControl
    {
        public SMSView()
        {
            InitializeComponent();
            this.DataContext = new SMSViewModel();
        }
    }
}
