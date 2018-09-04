using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.Models.Helpers
{
    public class ExtensionReceiver
    {
        private bool _IsAttached;
     

        public bool IsAttached
        {
            get
            {
                return _IsAttached;
            }
            set
            {
                _IsAttached = value;
                OnPropertyChanged("IsAttached");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
      
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    } 
}
