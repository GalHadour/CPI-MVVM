using CPI.ViewModels;
using System;
using System.Collections.Generic;
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
        #endregion

        #region Constructor
        #endregion

        #region Commands Methods
        #endregion
    }
}
