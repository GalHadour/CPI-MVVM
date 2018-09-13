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

        private ObservableCollection<Call> _Calls { get; set; }
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

        #endregion

        #region Constructor

        public void Initialized()
        {

            Calls = new ObservableCollection<Call>();

            Calls = TransferDB.Calls;
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
        #endregion
    }
}
