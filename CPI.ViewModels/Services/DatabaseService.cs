using CPI.Models;
using CPI.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CPI.ViewModels
{
    public class DatabaseService
    {
        #region Generic Methods

        /// <summary>
        /// Add new entity or update current if exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int AddOrUpdate<T>(T entity) where T : class, IEntityWithID
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    var original = dbContext.Set<T>().SingleOrDefault(t => t.ID == entity.ID);
                    if (original != null)
                        dbContext.Entry(original).CurrentValues.SetValues(entity);
                    else
                        dbContext.Set<T>().Add(entity);


                    return dbContext.SaveChanges();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return 0;
#endif
            }
        }

        /// <summary>
        /// Add new entities or update currents if exist 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityCollection"></param>
        /// <returns></returns>
        public static int AddOrUpdate<T>(List<T> entityCollection) where T : class, IEntityWithID
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    foreach (var entity in entityCollection)
                    {
                        var original = dbContext.Set<T>().SingleOrDefault(t => t.ID == entity.ID);
                        if (original != null)
                            dbContext.Entry(original).CurrentValues.SetValues(entity);
                        else
                            dbContext.Set<T>().Add(entity);
                    }

                    return dbContext.SaveChanges();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return 0;
#endif
            }
        }

        /// <summary>
        /// Find entity by ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static T FindByID<T>(Guid ID) where T : class, IEntityWithID
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    return dbContext.Set<T>().SingleOrDefault(t => t.ID == ID);
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }

        }

        /// <summary>
        /// Get all entities from DB 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetAll<T>() where T : class
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    List<T> collection = dbContext.Set<T>().ToList();
                    return collection;
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }

        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int Delete<T>(T entity) where T : class
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    dbContext.Entry(entity).State = EntityState.Deleted;
                    return dbContext.SaveChanges();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return 0;
#endif
            }
        }

        #endregion

        #region Local Methods via SQL Stored Procedures 

        /// <summary>
        /// Return last license
        /// </summary>
        /// <returns></returns>
        public static Models.Entity.License GetLastLicense()
        {
            try
            {
                using (var dbContext = new DBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    return dbContext.sp_GetLastLicense().FirstOrDefault();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }

        }

        /// <summary>
        /// Return All Users
        /// </summary>
        /// <returns></returns>
        public static List<User> GetAllUsers()
        {
            try
            {
                using (var dbContext = new DBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    return dbContext.sp_GetAllUsers().ToList();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }
        }

        /// <summary>
        /// Return All Licenses
        /// </summary>
        /// <returns></returns>
        public static List<Models.Entity.License> GetAllLicense()
        {
            try
            {
                using (var dbContext = new DBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    return dbContext.sp_GetAllLicenses().ToList();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }

        }


        /// <summary>
        /// Return All Settings
        /// </summary>
        /// <returns></returns>
        public static List<Setting> GetAllSettings()
        {
            try
            {
                using (var dbContext = new DBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    return dbContext.sp_GetAllSettings().ToList();
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }

        }



        /// <summary>
        /// Return All Users, Settings, Licenses
        /// </summary>
        /// <returns></returns>
        public static List<object> GetAllStuff()
        {
            try
            {

                using (var dbContext = new DBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<object> stuff = new List<object>
                    {
                        dbContext.sp_GetAllUsers().Where(u => u.Permission != "SuperAdmin").ToList(),
                        dbContext.sp_GetAllSettings().ToList(),
                        dbContext.sp_GetAllLicenses().ToList(),
                        dbContext.sp_GetAllARFCNs().ToList(),
                        dbContext.sp_GetAllUnit().ToList(),
                        dbContext.sp_GetAllComputers().ToList(),
                        dbContext.sp_GetAllReceivers().ToList(),
                        dbContext.sp_GetAllSessions().ToList()
                    };
                    return stuff;
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }
        }


        public static List<object> GetAllARFCNBySession(Session session)
        {
            try
            {
                using (var dbContext = new DBContext())
                {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<object> ARFCNListBySession = new List<object>
                    {
                        dbContext.sp_GetAllARFCNsBySession(session.ID).ToList()
                       
                    };
                    return ARFCNListBySession;
                }
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
#if (DEBUG)
                throw;
#else
                MessageBox.Show(ex.Message);
                return null;
#endif
            }
        }


    

        #endregion

    }
}
