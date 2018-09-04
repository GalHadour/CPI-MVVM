using System.ComponentModel;

namespace CPI.Models
{
    public class ExtensionARFCN : ISequencedObject, INotifyPropertyChanged
    {
        private int _sequenceNumber;
        private string _RxName;

        public int SequenceNumber
        {
            get { return _sequenceNumber; }
            set
            {
                _sequenceNumber = value;
                OnPropertyChanged("SequenceNumber");
            }
        }

        public string RxName
        {
            get { return _RxName; }
            set
            {
                _RxName = value;
                OnPropertyChanged("RxName");
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
