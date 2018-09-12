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

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for SettingItem.xaml
    /// </summary>
    public partial class SettingItem : UserControl
    {

        public static readonly DependencyProperty ValueIdentity = DependencyProperty.Register("Identity", typeof(string), typeof(SettingItem), null);
        public string Identity
        {
            get { return (string)GetValue(ValueIdentity); }
            set { SetValueDP(ValueIdentity, value); }
        }

        public static readonly DependencyProperty ValueMainContent = DependencyProperty.Register("MainContent", typeof(object), typeof(SettingItem), null);
        public object MainContent
        {
            get { return (object)GetValue(ValueMainContent); }
            set { SetValueDP(ValueMainContent, value); }
        }


        public SettingItem()
        {
            InitializeComponent();
#if (DEBUG)

#else
            MainBorder.Visibility = Visibility.Collapsed;
            MainContentControl.Visibility = Visibility.Collapsed;
#endif

        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (e.ChangedButton == MouseButton.Left)
                if (e.ClickCount == 2 && MainContentControl.Visibility == Visibility.Collapsed)
                {
                    MainBorder.Visibility = Visibility.Visible;
                    MainContentControl.Visibility = Visibility.Visible;
                }
                else if (e.ClickCount == 2 && MainContentControl.Visibility == Visibility.Visible)
                {
                    MainBorder.Visibility = Visibility.Collapsed;
                    MainContentControl.Visibility = Visibility.Collapsed;
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void SetValueDP(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {

            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }
}
