﻿using CPI.Models.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CPI.DTO
{
    public static class TransferDB
    {
        public static ObservableCollection<Setting> Settings { get; set; }
        public static ObservableCollection<ARFCN> ARFCNs { get; set; }
        public static ObservableCollection<License> Licenses { get; set; }
        public static List<User> Users { get; set; }
        public static ObservableCollection<Unit> Units { get; set; }
        public static ObservableCollection<Computer> Computers { get; set; }
    }
}