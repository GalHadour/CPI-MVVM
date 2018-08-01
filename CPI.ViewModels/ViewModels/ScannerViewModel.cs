using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CPI.DTO;
using CPI.Models;
using CPI.Models.Entity;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CPI.ViewModels
{
    public class ScannerViewModel : ViewModelBase
    {

        #region Create Settings

        readonly int ScannerListenerPort = 13000;
        readonly string BroadcastIP = "255.255.255.255";

        #endregion

        #region Fields
        private IDialogCoordinator dialogCoordinator;
        private Visibility _IsCancelVisible = Visibility.Collapsed;
        private int _Gain;
        private int _Speed;
        private int _Depth;
        private int sample_rate;
        private string band;
        private bool _GSM850;
        private bool _GSM900;
        private bool _DCS1800;
        private bool _PCS1900;
        private Unit _SelectedUnit;
        private ARFCN _SelectedARFCN;
        private ObservableCollection<Unit> _Units;
        private ObservableCollection<Computer> computers;
        private BackgroundWorker listenerWorker;
        private ObservableCollection<ARFCN> _ListARFCNs;
        private bool _IsEnabled;
        private UdpClient listener;
        private Visibility _MoreVisibility;

        #endregion

        #region Properties

        public RelayCommand Loaded { get; set; }
        public RelayCommand Unloaded { get; set; }
        public RelayCommand Scan { get; set; }
        public RelayCommand Cancel { get; set; }
        public RelayCommand History { get; set; }
        public RelayCommand AddNew { get; set; }

        public RelayCommand ShowARFCN { get; set; }
        public RelayCommand Close { get; private set; }
        public RelayCommand Delete { get; private set; }
        public RelayCommand Attach { get; private set; }

        public ObservableCollection<Unit> Units
        {
            get { return _Units; }
            set
            {
                _Units = value;
                OnPropertyChanged("Units");
            }
        }



        public Visibility IsCancelVisible
        {
            get { return _IsCancelVisible; }
            set
            {
                _IsCancelVisible = value;
                OnPropertyChanged("IsCancelVisible");
            }
        }

        public int Gain
        {
            get { return _Gain; }
            set
            {
                _Gain = value;
                OnPropertyChanged("Gain");
            }
        }
        public int Speed
        {
            get { return _Speed; }
            set
            {
                _Speed = value;
                OnPropertyChanged("Speed");
            }
        }
        public int Depth
        {
            get { return _Depth; }
            set
            {
                _Depth = value;
                UpdateDepth();
                OnPropertyChanged("Depth");
            }
        }
        public bool GSM850
        {
            get { return _GSM850; }
            set
            {
                _GSM850 = value;
                OnPropertyChanged("GSM850");
            }
        }
        public bool GSM900
        {
            get { return _GSM900; }
            set
            {
                _GSM900 = value;
                OnPropertyChanged("GSM900");
            }
        }
        public bool DCS1800
        {
            get { return _DCS1800; }
            set
            {
                _DCS1800 = value;
                OnPropertyChanged("DCS1800");
            }
        }
        public bool PCS1900
        {
            get { return _PCS1900; }
            set
            {
                _PCS1900 = value;
                OnPropertyChanged("PCS1900");
            }
        }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        

        public Unit SelectedUnit
        {
            get { return _SelectedUnit; }
            set
            {
                _SelectedUnit = value;
                Transfer.SelectedUnit = _SelectedUnit;
                Scan.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedUnit");
            }
        }

        public ARFCN SelectedARFCN
        {
            get { return _SelectedARFCN; }
            set
            {
                _SelectedARFCN = value;
                OnPropertyChanged("SelectedARFCN");
            }
        }
        public ObservableCollection<ARFCN> ListARFCNs
        {
            get { return _ListARFCNs; }
            set
            {
                _ListARFCNs = value;
                OnPropertyChanged("ListARFCNs");
            }
        }

        public Visibility MoreVisibility
        {
            get { return _MoreVisibility; }
            set
            {
                _MoreVisibility = value;
                OnPropertyChanged("MoreVisibility");
            }
        }


        #endregion

        #region Constructor
        public ScannerViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            Loaded = new RelayCommand(OnLoaded);
            Unloaded = new RelayCommand(OnUnloaded);
            Scan = new RelayCommand(OnScan, CanScan);
            Cancel = new RelayCommand(OnCancel);
            History = new RelayCommand(OnHistory);
            AddNew = new RelayCommand(OnAddNew);
            ShowARFCN = new RelayCommand(OnShowARFCN);
            Close = new RelayCommand(OnClose);
            Delete = new RelayCommand(OnDelete);
            Attach = new RelayCommand(OnAttach);

        }



        public void Initialized()
        {

            _ListARFCNs = new ObservableCollection<ARFCN>();
            _ListARFCNs.CollectionChanged += ListARFCNs_CollectionChanged;

            listenerWorker = new BackgroundWorker();
            listenerWorker.DoWork += ListenerWorker_DoWork;
            listenerWorker.ProgressChanged += ListenerWorker_ProgressChanged;
            listenerWorker.RunWorkerCompleted += ListenerWorker_RunWorkerCompleted;
            listenerWorker.WorkerReportsProgress = true;
            listenerWorker.WorkerSupportsCancellation = true;


            IsCancelVisible = Visibility.Collapsed;
            Gain = 24;
            Speed = 5;
            Depth = 0;

            GSM900 = true;

            Units = TransferDB.Units;
            computers = TransferDB.Computers;

            IsEnabled = true;
            MoreVisibility = Visibility.Collapsed;

#if (DEBUG)
#else
#endif
        }



        #endregion

        #region Commands Methods

        private void OnAttach()
        {
            Transfer.HamburgerMenuControl.SelectedIndex = 3;
            MoreVisibility = Visibility.Collapsed;
            Transfer.SelectedARFCN = SelectedARFCN;
        }

        private void OnDelete()
        {
            MoreVisibility = Visibility.Collapsed;
            ListARFCNs.Remove(SelectedARFCN);
        }

        private void OnClose()
        {
            MoreVisibility = Visibility.Collapsed;
        }


        private void OnShowARFCN()
        {
            MoreVisibility = Visibility.Visible;
        }

        private void OnAddNew()
        {
            
        }

        private void OnCancel()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == SelectedUnit.ID);
            if (Scanner.Cancel(cpu))
            {
                if (listenerWorker.IsBusy)
                    listenerWorker.CancelAsync();
                listener.Close();
                IsCancelVisible = Visibility.Collapsed;
            }
        }

        private bool CanScan()
        {
            return SelectedUnit != null;
        }

        private void OnScan()
        {

            UpdateBand();
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == SelectedUnit.ID);
            if (Scanner.Start(cpu, 1, BroadcastIP, ScannerListenerPort, band, Gain, Speed, sample_rate, 0))
            {
                if (!listenerWorker.IsBusy)
                    listenerWorker.RunWorkerAsync(ScannerListenerPort);
                IsEnabled = false;
                IsCancelVisible = Visibility.Visible;
            }



        }

        private void OnHistory()
        {
        }

        private void OnLoaded()
        {
        }
        private void OnUnloaded()
        {
        }


        #endregion






        #region Listener Worker
        private void ListenerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int port = (int)e.Argument;

            listener = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                if (worker.CancellationPending && worker.IsBusy)
                {
                    e.Cancel = true;
                    listener.Close();
                    break;
                }
                else
                {
                    byte[] byteSource = listener.Receive(ref endPoint);
                    string data = encoder.GetString(byteSource);

                    if (data.IndexOf("DONE") > -1)
                    {
                        e.Cancel = true;
                        listener.Close();
                        break;
                    }
                    else
                        worker.ReportProgress(0, data);
                }
            }
        }
        private void ListenerWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string stream = (string)e.UserState;

            string[] chunks = stream.Split(new string[] { "|" }, StringSplitOptions.None);
            ARFCN arfcn;
            if (chunks[0] == "ARFCN")
            {
                arfcn = new ARFCN();
                arfcn.ID = Guid.NewGuid();
                arfcn.Chanel = Convert.ToInt32(chunks[1]);
                arfcn.Frequency = chunks[2].Remove(chunks[2].Length - 1);
                arfcn.CI = Convert.ToInt32(chunks[3]);
                arfcn.LAC = Convert.ToInt32(chunks[4]);
                arfcn.MCC = Convert.ToInt32(chunks[5]);
                arfcn.MNC = Convert.ToInt32(chunks[6]);
                arfcn.Power = Convert.ToInt32(chunks[7]);


                arfcn.Configuration = chunks[8];
                arfcn.CellARFCNs = chunks[9];
                arfcn.NeighbourCells = chunks[10];

                arfcn.System = "GSM";
                arfcn.Band = band;

                ListARFCNs.Add(arfcn);
            }

            
                           

            //int mcc = Convert.ToInt32(arfcn.MCC);
            //int mnc = Convert.ToInt32(arfcn.MNC);
            //Provider provider = ManagerDB.FindProvider(mcc, mnc);
            //if (provider != null)
            //{
            //    arfcn.ProviderID = provider.ID;
            //    arfcn.Country = provider.Country;
            //    arfcn.Network = provider.Network;
            //    arfcn.Operator = provider.Operator;
            //    arfcn.Logo = provider.Logo;
            //}
            //worker.ReportProgress(scannCount++, new ScannerStatus() { Title = null, ARFCN = arfcn });
        }
        private void ListenerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsEnabled = true;
            IsCancelVisible = Visibility.Collapsed;
        }
        #endregion



        #region General Methods


        private void UpdateBand()
        {
            if (GSM850)
                band = "GSM850";
            else if (GSM900)
                band = "GSM900";
            else if (DCS1800)
                band = "DCS1800";
            else
                band = "PCS1900";
        }
        private void UpdateDepth()
        {
            switch (_Depth)
            {
                case 0:
                    sample_rate = 2000000;
                    break;
                case 1: // also give result with errors
                    sample_rate = 800000;
                    break;
                case 2: // very slow scanning but result is clear
                    sample_rate = 400000;
                    break;
                default:
                    sample_rate = 2000000;
                    break;
            }
        }
        private void ListARFCNs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SequencingService.SetCollectionSequence(ListARFCNs);
        }
        private async void ShowMessageAsync(string header, string message, MessageDialogStyle dialogStyle)
        {
            var settings = new MetroDialogSettings
            {
                AnimateShow = false
            };
            await dialogCoordinator.ShowMessageAsync(this, header, message, dialogStyle, settings);
        }
        public async static Task<MessageDialogResult> ShowMessageAsync(string title, string Message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            if (settings == null)
                settings = new MetroDialogSettings
                {
                    AnimateShow = false
                };
            return await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync(title, Message, style, settings);
        }

        #endregion


    }
}
