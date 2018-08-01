using CPI.DTO;
using CPI.Models;
using CPI.Models.Entity;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vlc.DotNet.Wpf;
using System.Timers;
using System.Windows.Media;

namespace CPI.ViewModels
{
    public class RecordsViewModel : ViewModelBase
    {

        #region Create Settings

        readonly string BroadcastIP = "255.255.255.255";
        readonly int BroadcastPort = 13150;
        readonly string CallsFolder = @"\CPI 1890\Calls";
        readonly string SmsFolder = @"\CPI 1890\SMS";
        readonly string path = "/home/mainuser";

        #endregion

        #region Fields
        private IDialogCoordinator dialogCoordinator;
        private int _NOR = 5;
        private int _RDS = 60;
        private bool _Loop = false;
        private RecordFile _SelectedRecordFile;
        private ObservableCollection<RecordFile> _RecordFiles;
        private ObservableCollection<Call> _CallFiles;
        private ObservableCollection<SMS> _SmsFiles;
        private ObservableCollection<Unit> Units;
        private ObservableCollection<Computer> computers;
        private BackgroundWorker listenerWorker;
        private UdpClient listener;
        private int _Gain = 40;
        private Call _SelectedCall;
        private VlcControl control;
        private string localCallsFolder;
        private string localSmsFolder;
        private bool _DFAD = false;
        private SMS _SelectedSMS;
        private Visibility _SMSDataVisibility = Visibility.Collapsed;
        private bool _DFAL = false;
        private Visibility _StopRecVisibility = Visibility.Collapsed;
        private Visibility _StopInterceptorVisibility = Visibility.Collapsed;





        #endregion

        #region Properties

        public int NOR
        {
            get { return _NOR; }
            set
            {
                _NOR = value;
                OnPropertyChanged("NOR");
            }
        }
        public int RDS
        {
            get { return _RDS; }
            set
            {
                _RDS = value;
                OnPropertyChanged("RDS");
            }
        }
        public bool Loop
        {
            get { return _Loop; }
            set
            {
                _Loop = value;
                OnPropertyChanged("Loop");
            }
        }

        /// <summary>
        /// Delete cfile after decoding
        /// </summary>
        public bool DFAD
        {
            get { return _DFAD; }
            set
            {
                _DFAD = value;
                OnPropertyChanged("DFAD");
            }
        }
        public bool DFAL
        {
            get { return _DFAL; }
            set
            {
                _DFAL = value;
                OnPropertyChanged("DFAL");
            }
        }
        public Visibility SMSDataVisibility
        {
            get { return _SMSDataVisibility; }
            set
            {
                _SMSDataVisibility = value;
                OnPropertyChanged("SMSDataVisibility");
            }
        }

        public Visibility StopRecVisibility
        {
            get { return _StopRecVisibility; }
            set
            {
                _StopRecVisibility = value;
                OnPropertyChanged("StopRecVisibility");
            }
        }


        public Visibility StopInterceptorVisibility
        {
            get { return _StopInterceptorVisibility; }
            set
            {
                _StopInterceptorVisibility = value;
                OnPropertyChanged("StopInterceptorVisibility");
            }
        }
        public RecordFile SelectedRecordFile
        {
            get { return _SelectedRecordFile; }
            set
            {
                _SelectedRecordFile = value;
                Run.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedRecordFile");
            }
        }
        public Call SelectedCall
        {
            get { return _SelectedCall; }
            set
            {
                _SelectedCall = value;
                Play.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedCall");
            }
        }
        public SMS SelectedSMS
        {
            get { return _SelectedSMS; }
            set
            {
                _SelectedSMS = value;
                OnPropertyChanged("SelectedSMS");
            }
        }
        public ObservableCollection<RecordFile> RecordFiles
        {
            get { return _RecordFiles; }
            set
            {
                _RecordFiles = value;
                OnPropertyChanged("RecordFiles");
            }
        }
        public ObservableCollection<Call> CallFiles
        {
            get { return _CallFiles; }
            set
            {
                _CallFiles = value;
                OnPropertyChanged("CallFiles");
            }
        }
        public RelayCommand Loaded { get; set; }
        public RelayCommand Unloaded { get; set; }
        public RelayCommand StartRec { get; set; }
        public RelayCommand StopRec { get; set; }
        public RelayCommand Run { get; set; }
        public RelayCommand Play { get; set; }
        public RelayCommand Reload { get; set; }
        public RelayCommand Stop { get; set; }
        public RelayCommand Close { get; set; }
        public RelayCommand ShowSMSData { get;  set; }
        public RelayCommand StopInterceptor { get; set; }
        public RelayCommand DeleteTest { get;  set; }

        public int Gain
        {
            get { return _Gain; }
            set
            {
                _Gain = value;
                OnPropertyChanged("Gain");
            }
        }

        public ObservableCollection<SMS> SmsFiles
        {
            get { return _SmsFiles; }
            set
            {
                _SmsFiles = value;
                OnPropertyChanged("SmsFiles");
            }
        }


        #endregion

        #region Constructor
        public RecordsViewModel(IDialogCoordinator instance)
        {
            _RecordFiles = new ObservableCollection<RecordFile>();
            _CallFiles = new ObservableCollection<Call>();
            _SmsFiles = new ObservableCollection<SMS>();
            _RecordFiles.CollectionChanged += _RecordFiles_CollectionChanged;
            _CallFiles.CollectionChanged += _CallFiles_CollectionChanged;
            _SmsFiles.CollectionChanged += _SmsFiles_CollectionChanged;

            dialogCoordinator = instance;

            Loaded = new RelayCommand(OnLoaded);
            Unloaded = new RelayCommand(OnUnloaded);
            StartRec = new RelayCommand(OnStartRec, CanStartStopRec);
            StopRec = new RelayCommand(OnStopRec, CanStartStopRec);
            Run = new RelayCommand(OnRun, CanRun);
            Play = new RelayCommand(OnPlay, CanPlay);
            Reload = new RelayCommand(OnReload);
            Stop = new RelayCommand(OnStop);
            Close = new RelayCommand(OnClose);
            ShowSMSData = new RelayCommand(OnShowSMSData);
            StopInterceptor = new RelayCommand(OnStopInterceptor);

            DeleteTest = new RelayCommand(OnDeleteTest);


            listenerWorker = new BackgroundWorker();
            listenerWorker.DoWork += ListenerWorker_DoWork;
            listenerWorker.ProgressChanged += ListenerWorker_ProgressChanged;
            listenerWorker.RunWorkerCompleted += ListenerWorker_RunWorkerCompleted;
            listenerWorker.WorkerReportsProgress = true;
            listenerWorker.WorkerSupportsCancellation = true;

        }

        private void OnDeleteTest()
        {
            RecordFiles.Remove(SelectedRecordFile);
        }

        public void Initialized()
        {

            InitMediaPlayer();
            localCallsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + CallsFolder;
            localSmsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + SmsFolder;
            if (!Directory.Exists(localSmsFolder))
                Directory.CreateDirectory(localSmsFolder);
            if (!Directory.Exists(localCallsFolder))
                Directory.CreateDirectory(localCallsFolder);

            Units = TransferDB.Units;
            computers = TransferDB.Computers;


#if (DEBUG)
#else
#endif
        }

        #endregion

        #region Commands Methods

        private void OnStopInterceptor()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            if (Interceptor.Stop(cpu, 1))
                StopInterceptorVisibility = Visibility.Collapsed;
        }

        private void OnShowSMSData()
        {
            SMSDataVisibility = Visibility.Visible;
        }
        private void OnClose()
        {
            SMSDataVisibility = Visibility.Collapsed;
        }
        private void OnReload()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Units.First().ID);
            Service.Reload(cpu, BroadcastIP, BroadcastPort, path);
            RecordFiles.Clear();
            SmsFiles.Clear();
            CallFiles.Clear();

        }
        private void OnRun()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Units.First().ID);
            ARFCN arfcn = Transfer.SelectedARFCN;
            if (Interceptor.Start(cpu, SelectedRecordFile, InterceptorType.Jupiter, DFAD, BroadcastIP, BroadcastPort, path))
                StopInterceptorVisibility = Visibility.Visible;
        }
        private void OnPlay()
        {
            control.SourceProvider.MediaPlayer.Play(new Uri(String.Format(@"{0}\{1}", localCallsFolder, SelectedCall.FileName)));
        }
        private void OnStop()
        {
            control.SourceProvider.MediaPlayer.Stop();
        }
        private void InitMediaPlayer()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            DirectoryInfo vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            control = new VlcControl();
            control.SourceProvider.CreatePlayer(vlcLibDirectory);
        }
        private bool CanPlay()
        {
            return SelectedCall != null;
        }
        private bool CanRun()
        {
            return SelectedRecordFile != null;
        }
        private void OnStopRec()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            if (Recorder.Stop(cpu, 1))
                StopRecVisibility = Visibility.Collapsed;
        }
        private bool CanStartStopRec()
        {
            return Transfer.SelectedARFCN != null && Transfer.SelectedUnit != null;
        }
        private void OnStartRec()
        {
            Computer cpu = computers.FirstOrDefault(c => c.Unit_ID == Transfer.SelectedUnit.ID);
            ARFCN arfcn = Transfer.SelectedARFCN;
            int mood = Loop ? 1 : 0;
            if (Recorder.Start(cpu, 1, arfcn.Band, arfcn.Chanel.Value, Convert.ToDouble(arfcn.Frequency) * 1000000, Gain, 400000, RDS, NOR, mood, path))
                StopRecVisibility = Visibility.Visible;
        }
        private void OnLoaded()
        {
            StartRec.RaiseCanExecuteChanged();
            StopRec.RaiseCanExecuteChanged();
            if (!listenerWorker.IsBusy)
                listenerWorker.RunWorkerAsync(BroadcastPort);
        }
        private void OnUnloaded()
        {
            if (listenerWorker.IsBusy)
            {
                listener.Close();
                listenerWorker.CancelAsync();

            }
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
                    break;
                }
                else
                {
                    byte[] byteSource = listener.Receive(ref endPoint);
                    string data = encoder.GetString(byteSource);

                    string[] chunks = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if (chunks.Length > 0)
                    {
                        switch (chunks[0])
                        {
                            case "RECORDS":
                                {
                                    RecordFile file;
                                    for (int i = 3; i < chunks.Count(); i++)
                                    {
                                        string[] subchunks = chunks[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                        file = new RecordFile
                                        {
                                            CPU = chunks[1],
                                            Rx = chunks[2],
                                            Size = subchunks[0],
                                            Created = DateTime.Parse(String.Format("{0} {1}", subchunks[1], subchunks[2])),
                                            FileName = subchunks[3]
                                        };

                                        worker.ReportProgress(0, new ListenerMessage
                                        {
                                            MessageType = Models.Type.Records,
                                            RecFile = file
                                        });
                                    }
                                }
                                break;
                            case "CALLS":
                                {
                                    Call newCall;
                                    for (int i = 2; i < chunks.Count(); i++)
                                    {
                                        string[] subchunks = chunks[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] filechunks = subchunks[3].Split(new string[] { "_", "." }, StringSplitOptions.None);

                                        newCall = new Call
                                        {
                                            UnitName = chunks[1],
                                            Size = subchunks[0],
                                            Created = DateTime.Parse(String.Format("{0} {1}", subchunks[1], subchunks[2])),
                                            FileName = subchunks[3],
                                            TS = filechunks[3],
                                            Rate = filechunks[4],
                                            Codec = filechunks[5],
                                            CallerNumber = filechunks[6],
                                            Kc = filechunks[7].ToUpper(),
                                        };

                                        worker.ReportProgress(0, new ListenerMessage
                                        {
                                            MessageType = Models.Type.CALL,
                                            CallFile = newCall
                                        });
                                    }
                                }
                                break;
                            case "SMS":
                                {
                                    SMS newSms;
                                    for (int i = 2; i < chunks.Count(); i++)
                                    {
                                        string[] subchunks = chunks[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] filechunks = subchunks[3].Split(new string[] { "_", "." }, StringSplitOptions.None);

                                        newSms = new SMS
                                        {
                                            UnitName = chunks[1],
                                            FileName = subchunks[3],
                                            TS = filechunks[3],
                                            Rate = filechunks[4],
                                            Kc = filechunks[6].ToUpper(),
                                        };

                                        worker.ReportProgress(0, new ListenerMessage
                                        {
                                            MessageType = Models.Type.SMS,
                                            SmsFile = newSms
                                        });
                                    }
                                }
                                break;
                            case "DELETE":
                                {
                                    worker.ReportProgress(0, new ListenerMessage
                                    {
                                        MessageType = Models.Type.Delete,
                                        RecFile = new RecordFile
                                        {
                                            FileName = chunks[1]
                                        }
                                    });
                                }
                                break;
                            case "INT":
                                {
                                    worker.ReportProgress(0, new ListenerMessage
                                    {
                                        MessageType = Models.Type.Interceptor,
                                        Host = chunks[1],
                                        Rx = chunks[2],
                                        Note = chunks[3],
                                    });
                                }
                                break;
                            case "REC":
                                {
                                    worker.ReportProgress(0, new ListenerMessage
                                    {
                                        MessageType = Models.Type.Recorder,
                                        Host = chunks[1],
                                        Rx = chunks[2],
                                        Note = chunks[3],
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    
                }
            }
        }
        private void ListenerWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ListenerMessage message = (ListenerMessage)e.UserState;
            switch (message.MessageType)
            {
                case Models.Type.Records:
                    {
                        RecordFile rf = RecordFiles.FirstOrDefault(f => f.FileName == message.RecFile.FileName);
                        if (rf == null)
                            RecordFiles.Add(message.RecFile);
                        else if (rf.Size != message.RecFile.Size)
                            rf.Size = message.RecFile.Size;
                    }
                    break;
                case Models.Type.CALL:
                    {
                        Call call = CallFiles.FirstOrDefault(f => f.FileName == message.CallFile.FileName);
                        Computer cpu = computers.FirstOrDefault(c => c.Name == message.CallFile.UnitName);

                        if (call == null)
                        {
                            if (DownloadFile(cpu, message.CallFile.FileName, Models.Type.CALL))
                                CallFiles.Add(message.CallFile);
                        }
                        else if (call.Size != message.CallFile.Size)
                        {
                            call.Size = message.CallFile.Size;
                            if (DownloadFile(cpu, message.CallFile.FileName, Models.Type.CALL))
                                CallFiles.Add(message.CallFile);
                        }
                        else if(DFAL)
                           Service.DeleteFile(cpu, message.CallFile.FileName, Models.Type.CALL);
                    }

                    break;
                case Models.Type.SMS:
                    {
                        SMS sms = SmsFiles.FirstOrDefault(f => f.FileName == message.SmsFile.FileName);
                        Computer cpu = computers.FirstOrDefault(c => c.Name == message.SmsFile.UnitName);

                        if (sms == null && DownloadFile(cpu, message.SmsFile.FileName, Models.Type.SMS) && SplitSmsData(message.SmsFile) && DFAL)
                            Service.DeleteFile(cpu, message.SmsFile.FileName, Models.Type.SMS);

                    }
                    break;
                case Models.Type.Delete:
                    {
                        RecordFile rf = RecordFiles.FirstOrDefault(f => f.FileName == message.RecFile.FileName);
                        if (rf != null)
                            RecordFiles.Remove(rf);
                    }
                    break;
                case Models.Type.Recorder:
                    {
                        StopRecVisibility = Visibility.Collapsed;
                    }
                    break;
                case Models.Type.Interceptor:
                    {
                        StopInterceptorVisibility = Visibility.Collapsed;
                    }
                    break;
                default:
                    break;
            }

        }
        private void ListenerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                    listenerWorker.RunWorkerAsync(BroadcastPort);
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

        #region General Methods

        private bool SplitSmsData(SMS sms)
        {
            try
            {
                string fileName = String.Format(@"{0}\{1}", localSmsFolder, sms.FileName);

                var lines = File.ReadAllLines(fileName);
                if (lines.Count() > 12)
                {
                    sms.SRI = lines[0];
                    sms.UDHI = lines[1];
                    sms.MMS = lines[2];

                    sms.SMSC = lines[3];
                    sms.Sender = lines[4];
                    sms.TimeStamp = DateTime.ParseExact(lines[5].Remove(lines[5].Length - 6), "dd/MM/yy HH:mm:ss", null);

                    sms.PID = lines[6];
                    sms.DCS = lines[7];

                    sms.TextType = lines[8];
                    sms.Class = lines[9];

                    sms.Alphabet = lines[10];
                    sms.Message = lines[11];
                    sms.Length = lines[12];

                    sms.ID = Guid.NewGuid();

                    SmsFiles.Add(sms);
                    DatabaseService.AddOrUpdate<SMS>(sms);
                    File.Delete(fileName);
                    return true;
                }
                else
                    return false;
                
            }
            catch (Exception)
            {
#if(DEBUG)
                throw;
#else
                return false;
#endif

            }
        }
        private bool DownloadFile(Computer cpu, string fileName, Models.Type type)
        {
            string localFile = String.Empty;
            string remoteFile = String.Empty;
            switch (type)
            {
                case Models.Type.CALL:
                    {
                        
                        localFile = String.Format(@"{0}\{1}", localCallsFolder, fileName);
                        remoteFile = String.Format("{0}/calls/{1}", path, fileName);
                    }
                    break;
                case Models.Type.SMS:
                    {

                        localFile = String.Format(@"{0}\{1}", localSmsFolder, fileName);
                        remoteFile = String.Format("{0}/sms/{1}", path, fileName);
                    }
                    break;
            }

            
            
            return Service.DownloadFile(cpu, localFile, remoteFile);
        }
        private void _CallFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SequencingService.SetCollectionSequence(CallFiles);
        }
        private void _SmsFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SequencingService.SetCollectionSequence(SmsFiles);
        }
        private void _RecordFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SequencingService.SetCollectionSequence(RecordFiles);
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
