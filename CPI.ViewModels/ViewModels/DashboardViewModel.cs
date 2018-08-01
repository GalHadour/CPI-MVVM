using CPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CPI.ViewModels
{
    public class DashboardViewModel:ViewModelBase
    {
        private Visibility _UnitDetailsVisibility = Visibility.Collapsed;

        public Visibility UnitDetailsVisibility
        {
            get { return _UnitDetailsVisibility; }
            set
            {
                _UnitDetailsVisibility = value;
                OnPropertyChanged("UnitDetailsVisibility");
            }
        }

        public RelayCommand ShowSMS { get; private set; }
        public RelayCommand ShowCalls { get; private set; }
        public RelayCommand ShowReceivers { get; private set; }
        public RelayCommand ShowRecords { get; private set; }
        public RelayCommand ShowUnitDetails { get; private set; }
        public RelayCommand CloseUnitDetails { get; private set; }

        public DashboardViewModel()
        {
            ShowSMS = new RelayCommand(OnShowSMS);
            ShowCalls = new RelayCommand(OnShowCalls);
            ShowReceivers = new RelayCommand(OnShowReceivers);
            ShowRecords = new RelayCommand(OnShowRecords);
            ShowUnitDetails = new RelayCommand(OnShowUnitDetails);
            CloseUnitDetails = new RelayCommand(OnCloseUnitDetails);
        }

        private void OnCloseUnitDetails()
        {
            UnitDetailsVisibility = Visibility.Collapsed;
        }

        private void OnShowUnitDetails()
        {
            UnitDetailsVisibility = Visibility.Visible;
        }

        private void OnShowRecords()
        {
            Transfer.HamburgerMenuControl.SelectedIndex = 3;
        }

        private void OnShowReceivers()
        {
            Transfer.HamburgerMenuControl.SelectedIndex = 1;
        }

        private void OnShowSMS()
        {
            Transfer.HamburgerMenuControl.SelectedIndex = 5;
        }
        private void OnShowCalls()
        {
            Transfer.HamburgerMenuControl.SelectedIndex = 4;
        }
    }



}
