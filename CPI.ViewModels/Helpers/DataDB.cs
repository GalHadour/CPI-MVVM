﻿using CPI.DTO;
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
                    TransferDB.Receivers = new ObservableCollection<Receiver>(collection[6] as List<Receiver>);
                    TransferDB.SessionsList= new ObservableCollection<Session>(collection[7] as List<Session>);
                    TransferDB.SMSs = new ObservableCollection<SMS>(collection[8] as List<SMS>);
                    TransferDB.Calls = new ObservableCollection<Call>(collection[9] as List<Call>);
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

        public static int AddUpdateARFCN()
        {
            return DatabaseService.AddOrUpdate(new List<ARFCN>(TransferDB.ARFCNs));
        }

        public static int AddUpdateSessions()
        {
            
            return DatabaseService.AddOrUpdate(new List<Session>(TransferDB.SessionsList));
        }


        public static void UpdateARFCNListBySession(Session SelectedSession)
        {
           
            List<object> collection = (List<object>)DatabaseService.GetAllARFCNBySession(SelectedSession);
            TransferDB.ARFCNs = new ObservableCollection<ARFCN>(collection[0] as List<ARFCN>);
            
            AddUpdateARFCN();///
        }

        public static void DeleteAllSessions()
        {
            foreach (Session session in TransferDB.SessionsList)
            {
                DatabaseService.Delete<Session>(session);
            }
        }

        public static void DeleteSelectedSession(Session session)
        {
              DatabaseService.Delete<Session>(session);
        }


        public static int AddUpdateSMSs()
        {
            return DatabaseService.AddOrUpdate(new List<SMS>(TransferDB.SMSs));
        }

        public static int AddUpdateCalls()
        {
            return DatabaseService.AddOrUpdate(new List<Call>(TransferDB.Calls));
        }
        #endregion

    }
}
