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

namespace Controls.RecordsView
{
    /// <summary>
    /// Interaction logic for RecordViewTable.xaml
    /// </summary>
    public partial class RecordsViewTable : UserControl
    {
       // private string _RecordsViewName;

        public RecordsViewTable()
        {
            InitializeComponent();
        }

        public string RecordsViewName  {get;set;}
    }
}
