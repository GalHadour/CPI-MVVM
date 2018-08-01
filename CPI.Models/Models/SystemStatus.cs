using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI.Models
{
    public class SystemStatus
    {
        public Type MessageType { get; set; }
        public string HostName { get; set; }
        public State Status { get; set; }
        public int Count { get; set; }
    }
}
