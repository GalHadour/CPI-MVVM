using CPI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.Models
{


    public class ListenerMessage
    {
        public Type MessageType { get; set; }
        public RecordFile RecFile { get; set; }
        public Call CallFile { get; set; }
        public SMS SmsFile { get; set; }

        public string Host { get; set; }
        public string Rx { get; set; }
        public string Note { get; set; }
    }
}
