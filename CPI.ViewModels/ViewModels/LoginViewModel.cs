using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using CPI.Models.Entity;
using CPI.DTO;
using System.Windows.Controls;

namespace CPI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

        #region Properties
        private BackgroundWorker worker;
        private List<User> userList;
        private IDialogCoordinator dialogCoordinator;
        private Visibility _isVisibile;
        private string _description;
        private string _copyright;
        private string _username;
        private bool _isActive;


        public Visibility IsVisibile
        {
            get { return _isVisibile; }
            set
            {
                _isVisibile = value;
                OnPropertyChanged("IsVisibile");
            }
        }



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

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public delegate void LoginSuccessful(string parameter);
        public LoginSuccessful OnLoginSuccessful { get; set; }


        #endregion

        #region Constructor
        public LoginViewModel(IDialogCoordinator instance)
        {
            RefreshUsers();
            dialogCoordinator = instance;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;

            IsVisibile = Visibility.Visible;
            Description = (string)Application.Current.TryFindResource("DESCRIPTION") + " " + version.ToString(3);
            Copyright = (string)Application.Current.TryFindResource("Copyright") + DateTime.Now.ToString("yyyy");

            LoginCommand = new RelayCommand(OnLogin, CanLogin);
            CancelCommand = new RelayCommand(OnCancel);

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

        }


        #endregion

        #region Methods

        private void RefreshUsers()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result = DatabaseService.GetAllUsers();
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                userList = e.Result as List<User>;
            };
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<object> genericlist = e.Argument as List<object>;
            string username = (string)genericlist[0];
            string password = (string)genericlist[1];

            e.Result = Login(username, password);
        }

        private User Login(string username, string password)
        {

            while (userList == null)
            {
                Thread.Sleep(200);
            };

            try
            {
                User user = userList.FirstOrDefault(u => u.UserName == username);
                if (user != null && VerifyUserEnteredPassword(user.Password, password))
                    return user;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool VerifyUserEnteredPassword(string savedPasswordHash, string enteredPassword)
        {
            // Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            // Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            // Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            // Compare the results
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null) { }
                else if (e.Cancelled) { }
                else { }

                User us = (User)e.Result;
                if (us != null)
                {
                    if (OnLoginSuccessful != null)
                    {
                        OnLoginSuccessful(us.Permission);
                        Transfer.UserPermission = us.Permission;
                    }


                }
                else
                    ShowMessageAsync("ERROR", (string)Application.Current.TryFindResource("loginError"), MessageDialogStyle.Affirmative);

                IsActive = false;
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                ShowMessageAsync("Exception", ex.Message, MessageDialogStyle.Affirmative);
#else
                ShowMessageAsync("ERROR", "An unexpected error occurred while processing the request. \nPlease try again latter!", MessageDialogStyle.Affirmative);
#endif
            }
        }

        private async void ShowMessageAsync(string header, string message, MessageDialogStyle dialogStyle)
        {
            var settings = new MetroDialogSettings
            {
                AnimateShow = false
            };
            await dialogCoordinator.ShowMessageAsync(this, header, message, dialogStyle, settings);
        }

        #endregion

        #region Commands Methods

        private void OnCancel()
        {
            Application.Current.Shutdown();
        }

        private void OnLogin(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            if (passwordBox != null)
            {
                var password = passwordBox.Password;
                InitializeLogin(Username, password);
            }
        }

        public void InitializeLogin(string username, string password)
        {
            if (!String.IsNullOrEmpty(password) && !String.IsNullOrWhiteSpace(password) && !String.IsNullOrEmpty(username) && !String.IsNullOrWhiteSpace(username))
            {

                List<object> arguments = new List<object>
                {
                    username,
                    password
                };
                IsActive = true;
                worker.RunWorkerAsync(arguments);
            }
            else
            {
                ShowMessageAsync("ERROR", (string)Application.Current.TryFindResource("loginError"), MessageDialogStyle.Affirmative);
            }
        }

        private bool CanLogin()
        {
            return Username != null;
        }
        #endregion

    }
}