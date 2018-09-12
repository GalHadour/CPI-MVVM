

using CPI.DTO;
using CPI.Models.Entity;
using CPI.Models.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Controls.Devieces;

namespace Controls.ReceiverUnit
{
    /// <summary>
    /// Interaction logic for ReceiverUI.xaml
    /// </summary>
    public partial class ReceiverUI : UserControl
    {

        #region Fields
        private bool _Loop = false;
        private bool _AutoRunInter = false;
        private bool _DelCfile = false;
        private bool _StopScanVisi = false;

        private Visibility _StopScanVisibility = Visibility.Collapsed;
        private Visibility _StopReceiverVisibility = Visibility.Collapsed;
        private Visibility _StartScanVisibility = Visibility.Visible;
        private Visibility _StartReceiverVisibility = Visibility.Visible;
        #endregion

        #region Properties
        public int Gain { get; set; }
        public int NOR { get; set; }
        public int Length { get; set; }
        public RelayCommandControls StartReceiver { get; set; }
        public RelayCommandControls StopReceiver { get; set; }
        public RelayCommandControls StartScan { get; set; }
        public RelayCommandControls StopScan { get; set; }
        public bool StopScanVisi
        {
            get { return _StopScanVisi; }
            set
            {
                _StopScanVisi = value;
                OnPropertyChanged("StopSacnVisi");
            }
        }
   


        public int UnitNumber { get; set; }
        public bool Loop
        {
            get { return _Loop; }
            set
            {
                _Loop = value;
                OnPropertyChanged("Loop");
            }
        }

        public bool AutoRunInter
        {
            get { return _AutoRunInter; }
            set
            {
                _AutoRunInter = value;
                OnPropertyChanged("AutoRunInter");
            }
        }

        public bool DelCfile
        {
            get { return _DelCfile; }
            set
            {
                _DelCfile = value;
                OnPropertyChanged("DelCfile");
            }
        }

        public Visibility StopScanVisibility
        {
            get { return _StopScanVisibility; }
            set
            {
                _StopScanVisibility = value;
                OnPropertyChanged("StopScanVisibility");
            }
        }

        public Visibility StopReceiverVisibility
        {
            get { return _StopReceiverVisibility; }
            set
            {
                _StopReceiverVisibility = value;
                OnPropertyChanged("StopReceiverVisibility");
            }
        }
        public Visibility StartReceiverVisibility
        {
            get { return _StartReceiverVisibility; }
            set
            {
                _StartReceiverVisibility = value;
                OnPropertyChanged("StartReceiverVisibility");
            }
        }

        public Visibility StartScanVisibility
        {
            get { return _StartScanVisibility; }
            set
            {
                _StartScanVisibility = value;
                OnPropertyChanged("StartScanVisibility");
            }
        }

        #endregion

        #region Constructor
        public ReceiverUI()
        {
            StartReceiver = new RelayCommandControls(OnStartReceiver, CanStartStopReceiver);
            StopReceiver = new RelayCommandControls(OnStopReceiver, CanStartStopReceiver);
            StartScan = new RelayCommandControls(OnStartScan, CanStartStopScan);
            StopScan = new RelayCommandControls(OnStopScan, CanStartStopScan);

            Units = TransferDB.Units;
            computers = TransferDB.Computers;
          
            InitializeComponent();
        }


        #endregion
        #region Methods
        //GAL
        private ObservableCollection<Unit> Units;
        private ObservableCollection<Computer> computers;
        readonly string path = "/home/mainuser";
        private void OnStartReceiver()
        {
            //Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            //UnitView arfcn = Transfer.SelectedUnitView;
            //int mood = Loop ? 1 : 0;
            //StopReceiverVisibility = Visibility.Visible;

            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            ARFCN arfcn = Transfer.SelectedARFCN;
            int mood = Loop ? 1 : 0;
            if (Recorder.Start(cpu, 1, arfcn.Band, arfcn.Chanel.Value, Convert.ToDouble(arfcn.Frequency) * 1000000, Gain, 400000, Length, NOR, mood, path))
            {
                StopReceiverVisibility = Visibility.Visible;//TODO  visible and collapsed both buttons at the end of the record
                StartReceiverVisibility = Visibility.Collapsed;
            }
        }
        private void OnStopReceiver()
        {
            //Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            //StopReceiverVisibility = Visibility.Collapsed;

            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            if (Recorder.Stop(cpu, 1))
            {
                StopReceiverVisibility = Visibility.Collapsed;//TODO  visible and collapsed both buttons at the end of the receiver
                StartReceiverVisibility = Visibility.Visible;
            }
        }
        private bool CanStartStopReceiver()
        {
            //return true;
            return Transfer.SelectedARFCN != null && Transfer.SelectedUnit != null;
        }




        private void OnStartScan()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            UnitView arfcn = Transfer.SelectedUnitView;
            int mood = Loop ? 1 : 0;
            IsEnabled = false;
            StartScanVisibility = Visibility.Collapsed;
            StopScanVisibility = Visibility.Visible;


            //UpdateBand();
            //    Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == SelectedUnit.ID);
            //    if (Scanner.Start(cpu, 1, BroadcastIP, ScannerListenerPort, band, Gain, Speed, sample_rate, 0))
            //    {
            //        if (!listenerWorker.IsBusy)
            //            listenerWorker.RunWorkerAsync(ScannerListenerPort);
            //        IsEnabled = false;
            //        IsCancelVisible = Visibility.Visible;
            //    }
            ;
        }
        private void OnStopScan()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            StopScanVisibility = Visibility.Collapsed;
            StartScanVisibility = Visibility.Visible;

        }
        private bool CanStartStopScan()
        {
            //return true;
            return Transfer.SelectedARFCN != null && Transfer.SelectedUnit != null;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}
