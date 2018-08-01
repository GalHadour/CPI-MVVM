using System.ComponentModel;


namespace CPI.Models
{
    public class ExtensionSMS : ISequencedObject, INotifyPropertyChanged
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

        public string FileName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
