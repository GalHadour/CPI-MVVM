using CPI.DTO;
using CPI.Models.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace CPI.ViewModels
{
    public static class DataDB
    {

        #region Methods 

        public static void GetDataDB()
        {

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result = DatabaseService.GetAllStuff();
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Result != null)
                {
                    List<object> collection = (List<object>)e.Result;

                    TransferDB.Users = collection[0] as List<User>;
                    TransferDB.Settings = new ObservableCollection<Setting>(collection[1] as List<Setting>);
                    TransferDB.Licenses = new ObservableCollection<Models.Entity.License>(collection[2] as List<Models.Entity.License>);
                    TransferDB.ARFCNs = new ObservableCollection<ARFCN>(collection[3] as List<ARFCN>);
                    TransferDB.Units = new ObservableCollection<Unit>(collection[4] as List<Unit>);
                    TransferDB.Computers = new ObservableCollection<Computer>(collection[5] as List<Computer>);

                }
            };
            worker.RunWorkerAsync();
        }
        public static void RefreshLicenses()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                e.Result = DatabaseService.GetAllLicense();
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                TransferDB.Licenses = new ObservableCollection<Models.Entity.License>(e.Result as List<Models.Entity.License>);
            };
            worker.RunWorkerAsync();
        }

        public static int UpdateSetting()
        {
            List<Setting> settingsList = new List<Setting>();
            foreach (var item in TransferDB.Settings.Where(s => s.NewValue != s.OldValue))
            {
                item.OldValue = item.NewValue;
                settingsList.Add(item);
            }
            if (settingsList.Count != 0)
                return DatabaseService.AddOrUpdate(settingsList);
            else
                return 0;
        }

        public static int UpdateAllStuff()
        {
            int status = UpdateSetting();
            status += DatabaseService.AddOrUpdate(new List<Unit>(TransferDB.Units));
            status += DatabaseService.AddOrUpdate(new List<Computer>(TransferDB.Computers));
            return status;
        }


        #endregion
    }
}
