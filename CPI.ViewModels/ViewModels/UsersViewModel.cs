using CPI.DTO;
using CPI.Models.Entity;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CPI.ViewModels
{
    public class UsersViewModel :ViewModelBase
    {

        #region Fields
        private bool isLoaded;
        private ObservableCollection<User> _users;
        private string _surname;
        private string _name;
        private string _permission;
        private string _password;
        private string _username;
        private IDialogCoordinator dialogCoordinator;
        private User _selectedUser;

        #endregion

        #region Properties
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand UnloadedCommand { get; set; }
        public ObservableCollection<User> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                OnPropertyChanged("Users");
            }
        }
        public User SelectedUser
        {
            get
            {
                return _selectedUser;
            }

            set
            {
                _selectedUser = value;
                OnPropertyChanged("SelectedUser");
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                _username = value;
                OnPropertyChanged("Username");
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
                OnPropertyChanged("Password");
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Permission
        {
            get
            {
                return _permission;
            }

            set
            {
                _permission = value;
                OnPropertyChanged("Permission");
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                OnPropertyChanged("Name");
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Surname
        {
            get
            {
                return _surname;
            }

            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Constructor
        public UsersViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;
            SaveCommand = new RelayCommand(OnSaveNewUser, CanSave);
            ClearCommand = new RelayCommand(OnClear);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            LoadedCommand = new RelayCommand(Loaded);
            UnloadedCommand = new RelayCommand(Unloaded);
        }

        #endregion

        #region Methods



        private async void ShowMessageAsync(string header, string message, MessageDialogStyle dialogStyle)
        {
            var settings = new MetroDialogSettings
            {
                AnimateShow = false
            };
            await dialogCoordinator.ShowMessageAsync(this, header, message, dialogStyle, settings);
        }
        private string CombinePassword(string password)
        {
            //Create the salt value with a cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Combine the salt and password bytes
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public async static Task<MessageDialogResult> ShowMessageAsync(string title, string Message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync(title, Message, style, settings);
        }

        #endregion

        #region Commands
        private void OnClear()
        {
            Surname = Username = Name = Password = Permission = null;
        }

        private bool CanSave()
        {
            return (Username != null && Surname != null && Name != null && Permission != null);
        }

        private bool CanDelete()
        {
            if (SelectedUser != null)
            {
                if (SelectedUser.Permission == "Admin" && Users.Count(u => u.Permission == "Admin") == 1)
                    return false;
                else
                    return true;
            }
            else
                return false;

        }

        private void OnSaveNewUser(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox.Password;
            if (!String.IsNullOrEmpty(password) && !String.IsNullOrWhiteSpace(password))
            {
                if (Users.FirstOrDefault(n => n.UserName == Username) == null)
                {
                    User newUser = new User()
                    {
                        ID = Guid.NewGuid(),
                        UserName = Username,
                        Password = CombinePassword(password),
                        Name = Name,
                        Surname = Surname,
                        Permission = Permission
                    };

                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (sender, e) =>
                    {
                        e.Result = DatabaseService.AddOrUpdate(e.Argument as User);
                    };
                    worker.RunWorkerCompleted += (sender, e) =>
                    {
                        int dd = (int)e.Result;
                        Users.Add(newUser);
                    };
                    worker.RunWorkerAsync(newUser);
                }
                else
                    ShowMessageAsync((string)Application.Current.TryFindResource("ERROR"), (string)Application.Current.TryFindResource("UsernameExists"), MessageDialogStyle.Affirmative);
            }
            else
                ShowMessageAsync((string)Application.Current.TryFindResource("ERROR"), (string)Application.Current.TryFindResource("AllFields"), MessageDialogStyle.Affirmative);
        }

        private void Unloaded()
        {

        }

        private void Loaded()
        {
            if (!isLoaded)
            {
                var usrs = TransferDB.Users;

                _users = new ObservableCollection<User>();
                foreach (var item in usrs.Where(u => u.Permission != "su"))
                {
                    _users.Add(item);
                }

                OnPropertyChanged("Users");
                isLoaded = true;
            }
        }

        private async void OnDelete()
        {
            var res = await ShowMessageAsync("Delete?", "Are you sure? Do you want delete user " + SelectedUser.UserName + "?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });
            if (res == MessageDialogResult.Affirmative)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                {
                    e.Result = DatabaseService.Delete(e.Argument as User);
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    int dd = (int)e.Result;
                    Users.Remove(SelectedUser);
                };
                worker.RunWorkerAsync(SelectedUser);
            }
        }
        #endregion

    }
}
