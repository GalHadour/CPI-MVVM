using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.ViewModels.ViewModels
{
   public class SMSViewModel
    {
        private bool _Sender;
        private bool _Band;
        private bool _ARFCN;
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
