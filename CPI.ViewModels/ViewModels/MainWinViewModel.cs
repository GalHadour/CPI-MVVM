﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPI.Models;
using EZLocalizeNS;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace CPI.ViewModels
{
    public class MainWinViewModel : ViewModelBase
    {
        #region Fields

        private object _selectedViewModel;
        private Models.Entity.License lastLicense;
        private IDialogCoordinator dialogCoordinator;
        private BackgroundWorker worker;
        private static string _userPermissionLevel;

        private RegistrationViewModel LicenseVM;
        private MainViewModel MainVM;
        private string _myTitle;
        private EZLocalize _Localize;

        #endregion

        #region Properties

        public string MyTitle
        {
            get { return _myTitle; }

            set
            {
                _myTitle = value;
                OnPropertyChanged("MyTitle");
            }
        }

        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand UnloadedCommand { get; set; }

        public object SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged("SelectedViewModel");
            }
        }

        public static string UserPermissionLevel
        {
            get { return _userPermissionLevel; }
            set
            {
                _userPermissionLevel = value;
            }
        }

        /// <summary>
        /// Switch the view from another xaml view (check the TEST button on login view)
        /// </summary>
        public RelayCommand SwitchView
        {
            get { return new RelayCommand(SwitchToMainView); }
        }

        public EZLocalize Localize
        {
            get { return _Localize; }
            set
            {
                _Localize = value;
                OnPropertyChanged("Localize");
            }
        }

        #endregion

        #region Constructor
        public MainWinViewModel(IDialogCoordinator instance)
        {

            dialogCoordinator = instance;
            LoadedCommand = new RelayCommand(Loaded);
            UnloadedCommand = new RelayCommand(Unloaded);
            GetLastLicense();
            LoginViewModel loginViewModel = new LoginViewModel(DialogCoordinator.Instance);
            loginViewModel.OnLoginSuccessful += CheckLicense;
            SelectedViewModel = loginViewModel;

            //SelectedViewModel = new MainViewModel();
        }

        public void Initialized()
        {

        }
        #endregion

        #region Methods

        private void LoadPreviousSessionParameters()
        {
            RegistryKey subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\PH Technologies\PH_RDR\PreviousSessionParameters");
            if (subKey != null)
            {
                Localize.ChangeLanguage(subKey.GetValue("Language").ToString());
            }

        }

        private void Unloaded()
        {
        }

        private void Loaded()
        {
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();

            LoadPreviousSessionParameters();

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<object> ff = (List<object>)e.Result;
            LicenseVM = (RegistrationViewModel)ff[0];
            MainVM = (MainViewModel)ff[1];
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = new List<object>
            {
                new RegistrationViewModel(DialogCoordinator.Instance),
                new MainViewModel()
            };
        }

        private void CheckLicense(string userPermission)
        {
            UserPermissionLevel = userPermission;
            if (userPermission == "SuperAdmin")
                SwitchToMainView();
            else
            {
                if (lastLicense != null && lastLicense.SerialNumber == FingerPrint.GetMAC().Replace(':', '-'))
                {
                    if (lastLicense.Period == 3650 || (DateTime.Today >= lastLicense.Start && DateTime.Today <= lastLicense.End && lastLicense.LastSession <= DateTime.Now))
                    {
                        int days = (lastLicense.End.Value - DateTime.Today).Days;
                        if (days <= 14)
                            ShowMessageAsync("License Warning", "Your license will expire in " + days + " days.\nPlease contact your distribution maintainer for update!", MessageDialogStyle.Affirmative);
                        lastLicense.LastSession = DateTime.Now;
                        SwitchToMainView();
                    }
                    else
                    {
                        ShowMessageAsync("License Error", "Application can not be loaded, unauthorized date interrupt occurred (" + lastLicense.Period + " days)! " +
                            "\nPlease contact your distribution maintainer for update the license!", MessageDialogStyle.Affirmative);
                        RunRegistrationProcess(lastLicense.Count);
                    }
                }
                else if (lastLicense != null && lastLicense.SerialNumber != FingerPrint.GetMAC().Replace(':', '-'))
                {
                    ShowMessageAsync("License Error", "The serial number in the license file does not match your computer’s active serial number" +
                            "\nPlease contact your distribution maintainer for update the license!", MessageDialogStyle.Affirmative);
                    RunRegistrationProcess(lastLicense.Count);
                }
                else
                    RunRegistrationProcess(0);
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

        private void RunRegistrationProcess(int licenseCount)
        {

            if (LicenseVM != null)
            {
                LicenseVM.LastLicenseCount = licenseCount;
                LicenseVM.OnSwitchView += SwitchToMainView;
                SelectedViewModel = LicenseVM;
            }
            else
            {
                RegistrationViewModel licenseViewModel = new RegistrationViewModel(DialogCoordinator.Instance);
                licenseViewModel.OnSwitchView += SwitchToMainView;
                SelectedViewModel = licenseViewModel;
            }
        }

        private void SwitchToMainView()
        {
            if (MainVM != null)
                SelectedViewModel = MainVM;
            else
                SelectedViewModel = new MainViewModel();
        }

        private void GetLastLicense()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result = DatabaseService.GetLastLicense();
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                lastLicense = (Models.Entity.License)e.Result;
            };
            worker.RunWorkerAsync();
        }
        #endregion


    }
}
