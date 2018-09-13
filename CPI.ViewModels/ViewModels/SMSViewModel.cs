using CPI.DTO;
using CPI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.ViewModels
{
   public class SMSViewModel :ViewModelBase
    {
        #region Fields
        private bool _Sender;
        private bool _Band;
        private bool _ARFCN;
        private string _SearchWord;
       private ObservableCollection<SMS> _SMSs { get; set; }
        private ObservableCollection<String> _SearchHistory { get; set; }
       
        #endregion

        #region Properties
        public bool Sender
        {
            get { return _Sender; }
            set
            {
                _Sender = value;
                OnPropertyChanged("Sender");
                OnPropertyChanged("SearchHistory");
            }
        }


        public bool ARFCN
        {
            get { return _ARFCN; }
            set
            {
                _ARFCN = value;
                OnPropertyChanged("ARFCN");
                OnPropertyChanged("SearchHistory");
            }
        }

        public bool Band
        {
            get { return _Band; }
            set
            {
                _Band = value;
                OnPropertyChanged("Band");
                OnPropertyChanged("SearchHistory");
            }
        }

        public SMSViewModel()
        {
            Sender = true;
        }

        public ObservableCollection<SMS> SMSs
        {
            get
            {
                return _SMSs;
            }
            set
            {
                _SMSs = value;
                OnPropertyChanged("SMSs");
            }
        }

        public ObservableCollection<String> SearchHistory
        {
            get
            {
                _SearchHistory = new ObservableCollection<string>();
              
                    if (Sender)
                    {
                        foreach(SMS sms in _SMSs)
                        {
                            _SearchHistory.Add(sms.Sender);
                        }
                    }
                    else if (ARFCN)
                    {
                    foreach (SMS sms in _SMSs)
                    {
                        _SearchHistory.Add(sms.Kc);
                    }
                }
                    else if (Band)
                    {
                    foreach (SMS sms in _SMSs)
                    {
                        _SearchHistory.Add(sms.Length);
                    }
                }
                    else
                    {
                    _SearchHistory = null;
                    }

                    return _SearchHistory;
                
            }
            set
            {
                _SearchHistory = value;
                OnPropertyChanged("SearchHistory");
            }
        }


        public RelayCommand Search { get; set; }

        public string SearchWord
        {
            get { return _SearchWord; }
            set
            {
                _SearchWord = value;
                OnPropertyChanged("SearchWord");
            }
        }

        #endregion

        #region Constructor
        public void Initialized()//TODO change to ctor
        {
            Search = new RelayCommand(OnSearch);
            SMSs = new ObservableCollection<SMS>();
            
             SMSs = TransferDB.SMSs;
            SequencingService.SetCollectionSequence(SMSs);
            //_SMSs.CollectionChanged += _SMSs_CollectionChanged;


#if (DEBUG)
#else
#endif
        }
        #endregion

        #region Commands Methods

        //private void _SMSs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    SequencingService.SetCollectionSequence(SMSs);
        //}


        public void OnSearch()
        {
           
            if (SearchWord != null)
            {
                if (Sender)
                {
                    _SMSs = new ObservableCollection<SMS>(_SMSs.Where(s => s.Sender == SearchWord));//TODO CHECK
                }
                else if (ARFCN)
                {
                    _SMSs = new ObservableCollection<SMS>(_SMSs.Where(s => s.Kc == SearchWord));
                }
                else if (Band)
                {
                    _SMSs = new ObservableCollection<SMS>(_SMSs.Where(s => s.Length == SearchWord));
                }
                else
                {
                    _SMSs = null;
                }

                OnPropertyChanged("SMSs");
            }

        }
        #endregion

    }
}
