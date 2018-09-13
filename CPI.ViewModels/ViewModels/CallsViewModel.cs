using CPI.DTO;
using CPI.Models.Entity;
using CPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPIViewModels
{
   public class CallsViewModel: ViewModelBase
    {
        #region Fields
        private bool _Caller;
        private bool _Band;
        private bool _ARFCN;
        private string _SearchWord;
    
        private ObservableCollection<String> _SearchHistory { get; set; }
        private ObservableCollection<Call> _Calls { get; set; }

        //private ObservableCollection<ARFCN> _ARFCNs { get; set; }
        #endregion

        #region Properties
        public bool Caller
        {
            get { return _Caller; }
            set
            {
                _Caller = value;
                OnPropertyChanged("Caller");
            }
        }


        public bool ARFCN
        {
            get { return _ARFCN; }
            set
            {
                _ARFCN = value;
                OnPropertyChanged("ARFCN");
            }
        }

        public bool Band
        {
            get { return _Band; }
            set
            {
                _Band = value;
                OnPropertyChanged("Band");
            }
        }

        public CallsViewModel()
        {
            Caller = true;
        }

        public ObservableCollection<Call> Calls
        {
            get
            {
                return _Calls;
            }
            set
            {
                _Calls = value;
                OnPropertyChanged("Calls");
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

                if (Caller)
                {
                    foreach (Call sms in _Calls)
                    {
                        if (!String.IsNullOrEmpty(sms.CallerNumber))
                            if (!_SearchHistory.Contains(sms.CallerNumber))
                                _SearchHistory.Add(sms.CallerNumber);
                    }
                }
                else if (ARFCN)
                {
                    foreach (Call sms in _Calls)
                    {
                        if (sms.ARFCN != null)
                            if (!String.IsNullOrEmpty(sms.ARFCN.Chanel.ToString()) && !_SearchHistory.Contains(sms.ARFCN.Chanel.ToString()))
                                _SearchHistory.Add(sms.ARFCN.Chanel.ToString());
                    }
                }
                else if (Band)
                {
                    foreach (Call sms in _Calls)
                    {
                        if (sms.ARFCN != null)
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

        public void Initialized()
        {

            Search = new RelayCommand(OnSearch);
            ResetSearch = new RelayCommand(OnResetSearch);
            Calls = new ObservableCollection<Call>();

            //_ARFCNs = TransferDB.ARFCNs;
            //foreach(ARFCN arfcn in _ARFCNs)          
            //{
            //    if(arfcn!=null)
            //    {
            //        Calls.Union(arfcn.Call);
            //    }
            //}

            Calls= TransferDB.Calls;
            SequencingService.SetCollectionSequence(Calls);


            //_Calls.CollectionChanged += _Calls_CollectionChanged;
#if (DEBUG)
#else
#endif
        }
        #endregion

        #region Commands Methods

        //private void _Calls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    SequencingService.SetCollectionSequence(Calls);
        //}



        public void OnSearch()//TODO check ARFCN and BAND
        {

            if (SearchWord != null)
            {
                if (Caller)
                {
                    _Calls = new ObservableCollection<Call>(_Calls.Where(s => s.CallerNumber == SearchWord));
                    //_Calls.GroupBy(x => x.CallerNumber).Select(x => x.First());
                }
                else if (ARFCN)
                {
                    _Calls = new ObservableCollection<Call>(_Calls.Where(s => s.ARFCN.Chanel.ToString() == SearchWord));
                    //_Calls.GroupBy(x =>x.ARFCN.Chanel.ToString().Select(x => x.First());
                }
                else if (Band)
                {
                    _Calls = new ObservableCollection<Call>(_Calls.Where(s => s.ARFCN != null && s.ARFCN.Band == SearchWord));
                    //_Calls.GroupBy(x => x.ARFCN.Band).Select(x => x.First());
                }
                else
                {
                    _Calls = null;
                }

                OnPropertyChanged("Calls");
            }

        }

        public void OnResetSearch()
        {
            SearchWord = null;
            //foreach (ARFCN arfcn in _ARFCNs)
            //{
            //    if (arfcn != null)
            //    {
            //        Calls.Union(arfcn.Call);
            //    }
            //}

            Calls = TransferDB.Calls;
            SequencingService.SetCollectionSequence(Calls);
        }
        #endregion
    }
}
