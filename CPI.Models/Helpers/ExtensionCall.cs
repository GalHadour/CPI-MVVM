using System.ComponentModel;

namespace CPI.Models
{
    public class ExtensionCall : ISequencedObject, INotifyPropertyChanged
    {
        private int _sequenceNumber;
        

        public int SequenceNumber
        {
            get { return _sequenceNumber; }
            set
            {
                _sequenceNumber = value;
                OnPropertyChanged("SequenceNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        //public string FileName { get; set; }
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
