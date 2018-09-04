using CPI.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.Models.Models
{
    public class UnitView
    {
   

        public UnitView()
        {
            Channel = 109;
            MCC = 425;
            MNC = 01;
            Band = "GSM900";
     
        }
        
        public int Channel { get; set; }
        public int MCC { get; set; }
        public int MNC { get; set; }
        public string Band { get; set; }
        public Double CPU1 { get; set; }
        public Double CPU2 { get; set; }
        public Double Temp1 { get; set; }
        public Double Temp2 { get; set; }





        //private int _CPU1, _CPU2;
        //private int _TEMP1, _TEMP2;
        //private int _Channel = 109;
        //private int _MCC = 425;
        //private string _MNC = "01";
        //private int _Band = 900;
        //private int _Gain;
        //private int _NOR;
        //private int _Length;


        //public int Channel
        //{
        //    get
        //    {
        //        return _Channel;
        //    }
        //    set
        //    {
        //        _Channel = value;
        //        OnPropertyChanged("Channel");
        //    }
        //}
        //public int MCC
        //{
        //    get
        //    {
        //        return _MCC;
        //    }
        //    set
        //    {
        //        _MCC = value;
        //        OnPropertyChanged("MCC");
        //    }
        //}
        //public string MNC
        //{
        //    get
        //    {
        //        return _MNC;
        //    }
        //    set
        //    {
        //        _MNC = value;
        //        OnPropertyChanged("MNC");
        //    }
        //}
        //public int Band
        //{
        //    get
        //    {
        //        return _Band;
        //    }
        //    set
        //    {
        //        _Band = value;
        //        OnPropertyChanged("Band");
        //    }
        //}

        //public int CPU1
        //{
        //    get
        //    {
        //        return _CPU1;
        //    }
        //    set
        //    {
        //        _CPU1 = value;
        //        OnPropertyChanged("CPU1");
        //    }
        //}
        //public int CPU2
        //{
        //    get
        //    {
        //        return _CPU2;
        //    }
        //    set
        //    {
        //        _CPU2 = value;
        //        OnPropertyChanged("CPU2");
        //    }
        //}
        //public int TEMP1
        //{
        //    get
        //    {
        //        return _TEMP1;
        //    }
        //    set
        //    {
        //        _TEMP1 = value;
        //        OnPropertyChanged("TEMP1");
        //    }
        //}
        //public int TEMP2
        //{
        //    get
        //    {
        //        return _TEMP2;
        //    }
        //    set
        //    {
        //        _TEMP2 = value;
        //        OnPropertyChanged("TEMP2");
        //    }
        //}



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
