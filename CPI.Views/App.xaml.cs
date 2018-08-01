using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using EZLocalizeNS;

namespace CPI.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static EZLocalize AppLocalize = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            ViewModels.DataDB.GetDataDB();

            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));

            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.PreviewMouseDownEvent,
                new RoutedEventHandler(TextBox_PreviewMouseDown));

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            AppLocalize = new EZLocalize(App.Current.Resources, "en", null, "Languages\\", "Interface");



            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));



            base.OnStartup(e);

        }
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhanded exception occurred: {0}", e.Exception.InnerException.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!textBox.IsFocused)
            {
                textBox.Focus();
                textBox.SelectAll();
                e.Handled = true;
            }
        }
    }
}
