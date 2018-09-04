namespace CPI.Models
{
    public class Status
    {
        public string Host { get; set; }
        public bool Recorder { get; set; }
        public bool Interceptor { get; set; }
        public bool Scanner { get; set; }
        public double CPU { get; set; }
        public double RAM { get; set; }
        public double TEMP { get; set; }
        public double HDD { get; set; }

    }
}
