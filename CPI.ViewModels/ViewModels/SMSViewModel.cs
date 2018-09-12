using System;
using System.Collections.Generic;
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
        #endregion

        #region Properties
        public bool Sender
        {
            get { return _Sender; }
            set
            {
                _Sender = value;
                OnPropertyChanged("Sender");
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

        public SMSViewModel()
        {
            Sender = true;
        }
        #endregion

        #region Constructor
        #endregion

        #region Commands Methods
        #endregion

    }
}
