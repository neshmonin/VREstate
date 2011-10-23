using System;

namespace Vre.Server.Dao
{
    internal class DatabaseSettingsDao
    {
        private class Dao : GenericDao<DatabaseSettings, int> { }

        public static void VerifyDatabase()
        {
            DatabaseSettings setting = null;

            try
            {
                using (Dao dao = new Dao())
                {
                    setting = dao.GetById((int)DatabaseSettings.Setting.DbVersion);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Database is not accessible or is invalid.", ex);
            }

            if (null == setting) throw new ApplicationException("Database is not initialized.");
            if (!setting.IntValue.HasValue) throw new ApplicationException("Database is invalid.");

            if (setting.IntValue == DatabaseSettings.CurrentDbVersion)
            {
                DateTime created = Get(DatabaseSettings.Setting.CreateTime, DateTime.MinValue);
                ServiceInstances.Logger.Info("Database version OK; created {0}.", created);
                return;
            }

            throw new ApplicationException("Database version is not supported: required = " +
                DatabaseSettings.CurrentDbVersion.ToString() + ", database value = " +
                setting.IntValue.ToString() + ".");
        }

        private static DatabaseSettings Get(Dao dao, DatabaseSettings.Setting id)
        {
            DatabaseSettings setting = dao.GetById((int)id);
            if (null == setting)
            {
                setting = new DatabaseSettings();
                setting.Id = (int)id;
            }
            return setting;
        }

        #region static safe strong-typed getters
        public static bool Get(DatabaseSettings.Setting id, bool defaultValue)
        {
            DatabaseSettings result = null;
            using (Dao dao = new Dao()) result = Get(dao, id);
            if (null == result) return defaultValue;
            if (!result.BitValue.HasValue) return defaultValue;
            return result.BitValue.Value;
        }

        public static long Get(DatabaseSettings.Setting id, long defaultValue)
        {
            DatabaseSettings result = null;
            using (Dao dao = new Dao()) result = Get(dao, id);
            if (null == result) return defaultValue;
            if (!result.IntValue.HasValue) return defaultValue;
            return result.IntValue.Value;
        }

        public static double Get(DatabaseSettings.Setting id, double defaultValue)
        {
            DatabaseSettings result = null;
            using (Dao dao = new Dao()) result = Get(dao, id);
            if (null == result) return defaultValue;
            if (!result.FloatValue.HasValue) return defaultValue;
            return result.FloatValue.Value;
        }

        public static string Get(DatabaseSettings.Setting id, string defaultValue)
        {
            DatabaseSettings result = null;
            using (Dao dao = new Dao()) result = Get(dao, id);
            if (null == result) return defaultValue;
            if (null == result.TextValue) return defaultValue;
            return result.TextValue;
        }

        public static DateTime Get(DatabaseSettings.Setting id, DateTime defaultValue)
        {
            DatabaseSettings result = null;
            using (Dao dao = new Dao()) result = Get(dao, id);
            if (null == result) return defaultValue;
            if (!result.TimeValue.HasValue) return defaultValue;
            return NHibernateHelper.DateTimeFromDb(result.TimeValue.Value);
        }
        #endregion

        #region static safe strong-typed setters
        public static void Set(DatabaseSettings.Setting id, bool value)
        {
            using (Dao dao = new Dao())
            {
                DatabaseSettings setting = Get(dao, id);
                setting.BitValue = value;
                dao.CreateOrUpdate(setting);
            }
        }

        public static void Set(DatabaseSettings.Setting id, long value)
        {
            using (Dao dao = new Dao())
            {
                DatabaseSettings setting = Get(dao, id);
                setting.IntValue = value;
                dao.CreateOrUpdate(setting);
            }
        }

        public static void Set(DatabaseSettings.Setting id, double value)
        {
            using (Dao dao = new Dao())
            {
                DatabaseSettings setting = Get(dao, id);
                setting.FloatValue = value;
                dao.CreateOrUpdate(setting);
            }
        }

        public static void Set(DatabaseSettings.Setting id, DateTime value)
        {
            // Enforce value range
            value = NHibernateHelper.DateTimeToDb(value);

            using (Dao dao = new Dao())
            {
                DatabaseSettings setting = Get(dao, id);
                setting.TimeValue = value;
                dao.CreateOrUpdate(setting);
            }
        }

        public static void Set(DatabaseSettings.Setting id, string value)
        {
            using (Dao dao = new Dao())
            {
                DatabaseSettings setting = Get(dao, id);
                setting.TextValue = value;
                dao.CreateOrUpdate(setting);
            }
        }
        #endregion
    }
}