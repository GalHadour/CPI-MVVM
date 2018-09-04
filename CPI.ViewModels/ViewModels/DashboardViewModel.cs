using Controls.ButttonDashboard;
using Controls.ReceiverUnit;
using CPI.DTO;
using CPI.Models;
using CPI.Models.Models;
using CPI.Models.ItemsInfo;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CPI.Models.Entity;
using Controls;


namespace CPI.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {


        #region Create Settings

        readonly string BroadcastIP = "255.255.255.255";
        readonly int BroadcastPort = 13156;


        #endregion

        #region Fields
        private Visibility _UnitDetailsVisibility = Visibility.Collapsed;
        private BackgroundWorker listener;
        private UdpClient client;

      

        #endregion

        #region Properties
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
        public RelayCommand Loaded { get; private set; }
        public RelayCommand Unloaded { get; private set; }
        //GAL start
        //private bool _LoopReceiver = false;
        //private bool _AutoRunInter = false;
        //private bool _DelCfile = false;
        //public RelayCommand StartReceiver { get; set; }
        //public RelayCommand StopReceiver { get; set; }
        //public RelayCommand StartScan { get; set; }
        //public RelayCommand StopScan { get; set; }

        private static int _UnitNumber = 0;
        private static int _ItemsNumber = 0;


        private ObservableCollection<Unit> Units;
        private ObservableCollection<Computer> computers;

    

        public ReceiverUI receiverUI1 { get; set; }
        public ReceiverUI receiverUI2 { get; set; }
        public ReceiverUI receiverUI3 { get; set; }
        public ReceiverUI receiverUI4 { get; set; }
        public DashboardButton dashboardButtonFiles { get; set; }
        public DashboardButton dashboardButtonRecordFiles { get; set; }
        public DashboardButton dashboardButtonPhoneCalls { get; set; }
        public DashboardButton dashboardButtonSmsMessages { get; set; }

        public UnitView ARFCN1 { get; set; }
        public UnitView ARFCN2 { get; set; }
        public UnitView ARFCN3 { get; set; }
        public UnitView ARFCN4 { get; set; }



        //public int GainReceiver{ get; set; }
        //public int NORReceiver { get; set; }
        //public int LengthReceiver { get; set; }

        //private Visibility _StopScanVisibility = Visibility.Collapsed;
        //private Visibility _StopReceiverVisibility = Visibility.Collapsed;


        //public Visibility StopScanVisibility
        //{
        //    get { return _StopScanVisibility; }
        //    set
        //    {
        //        _StopScanVisibility = value;
        //        OnPropertyChanged("StopScanVisibility");
        //    }
        //}

        //public Visibility StopReceiverVisibility
        //{
        //    get { return _StopReceiverVisibility; }
        //    set
        //    {
        //        _StopReceiverVisibility = value;
        //        OnPropertyChanged("StopReceiverVisibility");
        //    }
        //}



        //public bool LoopReceiver
        //{
        //    get { return _LoopReceiver; }
        //    set
        //    {
        //        _LoopReceiver = value;
        //        OnPropertyChanged("LoopReceiver");
        //    }
        //}

        //public bool AutoRunInter
        //{
        //    get { return _AutoRunInter; }
        //    set
        //    {
        //        _AutoRunInter = value;
        //        OnPropertyChanged("AutoRunInter");
        //    }
        //}

        //public bool DelCfile
        //{
        //    get { return _DelCfile; }
        //    set
        //    {
        //        _DelCfile = value;
        //        OnPropertyChanged("DelCfile");
        //    }
        //}




        public int UnitNumber
        {
            get { return ++_UnitNumber; }
            set
            {
                _UnitNumber = value;
                OnPropertyChanged("UnitNumber");
            }
        }


        public int ItemsNumber
        {
            get { return _ItemsNumber; }
            set
            {
                _ItemsNumber = value;
                OnPropertyChanged("ItemsNumber");
            }
        }
        

        //GAL END

        #endregion

        #region Constructor
        public DashboardViewModel()
        {
            Loaded = new RelayCommand(OnLoaded);
            Unloaded = new RelayCommand(OnUnloaded);
            ShowSMS = new RelayCommand(OnShowSMS);
            ShowCalls = new RelayCommand(OnShowCalls);
            ShowReceivers = new RelayCommand(OnShowReceivers);
            ShowRecords = new RelayCommand(OnShowRecords);
            ShowUnitDetails = new RelayCommand(OnShowUnitDetails);
            CloseUnitDetails = new RelayCommand(OnCloseUnitDetails);
            ////GAL START
          //StartReceiver = new RelayCommand(OnStartReceiver, CanStartStopReceiver);
          //  StopReceiver = new RelayCommand(OnStopReceiver, CanStartStopReceiver);
          //  StartScan = new RelayCommand(OnStartScan, CanStartStopScan);
          //  StopScan = new RelayCommand(OnStopScan, CanStartStopScan);
            ////GAL END

            listener = new BackgroundWorker();
            listener.DoWork += Listener_DoWork;
            listener.ProgressChanged += Listener_ProgressChanged;
            listener.RunWorkerCompleted += Listener_RunWorkerCompleted;
            listener.WorkerReportsProgress = true;
            listener.WorkerSupportsCancellation = true;

            //GAL
            InitializeDashboard();
     
            //GAL

        }


        //GAL start
        private void InitializeDashboard()
        {

            receiverUI1 = new ReceiverUI();
            receiverUI2 = new ReceiverUI();
            receiverUI3 = new ReceiverUI();
            receiverUI4 = new ReceiverUI();

            ARFCN1 = new UnitView();
            ARFCN2 = new UnitView();
            ARFCN3 = new UnitView();
            ARFCN4 = new UnitView();

            SetReceiverUI(receiverUI1, 1);
            SetReceiverUI(receiverUI2, 2);
            SetReceiverUI(receiverUI3, 3);
            SetReceiverUI(receiverUI4, 4);

            BrushConverter conv = new BrushConverter();

            dashboardButtonFiles = new DashboardButton();
            FilesInfo filesButton = new FilesInfo();
            SetDashboardButton("#ffea6c41", filesButton.ItemName, filesButton.Count, PackIconMaterialKind.File, dashboardButtonFiles);

            dashboardButtonRecordFiles = new DashboardButton();
            RecordsInfo recordsButton = new RecordsInfo();
            SetDashboardButton("#ffe69a2a", recordsButton.ItemName, recordsButton.Count, PackIconMaterialKind.RecordRec, dashboardButtonRecordFiles);

            dashboardButtonPhoneCalls = new DashboardButton();
            CallsInfo callButton = new CallsInfo();
            SetDashboardButton("#FF469408", callButton.ItemName, callButton.Count, PackIconMaterialKind.PhoneLog, dashboardButtonPhoneCalls);

            dashboardButtonSmsMessages = new DashboardButton();
            SmsInfo smsButton = new SmsInfo();
            SetDashboardButton("#FF177EC1", smsButton.ItemName, smsButton.Count, PackIconMaterialKind.EmailOpen, dashboardButtonSmsMessages);


            Units = TransferDB.Units;
            computers = TransferDB.Computers;
        }

        #endregion

        private void SetReceiverUI(ReceiverUI receiverObj,int unitNumber)
        {
            receiverObj.UnitNumber = unitNumber;
        }

        private void SetDashboardButton(string color, string name, int count, PackIconMaterialKind iconKind, DashboardButton dashboardButton)
        {
            BrushConverter conv = new BrushConverter();

            dashboardButton.ButtonName = name;
            dashboardButton.ColorButton = conv.ConvertFromString(color) as SolidColorBrush;
            dashboardButton.ItemsNumber = count;
            dashboardButton.IconKind = iconKind;
        }

        ////GAL
        //private bool _IsEnabledReceiver = true;
        //private bool _StopScanVisi = false;
        //public bool StopScanVisi
        //{
        //    get { return _StopScanVisi; }
        //    set
        //    {
        //        _StopScanVisi = value;
        //        OnPropertyChanged("StopSacnVisi");
        //    }
        //}
 

        //public bool IsEnabledReceiver
        //{
        //    get { return _IsEnabledReceiver; }
        //    set
        //    {
        //        _IsEnabledReceiver = value;
        //        OnPropertyChanged("IsEnabledReceiver");
        //    }
        //}
        //readonly string path = "/home/mainuser";
        //private void OnStartReceiver()
        //{
        //    Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
        //    ARFCN arfcn = Transfer.SelectedARFCN;
        //    int mood = receiverUI1.LoopReceiver ? 1 : 0;
        //    if (Recorder.Start(cpu, 1, arfcn.Band, arfcn.Chanel.Value, Convert.ToDouble(arfcn.Frequency) * 1000000, receiverUI1.GainReceiver, 400000, receiverUI1.LengthReceiver, receiverUI1.NORReceiver, mood, path))
        //        receiverUI1.StopReceiverVisibility = Visibility.Visible;
        //}
        //private void OnStopReceiver()
        //{
        //    Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
        //    if (Recorder.Stop(cpu, 1))
        //        receiverUI1.StopReceiverVisibility = Visibility.Collapsed;
        //}
        //private bool CanStartStopReceiver()
        //{
        //    return Transfer.SelectedARFCN != null && Transfer.SelectedUnit != null;
        //}




        //private void OnStartScan()
        //{
        //    Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
        //    ARFCN arfcn = Transfer.SelectedARFCN;
        //    int mood = receiverUI1.LoopReceiver ? 1 : 0;
        //    if (Recorder.Start(cpu, 1, arfcn.Band, arfcn.Chanel.Value, Convert.ToDouble(arfcn.Frequency) * 1000000, receiverUI1.GainReceiver, 400000, receiverUI1.LengthReceiver, receiverUI1.NORReceiver, mood, path))
        //        receiverUI1.StopScanVisibility = Visibility.Visible;
        //}
        //private void OnStopScan()
        //{
        //    Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
        //    if (Recorder.Stop(cpu, 1))
        //        receiverUI1.StopScanVisibility = Visibility.Collapsed;
        //}
        //private bool CanStartStopScan()
        //{
        //    return Transfer.SelectedARFCN != null && Transfer.SelectedUnit != null;
        //}
        //GAL ends

        #region Commands

        private void OnLoaded()
        {
            if (!listener.IsBusy)
                listener.RunWorkerAsync(BroadcastPort);
        }
        private void OnUnloaded()
        {
            //if (listener.IsBusy)
            //{
            //    client.Close();
            //    listener.CancelAsync();

            //}
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
        #endregion

        #region Listener Worker
        private void Listener_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int port = (int)e.Argument;

            client = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                if (worker.CancellationPending && worker.IsBusy)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    byte[] byteSource = client.Receive(ref endPoint);
                    string data = encoder.GetString(byteSource);

                    string[] chunks = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (chunks.Length > 7)
                    {
                        worker.ReportProgress(0, new Status
                        {
                            Host = chunks[0],
                            Recorder = chunks[1] == "1",
                            Interceptor = chunks[2] == "1",
                            Scanner = chunks[3] == "1",
                            CPU = Double.Parse(chunks[4]),
                            RAM = Double.Parse(chunks[5]),
                            TEMP = Double.Parse(chunks[6]),
                            HDD = Double.Parse(chunks[7])
                        });
                    }

                }
            }
        }
        private void Listener_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Status message = (Status)e.UserState;
            switch (message.Host)
            {
                case "CPU1":
                    {

                    }
                    break;
                case "CPU2":
                    {

                    }
                    break;
                case "CPU3":
                    {

                    }
                    break;

                default:
                    break;
            }

        }
        private void Listener_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

                if (e.Error is SocketException)
                {
                }
                else
                {
#if (DEBUG)
                    //ShowMessageAsync("ERROR", "An unexpected error occurred while UDP client running\n" +
                    //                    e.Error.Message, MessageDialogStyle.Affirmative);
                    //Restart background worker!!!
                    listener.RunWorkerAsync(BroadcastPort);
#endif
                }
            }
            else if (e.Cancelled)
            {

            }
            else
            {

            }
        }
        #endregion



        #region Methods
    

        #endregion
    }



}
