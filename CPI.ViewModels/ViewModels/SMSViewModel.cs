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

        //private ObservableCollection<ARFCN> _ARFCNs { get; set; }

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

        //public ObservableCollection<ARFCN> ARFCNs
        //{
        //    get
        //    {
        //        return _ARFCNs;
        //    }
        //    set
        //    {
        //        _ARFCNs = value;
        //        OnPropertyChanged("ARFCNs");
        //    }
        //}

        public ObservableCollection<String> SearchHistory//TODO check if the list in combobox is ok
        {
            get
            {
                _SearchHistory = new ObservableCollection<string>();
              
                    if (Sender)
                    {
                    foreach (SMS sms in _SMSs)
                    {
                        if (!String.IsNullOrEmpty(sms.Sender))
                            if (!_SearchHistory.Contains(sms.Sender))
                                 _SearchHistory.Add(sms.Sender);
                    }
                    }
                    else if (ARFCN)
                    {
                    foreach (SMS sms in _SMSs)
                    {
                        if(sms.ARFCN!=null)
                            if (!String.IsNullOrEmpty(sms.ARFCN.Chanel.ToString()) && !_SearchHistory.Contains(sms.ARFCN.Chanel.ToString()))
                                  _SearchHistory.Add(sms.ARFCN.Chanel.ToString());
                    }
                }
                    else if (Band)
                    {
                    foreach (SMS sms in _SMSs)
                    {
                        if (sms.ARFCN!=null)
                            if (!String.IsNullOrEmpty(sms.ARFCN.Band) && !_SearchHistory.Contains(sms.ARFCN.Band))
                                   _SearchHistory.Add(sms.ARFCN.Band);
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
        public RelayCommand ResetSearch { get; set; }
        

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
        public void Initialized()//TODO change to ctor  and check where to take sms from arfcns or SMS
        {
            Search = new RelayCommand(OnSearch);
            ResetSearch = new RelayCommand(OnResetSearch);
            SMSs = new ObservableCollection<SMS>();

            //_ARFCNs = TransferDB.ARFCNs;
            //foreach(ARFCN arfcn in _ARFCNs)          
            //{
            //    if(arfcn!=null)
            //    {
            //        SMSs.Union(arfcn.SMS);
            //    }
            //}

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


        public void OnSearch()//TODO check ARFCN and BAND
        {
           
            if (SearchWord != null)
            {
                if (Sender)
                {
                    _SMSs = new ObservableCollection<SMS>(_SMSs.Where(s => s.Sender == SearchWord));
                    //_SMSs.GroupBy(x => x.Sender).Select(x => x.First());
                }
                else if (ARFCN)
                {
                    _SMSs = new ObservableCollection<SMS>(_SMSs.Where(s => s.ARFCN.Chanel.ToString() == SearchWord));
                    //_SMSs.GroupBy(x =>x.ARFCN.Chanel.ToString().Select(x => x.First());
                }
                else if (Band)
                {
                    _SMSs = new ObservableCollection<SMS>(_SMSs.Where(s =>s.ARFCN!=null && s.ARFCN.Band == SearchWord));
                    //_SMSs.GroupBy(x => x.ARFCN.Band).Select(x => x.First());
                }
                else
                {
                    _SMSs = null;
                }

                OnPropertyChanged("SMSs");
            }

        }

        public void OnResetSearch()
        {
            SearchWord = null;
            //foreach (ARFCN arfcn in _ARFCNs)
            //{
            //    if (arfcn != null)
            //    {
            //        SMSs.Union(arfcn.SMS);
            //    }
            //}

            SMSs = TransferDB.SMSs;
            SequencingService.SetCollectionSequence(SMSs);
        }
        #endregion

    }
}
