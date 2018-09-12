using CPI.DTO;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace CPI.ViewModels
{
    public class AboutViewModel :ViewModelBase
    {
        #region Fields

        #endregion
        private string _description;
        private string _copyright;
        private string _license;
        private string _PCinfo;


        #region Properties

        #endregion
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Copyright
        {
            get { return _copyright; }
            set
            {
                _copyright = value;
                OnPropertyChanged("Copyright");
            }
        }

        public string PCinfo
        {
            get { return _PCinfo; }
            set
            {
                _PCinfo = value;
                OnPropertyChanged("PCinfo");
            }
        }

        public string License
        {
            get { return _license; }
            set
            {
                _license = value;
                OnPropertyChanged("License");
            }
        }

   

        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand UnloadedCommand { get; set; }
        public RelayCommand WebSiteCommand { get; set; }


        #region Constructor

        #endregion
        public AboutViewModel()
        {
            LoadedCommand = new RelayCommand(Loaded);
            UnloadedCommand = new RelayCommand(Unloaded);
            WebSiteCommand = new RelayCommand(OnWebSite);


        }


        #region Commands
        private void Unloaded()
        {
        }
        private void OnWebSite()
        {
            System.Diagnostics.Process.Start("http://www.phantom.co.il/");
        }
        private void Loaded()
        {
            var ls = TransferDB.Licenses.OrderByDescending(d => d.Count).FirstOrDefault();        

            if (ls != null)
            {
                PCinfo = ls.Summary;
                License = "Serial Number        \t" + ls.SerialNumber + "\n" +
                            "Authorization Key  \t" + ls.AuthorizationKey + "\n" +
                            "Registration Number\t" + ls.RegistrationNumber + "\n" +
                            "License Period     \t\t" + ls.Period + "\n" +
                            "Activated          \t\t" + ls.Start + "\n" +
                            "Expiration Date    \t\t" + ls.End;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;
            Description = (string)Application.Current.TryFindResource("description") + " " + version.ToString(3);
            Copyright = (string)Application.Current.TryFindResource("Copyright") + DateTime.Now.ToString("yyyy");

        }
        #endregion
    }
}
