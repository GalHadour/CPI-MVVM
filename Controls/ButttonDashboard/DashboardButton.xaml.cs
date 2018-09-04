using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Controls.ButttonDashboard
{

    /// <summary>
    /// Interaction logic for DashboardButton.xaml
    /// </summary>
    public partial class DashboardButton : UserControl
    {

        private  int _ItemsNumber = 0;
        SolidColorBrush _ColorButton=new SolidColorBrush();
        private string _ButtonName;
        PackIconMaterialKind _IconKind;
        public DashboardButton()
        {

            InitializeComponent();

        }

        public int ItemsNumber
        {
            get { return _ItemsNumber; }
            set
            {
                _ItemsNumber = value;
                OnPropertyChanged("ItemsNumber");
            }
        }

        public PackIconMaterialKind IconKind
        {
            get {  return _IconKind; }
            set
            {
                _IconKind = value;
                OnPropertyChanged("IconKind");
            }
        }

        
   public string ButtonName
        {
            get { return _ButtonName; }
            set
            {
                _ButtonName = value;
                OnPropertyChanged("ButtonName");
            }
        }

        public SolidColorBrush ColorButton
        {
            get
            {
                return _ColorButton;
            }
            set
            {
                _ColorButton = value;
                OnPropertyChanged("ColorButton");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
