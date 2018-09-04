using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.Models.ItemsInfo
{
     public class CallsInfo
    {
        private int _count=0;
        private string _ItemName = "PHONE CALLS";//GAL

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ItemName
        {
            get { return _ItemName; }//GAL
            set
            {
                _ItemName = value;
                OnPropertyChanged("ItemName");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

