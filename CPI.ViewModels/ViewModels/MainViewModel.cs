
using CPI.Models;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;

namespace CPI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Create Settings
        readonly private int statusListenerPort = 13155;

        #endregion

        #region Fields
        private Brush _ScannColor;
        private Brush _RecColor;
        private Brush _IntColor;
        private Brush _CallColor;
        private Brush _SmsColor;

        private BackgroundWorker statusListener;
        private bool _IsInterceptorSpin = false;
        private bool _IsScannerSpin = false;
        #endregion

        #region Properties
        public Brush ScannColor
        {
            get { return _ScannColor; }
            set
            {
                _ScannColor = value;
                OnPropertyChanged("ScannColor");
            }
        }
        public Brush RecColor
        {
            get { return _RecColor; }
            set
            {
                _RecColor = value;
                OnPropertyChanged("RecColor");
            }
        }
        public Brush IntColor
        {
            get { return _IntColor; }
            set
            {
                _IntColor = value;
                OnPropertyChanged("IntColor");
            }
        }
        public Brush CallColor
        {
            get { return _CallColor; }
            set
            {
                _CallColor = value;
                OnPropertyChanged("CallColor");
            }
        }
        public Brush SmsColor
        {
            get { return _SmsColor; }
            set
            {
                _SmsColor = value;
                OnPropertyChanged("SmsColor");
            }
        }
        public RelayCommand Loaded { get; set; }

        public bool IsInterceptorSpin
        {
            get { return _IsInterceptorSpin; }
            set
            {
                _IsInterceptorSpin = value;
                OnPropertyChanged("IsInterceptorSpin");
            }
        }
        public bool IsScannerSpin
        {
            get { return _IsScannerSpin; }
            set
            {
                _IsScannerSpin = value;
                OnPropertyChanged("IsScannerSpin");
            }
        }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            Loaded = new RelayCommand(OnLoad);

            statusListener = new BackgroundWorker();
            statusListener.DoWork += StatusListener_DoWork;
            statusListener.ProgressChanged += StatusListener_ProgressChanged;
            statusListener.RunWorkerCompleted += StatusListener_RunWorkerCompleted;
            statusListener.WorkerReportsProgress = true;
            statusListener.WorkerSupportsCancellation = true;
        }
        #endregion

        #region Status Listener
        private void StatusListener_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int port = (int)e.Argument;

            UdpClient listener = new UdpClient(port);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

            ASCIIEncoding encoder = new ASCIIEncoding();

            while (true)
            {
                byte[] byteSource = listener.Receive(ref endPoint);
                string data = encoder.GetString(byteSource);

                string[] chunks = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (chunks.Length > 0)
                {
                    switch (chunks[0])
                    {
                        case "CALL":
                            {
                                worker.ReportProgress(0, new SystemStatus
                                {
                                    MessageType = Models.Type.CALL,
                                    HostName = chunks[1],
                                    Count = Int32.Parse(chunks[2]),
                                });
                            }
                            break;
                        case "SMS":
                            {
                                worker.ReportProgress(0, new SystemStatus
                                {
                                    MessageType = Models.Type.SMS,
                                    HostName = chunks[1],
                                    Count = Int32.Parse(chunks[2]),
                                });
                            }
                            break;
                        case "RC":
                            {
                                worker.ReportProgress(0, new SystemStatus
                                {
                                    MessageType = Models.Type.Recorder,
                                    HostName = chunks[1],
                                    Status = chunks[2] == "RC" ? State.On : State.Off,

                                });
                            }
                            break;
                        case "IN":
                            {
                                worker.ReportProgress(0, new SystemStatus
                                {
                                    MessageType = Models.Type.Interceptor,
                                    HostName = chunks[1],
                                    Status = chunks[2] == "IN" ? State.On : State.Off,
                                });
                            }
                            break;
                        case "SC":
                            {
                                worker.ReportProgress(0, new SystemStatus
                                {
                                    MessageType = Models.Type.Scanner,
                                    HostName = chunks[1],
                                    Status = chunks[2] == "SC" ? State.On : State.Off,

                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void StatusListener_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SystemStatus message = (SystemStatus)e.UserState;
            switch (message.MessageType)
            {
                case Models.Type.CALL:
                    CallColor = message.Count > 0 ? ColorsPalette.Red : ColorsPalette.Blue;
                    break;
                case Models.Type.SMS:
                    SmsColor = message.Count > 0 ? ColorsPalette.Red : ColorsPalette.Blue;
                    break;
                case Models.Type.Recorder:
                    RecColor = message.Status == State.On ? ColorsPalette.Red : ColorsPalette.Blue;
                    break;
                case Models.Type.Interceptor:
                    if (message.Status == State.On)
                    {
                        IntColor = ColorsPalette.Red;
                        IsInterceptorSpin = true;
                    }
                    else
                    {
                        IntColor = ColorsPalette.Blue;
                        IsInterceptorSpin = false;
                    }
                    
                    break;
                case Models.Type.Scanner:
                    if (message.Status == State.On)
                    {
                        ScannColor = ColorsPalette.Red;
                        IsScannerSpin = true;
                    }
                    else
                    {
                        ScannColor = ColorsPalette.Blue;
                        IsScannerSpin = false;
                    }
                    break;
                default:
                    break;
            }
        }

        private void StatusListener_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                statusListener.RunWorkerAsync(statusListenerPort);
            else if (e.Cancelled)
            {

            }
            else
            {

            }
        }

        #endregion

        #region Commands
        private void OnLoad()
        {
            ScannColor = ColorsPalette.Blue;
            RecColor = ColorsPalette.Blue;
            IntColor = ColorsPalette.Blue;
            CallColor = ColorsPalette.Blue;
            SmsColor = ColorsPalette.Blue;


            statusListener.RunWorkerAsync(statusListenerPort);
        }
        #endregion
    }
}
