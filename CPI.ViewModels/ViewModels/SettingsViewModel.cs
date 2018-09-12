using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using CPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EZLocalizeNS;
using Microsoft.Win32;
using CPI.Models.Entity;
using CPI.DTO;

namespace CPI.ViewModels
{
   public class SettingsViewModel: ViewModelBase
    {
        #region Fields

        private IDialogCoordinator dialogCoordinator;
        private ObservableCollection<Setting> _settings;
        private ObservableCollection<Models.Entity.License> _licenses;

        private ObservableCollection<Unit> _Units;
        private ObservableCollection<Computer> _Computers;

        private int _licensePeriod;
        private List<Language> _Languages;
        private Language _SelectedLanguage;


        private Visibility _SuperAdmin;
        private Models.Entity.License _SelectedLicense;
        private bool _IsReadOnly;


        #endregion

        #region Properties
        public ObservableCollection<Unit> Units
        {
            get { return _Units; }
            set
            {
                _Units = value;
                OnPropertyChanged("Units");
            }
        }

        public ObservableCollection<Computer> Computers
        {
            get { return _Computers; }
            set
            {
                _Computers = value;
                OnPropertyChanged("Computers");
            }
        }
        public Visibility SuperAdmin
        {
            get { return _SuperAdmin; }
            set
            {
                _SuperAdmin = value;
                OnPropertyChanged("SuperAdmin");
            }
        }

        public int LicensePeriod
        {
            get { return _licensePeriod; }
            set
            {
                _licensePeriod = value;
                OnPropertyChanged("LicensePeriod");
                GenerateCommand.RaiseCanExecuteChanged();
            }
        }
        public List<Language> Languages
        {
            get { return _Languages; }
            set
            {
                _Languages = value;
                OnPropertyChanged("Languages");
            }
        }
        public Language SelectedLanguage
        {
            get { return _SelectedLanguage; }
            set
            {
                if (value != _SelectedLanguage)
                {
                    _SelectedLanguage = value;
                    RegistryKey subKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\PH Technologies\PH_RDR\PreviousSessionParameters");
                    subKey.SetValue("Language", _SelectedLanguage.Culture);
                    ShowMessageAsync("INFO", "In order for the changes to take effect, please restart the program!", MessageDialogStyle.Affirmative);
                    OnPropertyChanged("SelectedLanguage");
                }
            }
        }

        public bool IsShowDrones
        {
            get { return _settings.FirstOrDefault(s => s.Key == "IsShowDetectDrones").NewValue == "True"; }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "IsShowDetectDrones").NewValue = value.ToString();
                OnPropertyChanged("IsShowDrones");
            }
        }
        public bool IsAlarm
        {
            get { return _settings.FirstOrDefault(s => s.Key == "IsAlarmWhenJammerTransmitting").NewValue == "True"; }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "IsShowDetectDrones").NewValue = value.ToString();
                OnPropertyChanged("IsAlarm");
            }
        }

        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set
            {
                _IsReadOnly = value;
                OnPropertyChanged("IsReadOnly");
            }
        }
        public bool EnableScanLow
        {
            get { return _settings.FirstOrDefault(s => s.Key == "IsScanningLow").NewValue == "True"; }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "IsScanningLow").NewValue = value.ToString();
                OnPropertyChanged("EnableScanLow");
            }
        }
        public bool EnableScanHigh
        {
            get { return _settings.FirstOrDefault(s => s.Key == "IsScanningHigh").NewValue == "True"; }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "IsScanningHigh").NewValue = value.ToString();
                OnPropertyChanged("EnableScanHigh");
            }
        }

        public int ScanPerCycle
        {
            get { return Int32.Parse(_settings.FirstOrDefault(s => s.Key == "ScanningLoops").NewValue); }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "ScanningLoops").NewValue = value.ToString();
                OnPropertyChanged("ScanPerCycle");
            }
        }

        public int Detections
        {
            get { return Int32.Parse(_settings.FirstOrDefault(s => s.Key == "ScanningDetections").NewValue); }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "ScanningDetections").NewValue = value.ToString();
                OnPropertyChanged("Detections");
            }
        }

        public int LowFilterSeq
        {
            get { return Int32.Parse(_settings.FirstOrDefault(s => s.Key == "FilterSequence_Low").NewValue); }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "FilterSequence_Low").NewValue = value.ToString();
                OnPropertyChanged("LowFilterSeq");
            }
        }

        public int HighFilterSeq
        {
            get { return Int32.Parse(_settings.FirstOrDefault(s => s.Key == "FilterSequence_High").NewValue); }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "FilterSequence_High").NewValue = value.ToString();
                OnPropertyChanged("HighFilterSeq");
            }
        }

        public string LowFilterType
        {
            get { return _settings.FirstOrDefault(s => s.Key == "FilterType_Low").NewValue; }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "FilterType_Low").NewValue = value.ToString();
                OnPropertyChanged("LowFilterType");
            }
        }

        public string HighFilterType
        {
            get { return _settings.FirstOrDefault(s => s.Key == "FilterType_High").NewValue; }
            set
            {
                _settings.FirstOrDefault(s => s.Key == "FilterType_High").NewValue = value.ToString();
                OnPropertyChanged("HighFilterType");
            }
        }

        public ObservableCollection<Setting> Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged("Settings");
            }
        }
       
        public ObservableCollection<Models.Entity.License> Licenses
        {
            get { return _licenses; }
            set
            {
                _licenses = value;
                OnPropertyChanged("Licenses");

            }
        }
      

        public Models.Entity.License SelectedLicense
        {
            get { return _SelectedLicense; }
            set
            {
                _SelectedLicense = value;
                OnPropertyChanged("SelectedLicense");
                DeleteLicense.RaiseCanExecuteChanged();
            }
        }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand UnloadedCommand { get; set; }
        public RelayCommand GenerateCommand { get; set; }
        public RelayCommand Save { get; set; }

        public RelayCommand DeleteLicense { get; set; }



        #endregion

        #region Constructor
        public SettingsViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            LoadedCommand = new RelayCommand(Loaded);
            UnloadedCommand = new RelayCommand(Unloaded);
            GenerateCommand = new RelayCommand(OnGenerateNewLicense, CanGenerate);
            Save = new RelayCommand(OnSave);
            DeleteLicense = new RelayCommand(OnDeleteLicense, CanDeleteLicense);

            Languages = new List<Language>
            {
                new Language { Name = "English", Culture = "en" },
                new Language { Name = "Русский", Culture = "ru" },
                new Language { Name = "Português", Culture = "pt" }
            };

            Settings = TransferDB.Settings;
            Licenses = TransferDB.Licenses;

            //_Units = TransferDB.Units;
            //_Computers = TransferDB.Computers;///TODO maybe add more



            if (Transfer.UserPermission != null)
            {
                if (Transfer.UserPermission == "SuperAdmin")
                {
                    IsReadOnly = false;
                    SuperAdmin = Visibility.Visible;
                }
                else
                {
                    IsReadOnly = true;
                    SuperAdmin = Visibility.Collapsed;
                }
            }
        }






        #endregion

        #region Commands Methods
        private async void OnSave()
        {
            var res = await ShowMessageAsync("Save?", "Are you sure? Save Updated values to the DataBase", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });
            if (res == MessageDialogResult.Affirmative)
                DataDB.UpdateAllStuff();

        }
        private bool CanGenerate()
        {
            return _licensePeriod != 0;
        }
        private async void OnGenerateNewLicense()
        {
            var res = await ShowMessageAsync("Generate?", "Are you sure? Generate new license?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });
            if (res == MessageDialogResult.Affirmative)
            {
                Licenses.Add(GenerateNewLicense(LicensePeriod));
            }
        }
        private void Unloaded()
        {


        }
        private void Loaded()
        {
            if (Settings != null)
            {
                SettingParser();
            }
        }

        private async void OnDeleteLicense()
        {
            var res = await ShowMessageAsync("Delete?", "Are you sure to delete this License?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });
            if (res == MessageDialogResult.Affirmative)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (sender, e) =>
                {
                    e.Result = DatabaseService.Delete(e.Argument as Models.Entity.License);
                };
                worker.RunWorkerCompleted += (sender, e) =>
                {
                    int dd = (int)e.Result;
                    Licenses.Remove(SelectedLicense);
                };
                worker.RunWorkerAsync(SelectedLicense);
            }
        }

        private bool CanDeleteLicense()
        {
            return SelectedLicense != null;
        }
        #endregion

        #region General Methods

        private void SettingParser()
        {
            Language language;
            RegistryKey subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\PH Technologies\PH_RDR\PreviousSessionParameters");
            if (subKey == null)
            {
                subKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\PH Technologies\PH_RDR\PreviousSessionParameters");
                language = _Languages.FirstOrDefault(l => l.Culture == Settings.FirstOrDefault(s => s.Key == "Language").NewValue);
                subKey.SetValue("Language", language.Culture);
            }
            else
                language = _Languages.FirstOrDefault(l => l.Culture == subKey.GetValue("Language", "en").ToString());

            _SelectedLanguage = language;
            OnPropertyChanged("SelectedLanguage");
        }
        private Models.Entity.License GenerateNewLicense(int licensePeriod)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];

            string appGuid = attribute.Value;
            string serialNumber = FingerPrint.GetMAC().Replace(':', '-');
            string summary = FingerPrint.GetSummary() + "\nMAC\t" + serialNumber + "\nGUID\t" + appGuid;
            int numberOfLicenses = Licenses.Count;
            string authorizationKey = FingerPrint.GetHash(appGuid + numberOfLicenses.ToString());
            string registrationNumber = FingerPrint.GetHash(serialNumber + authorizationKey + licensePeriod.ToString());


            Models.Entity.License NewLicense = new Models.Entity.License()
            {
                ID = Guid.NewGuid(),
                Count = ++numberOfLicenses,
                SerialNumber = serialNumber,
                AuthorizationKey = authorizationKey,
                RegistrationNumber = registrationNumber,
                Period = licensePeriod,
                Start = DateTime.Today,
                End = DateTime.Today.Add(new TimeSpan(licensePeriod, 0, 0, 0)),
                LastSession = DateTime.Now,
                Summary = summary

            };

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result = DatabaseService.AddOrUpdate(e.Argument as Models.Entity.License);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                int dd = (int)e.Result;
                DataDB.RefreshLicenses();
            };
            worker.RunWorkerAsync(NewLicense);


            string LicenseDir = @"\PHRDR\License";
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(documentPath + LicenseDir))
                Directory.CreateDirectory(documentPath + LicenseDir);

            string licenseFile = String.Format(@"{0}\License.lph", documentPath + LicenseDir);

            File.Create(licenseFile).Close();
            string toFile = "Serial Number:\n\t" + serialNumber + "\n" +
                            "Authorization Key:\n\t" + authorizationKey + "\n" +
                            "Registration Number:\n\t" + registrationNumber + "\n" +
                            "Period:\n\t" + licensePeriod + "\n" +
                            "Start:\n\t" + DateTime.Today + "\n" +
                            "Last Session:\n\t" + DateTime.Now + "\n" +
                            "End:\n\t" + DateTime.Today.Add(new TimeSpan(licensePeriod, 0, 0, 0)) + "\n\n" +
                            "Summary PC:\n" + summary;
            File.WriteAllText(licenseFile, toFile);
            return NewLicense;
        }
        private async void ShowMessageAsync(string header, string message, MessageDialogStyle dialogStyle)
        {
            await dialogCoordinator.ShowMessageAsync(this, header, message, dialogStyle, new MetroDialogSettings
            {
                AnimateShow = false
            });
        }
        public async static Task<MessageDialogResult> ShowMessageAsync(string title, string Message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync(title, Message, style, settings);
        }

        #endregion
    }
}
