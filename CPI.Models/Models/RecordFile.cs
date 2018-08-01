using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.Models
{
    public class RecordFile : ISequencedObject, INotifyPropertyChanged
    {
        private int _sequenceNumber;
        private string _FileName;
        private string _Size;

        public int SequenceNumber
        {
            get { return _sequenceNumber; }
            set
            {
                _sequenceNumber = value;
                OnPropertyChanged("SequenceNumber");
            }
        }

        public string Unit { get; set; }
        public string ID { get; set; }
        public string CPU { get; set; }
        public string Rx { get; set; }
        public string Band { get; set; }
        public string Frequency { get; set; }
        public string SampleRate { get; set; }
        public string ARFCN { get; set; }
        public string Count { get; set; }
        public DateTime Created { get; set; }
        public string Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                OnPropertyChanged("Size");
            }
        }
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                try
                {
                    string[] chunks = _FileName.Split(new string[] { "_", "." }, StringSplitOptions.None);
                    Count = chunks[1];
                    ID = chunks[0];
                    Band = chunks[2];
                    ARFCN = chunks[3];
                    Frequency = chunks[4];
                    SampleRate = chunks[5];
                }
                catch (Exception)
                {
#if (DEBUG)
                    throw;
#endif
                }

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
