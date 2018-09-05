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
using System.Windows.Documents;
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
        private Visibility _IsScanVisible = Visibility.Visible;
        
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
        private ObservableCollection<Computer> _Computers;
        private BackgroundWorker listenerWorker;
        private ObservableCollection<ARFCN> _ListARFCNs;
        private bool _IsEnabled;
        private bool _IsEnabledSession=true;
        private UdpClient listener;
        private Visibility _MoreVisibility;
        private Receiver _SelectedReceiver;
        private ObservableCollection<Receiver> _RxList;

        private ObservableCollection<Session> _SessionsList;
        //private Visibility _SaveVisi = Visibility.Collapsed;
        //private Visibility _LoadVisi = Visibility.Collapsed;
        //private Visibility _WarningNSVisibility = Visibility.Collapsed;
        //private Visibility _DeleteAllSessionsVisibility = Visibility.Collapsed;
        //private Visibility _DelSelectedSessionVisibility = Visibility.Collapsed;

        private Visibility _SaveVisibility = Visibility.Collapsed;
        



        private string _SessionNameToSave;
        private Session _SelectedSession;
        private Session _SessionToSave;



        #endregion

        #region Properties
        public RelayCommand SaveAs { get; set; }
        public RelayCommand Save { get; set; }
        public RelayCommand Load { get; set; }
        public RelayCommand CancelSave { get; set; }
        //public RelayCommand YesSave { get; set; }
        //public RelayCommand NoSave { get; set; }
        //public RelayCommand YesLoad { get; set; }
        //public RelayCommand NoLoad { get; set; }
        //public RelayCommand OKWarningNS { get; set; }
        public RelayCommand DeleteAllSessions { get; set; }
        //public RelayCommand YesDeleteAllSessions { get; set; }
        //public RelayCommand NoDeleteAllSessions { get; set; }
        //public RelayCommand DeleteSelectedSession { get; set; }
        //public RelayCommand YesDelSelectedSession { get; set; }
        //public RelayCommand NoDelSelectedSession { get; set; }


        //public RelayCommand Loaded { get; set; }
        //public RelayCommand Unloaded { get; set; }
        public RelayCommand Scan { get; set; }
        public RelayCommand Cancel { get; set; }
        //public RelayCommand History { get; set; }
        //public RelayCommand AddNew { get; set; }

        public RelayCommand ShowARFCN { get; set; }
        public RelayCommand Close { get; private set; }
        //public RelayCommand Delete { get; private set; }
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

        public ObservableCollection<Computer> Computers
        {
            get { return _Computers; }
            set
            {
                _Computers = value;
                OnPropertyChanged("Computers");
            }
        }


        public ObservableCollection<Receiver> RxList
        {
            get
            {
                //return _RxList;  
               return new ObservableCollection<Receiver>(_RxList.Where(s => s.IsAttached==false).ToList());
              //  return new ObservableCollection<Receiver>(_RxList.Where(s => s.ARFCN_ID == null).ToList());

            }
            set
            {
                _RxList = value;
                OnPropertyChanged("RxList");
            }
        }


        public Receiver SelectedReceiver
        {
            get { return _SelectedReceiver; }
            set
            {
                _SelectedReceiver = value;
                Attach.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedReceiver");
                //OnPropertyChanged("SelectedReceiverName");
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

        public Visibility IsScanVisible
        {
            get { return _IsScanVisible; }
            set
            {
                _IsScanVisible = value;
                OnPropertyChanged("IsScanVisible");
            }
        }

        public Visibility SaveVisibility
        {
            get { return _SaveVisibility; }
            set
            {
                _SaveVisibility = value;
                OnPropertyChanged("SaveVisibility");
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

        public bool IsEnabledSession
        {
            get { return _IsEnabledSession; }
            set
            {
                _IsEnabledSession = value;
                OnPropertyChanged("IsEnabledSession");
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
                _ListARFCNs= value;
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

        public ObservableCollection<Session> SessionsList
        {
            get { return _SessionsList; }
            set
            {

                _SessionsList = value;
                OnPropertyChanged("SessionsList");
            }
        }

        //public Visibility SaveVisibility
        //{
        //    get { return _SaveVisi; }
        //    set
        //    {
        //        _SaveVisi = value;
        //        OnPropertyChanged("SaveVisibility");
        //    }
        //}

        //public Visibility LoadVisibility
        //{
        //    get { return _LoadVisi; }
        //    set
        //    {
        //        _LoadVisi = value;
        //        OnPropertyChanged("LoadVisibility");
        //    }
        //}

        //public Visibility WarningNSVisibility
        //{
        //    get { return _WarningNSVisibility; }
        //    set
        //    {
        //        _WarningNSVisibility = value;
        //        OnPropertyChanged("WarningNSVisibility");
        //    }
        //}

        //public Visibility DeleteAllSessionsVisibility
        //{
        //    get { return _DeleteAllSessionsVisibility; }
        //    set
        //    {
        //        _DeleteAllSessionsVisibility = value;
        //        OnPropertyChanged("DeleteAllSessionsVisibility");
        //    }
        //}

        //public Visibility DelSelectedSessionVisibility
        //{
        //    get { return _DelSelectedSessionVisibility; }
        //    set
        //    {
        //        _DelSelectedSessionVisibility = value;
        //        OnPropertyChanged("DelSelectedSessionVisibility");
        //    }
        //}

        private string _SelectedReceiverName = "NS";
        public string SelectedReceiverName
        {
            get
            {
                if (SelectedARFCN != null && _RxList.Count != 0 && SelectedReceiver != null )
                {
                    if (SelectedARFCN.RxName != null)
                    {
                        SelectedReceiver = _RxList.FirstOrDefault(r => r.Name == SelectedARFCN.RxName);

                        _SelectedReceiverName = SelectedReceiver.Name;
                    }
                }
                else if (_SelectedReceiverName == null)
                {
                    _SelectedReceiverName = "NS";
                }
                return _SelectedReceiverName;
            }
            set
            {
                _SelectedReceiverName = value;

                OnPropertyChanged("SelectedReceiverName");

            }
        }


        public string SessionNameToSave
        {
            get { return _SessionNameToSave; }
            set
            {
                _SessionNameToSave = value;
               
                OnPropertyChanged("SessionNameToSave");
                SaveAs.RaiseCanExecuteChanged();
            }
        }

        public Session SelectedSession
        {
            get { return _SelectedSession; }
            set
            {
                _SelectedSession = value;
                //DeleteSelectedSession.RaiseCanExecuteChanged();
                Load.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedSession");
             
            }
        }


        public Session SessionToSave
        {
            get { return _SessionToSave; }
            set
            {
                _SessionToSave = value;
                OnPropertyChanged("SessionToSave");
            }
        }
        #endregion

        #region Constructor
        public ScannerViewModel(IDialogCoordinator instance)
        {
            dialogCoordinator = instance;

            Save = new RelayCommand(OnSave);
            SaveAs = new RelayCommand(OnSaveAs, CanSaveAs);
            Load = new RelayCommand(OnLoad, CanLoad);
            //YesSave = new RelayCommand(OnYesSave);
            //NoSave = new RelayCommand(OnNoSave);
            //YesLoad = new RelayCommand(OnYesLoad);
            //NoLoad = new RelayCommand(OnNoLoad);
            //OKWarningNS = new RelayCommand(OnOKWarningNS);
            DeleteAllSessions = new RelayCommand(OnDeleteAllSessions, CanDeleteAllSessions);
            //YesDeleteAllSessions = new RelayCommand(OnYesDeleteAllSessions);
            //NoDeleteAllSessions = new RelayCommand(OnNoDeleteAllSessions);
            //DeleteSelectedSession = new RelayCommand(OnDeleteSelectedSession, CanDeleteSelectedSession);
            //YesDelSelectedSession = new RelayCommand(OnYesDelSelectedSession);
            //NoDelSelectedSession = new RelayCommand(OnNoDelSelectedSession);
            CancelSave = new RelayCommand(OnCancelSave);

            //Loaded = new RelayCommand(OnLoaded);
            //Unloaded = new RelayCommand(OnUnloaded);
            Scan = new RelayCommand(OnScan, CanScan);
            Cancel = new RelayCommand(OnCancel,CanCancel);
            //History = new RelayCommand(OnHistory);
            //AddNew = new RelayCommand(OnAddNew);
            ShowARFCN = new RelayCommand(OnShowARFCN);
            Close = new RelayCommand(OnClose);
            //Delete = new RelayCommand(OnDelete);
            Attach = new RelayCommand(OnAttach, CanAttach);
            SelectedUnit = new Unit();
         
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

            _Units = TransferDB.Units;
            _Computers = TransferDB.Computers;
            _SessionsList = TransferDB.SessionsList;
            _RxList = TransferDB.Receivers;

            _RxList.Insert(0, new Receiver
            {
                Name = "NS",
                ARFCN_ID = null
            });
            OnPropertyChanged("RxList");

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
            Transfer.SelectedARFCN = SelectedARFCN;

            if (SelectedReceiver.Name == "NS" && !string.IsNullOrEmpty(SelectedARFCN.RxName))
            {
                _RxList.FirstOrDefault(d => d.Name == SelectedARFCN.RxName).ARFCN_ID = null;
                SelectedReceiver.ARFCN = SelectedARFCN;

                SelectedReceiver.IsAttached = false;
                SelectedARFCN.RxName = "";
            }
            //else if (SelectedReceiver.Name != "NS" && !string.IsNullOrEmpty(SelectedARFCN.RxName) && SelectedReceiver.Name != SelectedARFCN.RxName) //TODO maybe add
            //{
            //    WarningNSVisibility = Visibility.Visible;
            //}
            else if (SelectedReceiver.Name != "NS")
            {
                SelectedReceiver.ARFCN_ID = SelectedARFCN.ID;
                SelectedReceiver.IsAttached = true;
                SelectedARFCN.RxName = SelectedReceiver.Name;
            }
            MoreVisibility = Visibility.Collapsed;
            //SelectedReceiver = null;
            OnPropertyChanged("RxList");
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
          
            if (SelectedARFCN != null)
            {
                if (SelectedReceiver == null && !string.IsNullOrEmpty(SelectedARFCN.RxName))
                {
                    SelectedReceiver = _RxList.FirstOrDefault(r => r.Name == SelectedARFCN.RxName);
                    SelectedReceiver.IsAttached = false;
                }

                if (SelectedARFCN.RxName != null)
                {
                    //SelectedReceiver = _RxList.FirstOrDefault(d => d.IsAttached == true);
                    // SelectedReceiver.ARFCN = SelectedARFCN;

                    Receiver r = _RxList.FirstOrDefault(d => d.ARFCN_ID == SelectedARFCN.ID);//TODO

                    SelectedReceiver = r;

                    //SelectedReceiver = _RxList.FirstOrDefault(d => d.Name == SelectedARFCN.RxName);

                    //TODO: Add IsAttached property to the receiver extension and continue with this parameter to hide receiver from RxList 
                }
                //OnPropertyChanged("SelectedReceiverName");///
                MoreVisibility = Visibility.Visible;
            }   
        }

        //private void OnAddNew()
        //{
            
        //}

        private void OnCancel()
        {
            Computer cpu = Computers.FirstOrDefault(c => c.Unit_ID == SelectedUnit.ID);
            if (Scanner.Cancel(cpu))
            {
                if (listenerWorker.IsBusy)
                    listenerWorker.CancelAsync();
                listener.Close();
                IsScanVisible = Visibility.Visible;
                IsCancelVisible = Visibility.Collapsed;
            }
        }

        private bool CanScan()
        {
            return SelectedUnit != null;
        }

        private bool CanAttach()
        {
            return SelectedReceiver != null;
        }

       
        private void OnScan()
        {
            ListARFCNs.Clear();
            TransferDB.ARFCNs.Clear();
        

            SelectedUnit =Units[0];
            UpdateBand();
            Computer cpu = Computers.FirstOrDefault(c => c.Unit_ID == SelectedUnit.ID);
       
            if (Scanner.Start(cpu, 1, BroadcastIP, ScannerListenerPort, band, Gain, Speed, sample_rate, 0))
            {
              
                IsScanVisible = Visibility.Collapsed;
                IsCancelVisible = Visibility.Visible;
                Cancel.RaiseCanExecuteChanged();
                if (!listenerWorker.IsBusy)
                    listenerWorker.RunWorkerAsync(ScannerListenerPort);
                IsEnabled = false;
                IsEnabledSession = false;
            }
        }

        //private void OnHistory()
        //{
        //}

        //private void OnLoaded()
        //{

        //}
        //private void OnUnloaded()
        //{


        //}


        //private void OnSave()
        //{
        //    SaveVisibility = Visibility.Visible;
        //}

        private bool CanSaveAs()
        {
            return (!string.IsNullOrEmpty(SessionNameToSave));        
        }

        private bool CanCancel()
        {
            return IsScanVisible == Visibility.Collapsed;
        }
        //private void OnLoad()
        //{
        //    LoadVisibility = Visibility.Visible;
        //}

        private bool CanLoad()
        {
            return SelectedSession != null;
        }

        private bool CanDeleteAllSessions()
        {
            return SessionsList.Count > 0;
        }

        //private void OnYesSave()
        //{
        //    SessionToSave = new Session();
        //    if(ListARFCNs.Count != 0 && !string.IsNullOrEmpty(SessionNameToSave))//TODO add ListARFCNs.Count!=0 &&
        //    {
        //       SessionToSave.ID = Guid.NewGuid(); 
        //        SessionToSave.Name = SessionNameToSave;
        //        SessionToSave.Date = DateTime.Now;
        //        SessionsList.Add(SessionToSave);

        //        TransferDB.SessionsList =SessionsList;
        //        DataDB.AddUpdateSessions();
        //        foreach (ARFCN ARFCN in ListARFCNs)
        //        {
        //            ARFCN.Session_ID = SessionToSave.ID;
        //        }
        //        TransferDB.ARFCNs = ListARFCNs;
        //        DataDB.AddUpdateARFCN(); 
        //    }

        //    SessionNameToSave = null;
        //    SaveVisibility = Visibility.Collapsed;
        //}

        //private void OnNoSave()
        //{
        //    SaveVisibility = Visibility.Collapsed;
        //}

        //private void OnYesLoad()
        //{
        //    if (SelectedSession != null) 
        //    {
        //        DataDB.UpdateARFCNListBySession(SelectedSession);             
        //        ListARFCNs = TransferDB.ARFCNs;

        //        SequencingService.SetCollectionSequence(ListARFCNs);
        //        OnPropertyChanged("ListARFCNs");
        //    }

        //    LoadVisibility = Visibility.Collapsed;
        //}
        //private void OnOKWarningNS()
        //{
        //    WarningNSVisibility = Visibility.Collapsed;
        //}


        //private void OnNoLoad()
        //{
        //    LoadVisibility = Visibility.Collapsed;
        //}
        #endregion
        //private void OnDeleteAllSessions()
        //{
        //    DeleteAllSessionsVisibility = Visibility.Visible;
        //}

        //private void OnYesDeleteAllSessions()
        //{            
        //    TransferDB.SessionsList = SessionsList;
        //    DataDB.DeleteAllSessions();
        //    SessionsList.Clear();
        //    DeleteAllSessionsVisibility = Visibility.Collapsed;
        //}

        //private void OnNoDeleteAllSessions()
        //{
        //    DeleteAllSessionsVisibility = Visibility.Collapsed;
        //}

        //private void OnDeleteSelectedSession()
        //{
        //    DelSelectedSessionVisibility = Visibility.Visible;
        //}


        //private void OnNoDelSelectedSession()
        //{
        //    DelSelectedSessionVisibility = Visibility.Collapsed;
        //}

        //private void OnYesDelSelectedSession()
        //{
        //    DataDB.DeleteSelectedSession(SelectedSession);
        //    SessionsList.Remove(SelectedSession);
        //    DelSelectedSessionVisibility = Visibility.Collapsed;
        //}

        //private bool CanDeleteSelectedSession()
        // {
        //     return SelectedSession != null;
        // }


        private void OnSave()
        {
            SaveVisibility = Visibility.Visible;
        }
        private void OnCancelSave()
        {
            SaveVisibility = Visibility.Collapsed;
        }
        

        private async void OnSaveAs()
        {
            var res = await ShowMessageAsync("Save?", "Are you sure? Save current values to the DataBase", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });

            if (res == MessageDialogResult.Affirmative)
            {
                SessionToSave = new Session();
                if (ListARFCNs.Count != 0 && !string.IsNullOrEmpty(SessionNameToSave))//TODO add ListARFCNs.Count!=0 &&
                {
                    SessionToSave.ID = Guid.NewGuid();
                    SessionToSave.Name = SessionNameToSave;
                    SessionToSave.Date = DateTime.Now;
                    SessionsList.Add(SessionToSave);

                    TransferDB.SessionsList = SessionsList;
                    DataDB.AddUpdateSessions();
                    foreach (ARFCN ARFCN in ListARFCNs)
                    {
                        ARFCN.Session_ID = SessionToSave.ID;
                    }
                    TransferDB.ARFCNs = ListARFCNs;
                    DataDB.AddUpdateARFCN();
                }

                SessionNameToSave = null;
                SaveVisibility = Visibility.Collapsed;
            }
        }

        private async void OnLoad()
        {
            var res = await ShowMessageAsync("Load?", "Are you sure? Load current values to the DataBase", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });
            if (res == MessageDialogResult.Affirmative)
            {
                if (SelectedSession != null)
                {
                    DataDB.UpdateARFCNListBySession(SelectedSession);
                    ListARFCNs = TransferDB.ARFCNs;

                    SequencingService.SetCollectionSequence(ListARFCNs);
                    OnPropertyChanged("ListARFCNs");
                }

                //LoadVisibility = Visibility.Collapsed;
            }
        }

        private async void OnDeleteAllSessions()
        {
            var res = await ShowMessageAsync("Delete?", "Are you sure? Delete all sessions", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AnimateShow = false
            });
            if (res == MessageDialogResult.Affirmative)
            {
                if (SelectedSession != null)
                {
                    DataDB.UpdateARFCNListBySession(SelectedSession);
                    ListARFCNs = TransferDB.ARFCNs;

                    SequencingService.SetCollectionSequence(ListARFCNs);
                    OnPropertyChanged("ListARFCNs");
                }

            }
        }


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
            IsEnabledSession = true;
            IsScanVisible = Visibility.Visible;
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
        //private async void ShowMessageAsync(string header, string message, MessageDialogStyle dialogStyle)
        //{
        //    var settings = new MetroDialogSettings
        //    {
        //        AnimateShow = false
        //    };
        //    await dialogCoordinator.ShowMessageAsync(this, header, message, dialogStyle, settings);
        //}
        //public async static Task<MessageDialogResult> ShowMessageAsync(string title, string Message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        //{
        //    if (settings == null)
        //        settings = new MetroDialogSettings
        //        {
        //            AnimateShow = false
        //        };
        //    return await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync(title, Message, style, settings);
        //}

        #endregion


        

        public async static Task<MessageDialogResult> ShowMessageAsync(string title, string Message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            return await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync(title, Message, style, settings);
        }
     

    }
}
