using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NHibernate;
using Vre.Server.BusinessLogic;
using Vre.Server.Dao;
using Vre.Server.RemoteService;

namespace Vre.Server.Command
{
    internal class ModelImport
    {
        private CsvSuiteTypeInfo _extraSuiteInfo;
        private StringBuilder _log;
        private Dictionary<string, SuiteType> _typeCache;
        private ClientSession _clientSession;
        //private ISession _session;

        public static void ImportModel(string estateDeveloperName, string siteName,
            string modelFileName, string extraSuiteInfoFileName, bool dryRun)
        {
            StringBuilder log = new StringBuilder();
            string logFileName = Path.Combine(Path.GetDirectoryName(modelFileName), Path.GetFileNameWithoutExtension(modelFileName)) 
                + ".import.log.txt";
            FileStream logFile = File.Open(logFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

            Exception importError = null;
            
            try
            {
                DatabaseSettingsDao.VerifyDatabase();

                ModelImport instance = new ModelImport();
                instance._log = log;
                instance.doImport(estateDeveloperName, siteName, modelFileName, extraSuiteInfoFileName, dryRun);
                //instance.generateSqlScript(estateDeveloperName, siteName, modelFileName, extraSuiteInfoFileName);
            }
            catch (Exception e)
            {
                importError = e;
                log.AppendFormat("Error importing model: {0}\r\n{1}", e.Message, e.StackTrace);
            }

            logFile.Seek(0, SeekOrigin.End);
            using (StreamWriter sw = new StreamWriter(logFile)) sw.Write(log);

            if (importError != null) throw importError;
        }

        private void generateSqlScript(string estateDeveloperName, string siteName,
            string modelFileName, string extraSuiteInfoFileName)
        {
            _extraSuiteInfo = new CsvSuiteTypeInfo(extraSuiteInfoFileName);

            StringBuilder readWarnings = new StringBuilder();
            Vre.Server.Model.Kmz.Kmz kmz = new Vre.Server.Model.Kmz.Kmz(modelFileName, readWarnings);

            using (FileStream file = File.Create(kmz.Model.Site.Name + ".sql"))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine("declare @sid int");
                    sw.WriteLine("declare @buildingid int");
                    sw.WriteLine("declare @siteid int");
                    sw.WriteLine("set @siteid = -1");

                    sw.WriteLine("declare @timestamp datetime");
                    sw.WriteLine("set @timestamp = getdate()");

                    Dictionary<string, string> stypes = new Dictionary<string, string>();

                    foreach (Vre.Server.Model.Kmz.Building bldg in kmz.Model.Site.Buildings)
                    {
                        foreach (Vre.Server.Model.Kmz.Suite s in bldg.Suites)
                        {
                            if (!stypes.ContainsKey(s.ClassName))
                            {
                                string variable = "@st_" + s.ClassName.Replace('-', '_');
                                sw.WriteLine("declare " + variable + " int");
                                sw.WriteLine("select " + variable + " = [autoid] from [suitetypes] where [siteid] = @siteid and [name] = '" + s.ClassName + "'");
                                sw.WriteLine("if " + variable + " is null begin");
                                sw.WriteLine("  insert into [suitetypes] ([created],[updated],[deleted],[name],[siteid],[floorarea],[bedroomcnt],[dencnt],[bathroomcnt],[balconycnt],[terracecnt]) values(@timestamp,@timestamp,0,'{0}',@siteid,'{1},2',{2},{3},{4},{5},{6})",
                                    s.ClassName, _extraSuiteInfo.GetFloorAreaSqFt(s.ClassName),
                                    _extraSuiteInfo.GetBedroomCount(s.ClassName),
                                    _extraSuiteInfo.GetDenCount(s.ClassName),
                                    _extraSuiteInfo.GetBathroomCount(s.ClassName),
                                    _extraSuiteInfo.GetBalconyCount(s.ClassName),
                                    _extraSuiteInfo.GetTerraceCount(s.ClassName));
                                sw.WriteLine("  set " + variable + " = @@identity");
                                sw.WriteLine("end");
                                stypes.Add(s.ClassName, variable);
                            }
                        }
                    }

                    foreach (Vre.Server.Model.Kmz.Building bldg in kmz.Model.Site.Buildings)
                    {                        
                        sw.WriteLine("select @buildingid = [autoid] from [buildings] where [name] = '{0}' and siteid = @siteid", bldg.Name);
                        foreach (Vre.Server.Model.Kmz.Suite s in bldg.Suites)
                        {
                            sw.WriteLine("set @sid = null");
                            sw.WriteLine("select @sid = [autoid] from [suites] where [buildingid] = @buildingid and [suitename] = '" + s.Name + "'");
                            sw.WriteLine("if @sid is null begin");
                            sw.WriteLine("  insert into [suites] ([created],[updated],[deleted],[buildingid],[physicallevelnumber],[floorname],[suitename],[status],[suitetypeid],[ceilingheight],[showpanoramicview]) values(@timestamp,@timestamp,0,@buildingid,{0},'{1}','{2}',0,{3},{4},{5})",
                                1, "1", s.Name, stypes[s.ClassName], s.CeilingHeightFt, s.ShowPanoramicView ? "1" : "0");
                            sw.WriteLine("end else begin");
                            sw.WriteLine("  update [suites] set [updated] = @timestamp, [suitetypeid] = {0}, [ceilingheight] = {1}, [showpanoramicview] = {2} where [autoid] = @sid",
                                stypes[s.ClassName], s.CeilingHeightFt, s.ShowPanoramicView ? "1" : "0");
                            sw.WriteLine("end");
                        }
                    }
                }
            }

        }

        private void doImport(string estateDeveloperName, string siteName,
            string modelFileName, string extraSuiteInfoFileName, bool dryRun)
        {
            _extraSuiteInfo = new CsvSuiteTypeInfo(extraSuiteInfoFileName);

            //_session = NHibernateHelper.GetSession();
            using (_clientSession = ClientSession.MakeSystemSession())//_session))
            {
                _clientSession.Resume();
                //_clientSession.DbSession.FlushMode = FlushMode.Always;
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(_clientSession.DbSession))
                {
                    EstateDeveloper developer;
                    Site site = null;
                    _typeCache = new Dictionary<string, SuiteType>();

                    using (EstateDeveloperDao dao = new EstateDeveloperDao(_clientSession.DbSession))
                        developer = dao.GetById(estateDeveloperName);

                    if (null == developer) throw new ObjectNotFoundException(estateDeveloperName, "EstateDeveloper");
                    _log.AppendFormat("Using estate developer ID={0}, Name={1}\r\n", developer.AutoID, developer.Name);

                    bool siteCreated = false;

                    if (siteName != null)
                    {
                        using (SiteDao dao = new SiteDao(_clientSession.DbSession))
                            site = dao.GetById(developer, siteName);

                        if (null == site)
                        {
                            site = new Site(developer, siteName);
                            _clientSession.DbSession.Save(site);
                            siteCreated = true;
                            _log.AppendFormat("Created new site provided by command-line ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                        else
                        {
                            _log.AppendFormat("Using command-line override site ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                    }

                    StringBuilder readWarnings = new StringBuilder();
                    Vre.Server.Model.Kmz.Kmz kmz = new Vre.Server.Model.Kmz.Kmz(modelFileName, readWarnings);

                    if (null == site)
                    {
                        using (SiteDao dao = new SiteDao(_clientSession.DbSession))
                            site = dao.GetById(developer, kmz.Model.Site.Name);

                        if (null == site)
                        {
                            site = new Site(developer, kmz.Model.Site.Name);
                            _clientSession.DbSession.Save(site);
                            siteCreated = true;
                            _log.AppendFormat("Created new site provided by model ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                        else
                        {
                            _log.AppendFormat("Using site provided by model ID={0}, Name={1}\r\n", site.AutoID, site.Name);
                        }
                    }

                    importSite(kmz.Model.Site, site, siteCreated);

                    if (dryRun)
                    {
                        tran.Rollback();
                        _log.Append("DRY RUN MODE: all changes rolled back.");
                    }
                    else
                    {
                        //_clientSession.DbSession.Flush();
                        tran.Commit();
                        _log.Append("All changes comitted to database.");
                    }
                }  // transaction
            }  // client session
        }

        private void importSite(Vre.Server.Model.Kmz.ConstructionSite modelSite, Site dbSite, bool isCreated)
        {
            List<string> missingBuildings = new List<string>(dbSite.Buildings.Count);
            foreach (Building b in dbSite.Buildings) missingBuildings.Add(b.Name);

            foreach (Vre.Server.Model.Kmz.Building mb in modelSite.Buildings)
            {
                bool created = false;
                Building dbb = null;
                foreach (Building b in dbSite.Buildings)
                    if (b.Name.Equals(mb.Name) && !b.Deleted) { dbb = b; break; }

                if (null == dbb)
                {
                    foreach (Building b in dbSite.Buildings)
                        if (b.Name.Equals(mb.Name)) { dbb = b; break; }
                }

                if (null == dbb)
                {
                    dbb = new Building(dbSite, mb.Name);
                    _clientSession.DbSession.Save(dbb);
                    created = true;
                    _log.AppendFormat("Created new building ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
                }
                else
                {
                    if (dbb.Deleted)
                    {
                        dbb.Undelete();
                        using (BuildingDao dao = new BuildingDao(_clientSession.DbSession))
                            if (!dao.SafeUpdate(dbb)) throw new StaleObjectStateException("Building", dbb.AutoID);

                        _log.AppendFormat("Recovered deleted building ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
                    }
                    else
                    {
                        _log.AppendFormat("Updating existing building ID={0}, Name={1}\r\n", dbb.AutoID, dbb.Name);
                    }
                    missingBuildings.Remove(dbb.Name);
                }

                importBuilding(mb, dbb, created);
            }

            // hide missing buildings
            //
            foreach (string bn in missingBuildings)
            {
                Building dbb = null;
                foreach (Building b in dbSite.Buildings)
                    if (b.Name.Equals(bn) && !b.Deleted) { dbb = b; break; }

                if (dbb != null)
                {
                    using (BuildingDao dao = new BuildingDao(_clientSession.DbSession))
                        if (!dao.SafeDelete(dbb)) throw new StaleObjectStateException("Building", dbb.AutoID);
                    _log.AppendFormat("Removed building missing in model ID={0}, Name={1}", dbb.AutoID, dbb.Name);
                }
            }

            // find and hide unused suite types
            //
            List<string> missingSuiteTypes = new List<string>(dbSite.SuiteTypes.Count);
            foreach (SuiteType st in dbSite.SuiteTypes) missingSuiteTypes.Add(st.Name);
            foreach (string stn in _typeCache.Keys) missingSuiteTypes.Remove(stn);
            foreach (string stn in missingSuiteTypes)
            {
                SuiteType dbst = null;
                foreach (SuiteType st in dbSite.SuiteTypes)
                    if (st.Name.Equals(stn) && !st.Deleted) { dbst = st; break; }

                if (dbst != null)
                {
                    using (SuiteTypeDao dao = new SuiteTypeDao(_clientSession.DbSession))
                        if (!dao.SafeDelete(dbst)) throw new StaleObjectStateException("SuiteType", dbst.AutoID);
                    _log.AppendFormat("Removed suite type missing in model ID={0}, Name={1}", dbst.AutoID, dbst.Name);
                }
            }
        }

        private void importBuilding(Vre.Server.Model.Kmz.Building modelBuilding, Building dbBuilding, bool isCreated)
        {
            List<string> missingSuites = new List<string>(dbBuilding.Suites.Count);

            foreach (Suite s in dbBuilding.Suites) missingSuites.Add(s.SuiteName);

            foreach (Vre.Server.Model.Kmz.Suite ms in modelBuilding.Suites)
            {
                bool created = false;
                Suite dbs = null;
                foreach (Suite s in dbBuilding.Suites)
                    if (s.SuiteName.Equals(ms.Name) && !s.Deleted) { dbs = s; break; }

                if (null == dbs)
                {
                    foreach (Suite s in dbBuilding.Suites)
                        if (s.SuiteName.Equals(ms.Name)) { dbs = s; break; }
                }

                if (null == dbs)
                {
                    dbs = new Suite(dbBuilding, -1, ms.Floor, ms.Name);
                    _clientSession.DbSession.Save(dbs);
                    created = true;
                    _log.AppendFormat("Created new suite ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                }
                else
                {
                    if (dbs.Deleted)
                    {
                        dbs.Undelete();
                        using (SuiteDao dao = new SuiteDao(_clientSession.DbSession))
                            if (!dao.SafeUpdate(dbs)) throw new StaleObjectStateException("Suite", dbs.AutoID);
                        _log.AppendFormat("Recovered deleted suite ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                    }
                    else
                    {
                        _log.AppendFormat("Updating existing suite ID={0}, Name={1}\r\n", dbs.AutoID, dbs.SuiteName);
                    }
                    missingSuites.Remove(dbs.SuiteName);
                }

                importSuite(ms, dbs, created);
            }

            foreach (string sn in missingSuites)
            {
                Suite dbs = null;
                foreach (Suite s in dbBuilding.Suites)
                    if (s.SuiteName.Equals(sn) && !s.Deleted) { dbs = s; break; }

                if (dbs != null)
                {
                    using (SuiteDao dao = new SuiteDao(_clientSession.DbSession))
                        if (!dao.SafeDelete(dbs)) throw new StaleObjectStateException("Suite", dbs.AutoID);
                    _log.AppendFormat("Removed suite missing in model ID={0}, Name={1}", dbs.AutoID, dbs.SuiteName);
                }
            }
        }

        private void importSuite(Vre.Server.Model.Kmz.Suite modelSuite, Suite dbSuite, bool isCreated)
        {
            bool changed = isCreated;

            if (isCreated)
            {
                // this must be the first call on new suite as it re-reads suite from DB;
                // all subsequent changes shall be lost!
                using (SiteManager mgr = new SiteManager(_clientSession))
                    mgr.SetSuitePrice(dbSuite, (float)_extraSuiteInfo.GetDefaultInitialPrice(modelSuite.ClassName));

                if (0 == modelSuite.CeilingHeightFt)
                    dbSuite.CeilingHeight = new ValueWithUM(
                        _extraSuiteInfo.GetCeilingHeightFt(modelSuite.ClassName), ValueWithUM.Unit.Feet);
                else
                    dbSuite.CeilingHeight = new ValueWithUM(
                        modelSuite.CeilingHeightFt, ValueWithUM.Unit.Feet);

                dbSuite.ShowPanoramicView = _extraSuiteInfo.GetDefaultShowPanoramicView(modelSuite.ClassName);

                dbSuite.Status = Suite.SalesStatus.Available;

                setSuiteType(dbSuite, modelSuite.ClassName);
            }
            else
            {
                if (!dbSuite.SuiteType.Name.Equals(modelSuite.ClassName))
                {
                    setSuiteType(dbSuite, modelSuite.ClassName);
                    changed = true;
                }
            }

            //if (changed)
            //{
            //    _clientSession.DbSession.Flush();

            //    //using (SuiteDao dao = new SuiteDao(_clientSession.DbSession))
            //    //    if (!dao.SafeUpdate(dbSuite)) throw new StaleObjectStateException("Suite", dbSuite.AutoID);

            //    _clientSession.DbSession.Refresh(dbSuite);
            //}
        }

        private void setSuiteType(Suite dbSuite, string newClassName)
        {
            SuiteType stype = null;

            if (!_typeCache.TryGetValue(newClassName, out stype))
            {
                stype = new SuiteType(dbSuite.Building.ConstructionSite, newClassName);

                stype.BalconyCount = _extraSuiteInfo.GetBalconyCount(newClassName);
                stype.TerraceCount = _extraSuiteInfo.GetTerraceCount(newClassName);
                stype.BathroomCount = _extraSuiteInfo.GetBathroomCount(newClassName);
                stype.BedroomCount = _extraSuiteInfo.GetBedroomCount(newClassName);
                stype.DenCount = _extraSuiteInfo.GetDenCount(newClassName);
                stype.FloorArea = new ValueWithUM(_extraSuiteInfo.GetFloorAreaSqFt(newClassName), ValueWithUM.Unit.SqFeet);

                _clientSession.DbSession.Save(stype);
                _log.AppendFormat("Created new suite type ID={0}, Name={1}\r\n", stype.AutoID, stype.Name);

                _typeCache.Add(stype.Name, stype);
            }

            dbSuite.SuiteType = stype;
        }

        internal class CsvSuiteTypeInfo
        {
            private Dictionary<string, string[]> _rawData;

            public CsvSuiteTypeInfo(string fileName)
            {
                if (null == fileName) return;

                _rawData = new Dictionary<string, string[]>();
                using (FileStream fs = File.OpenRead(fileName))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string line = sr.ReadLine();
                        string[] parts = splitCsvLine(line);
                        if (10 != parts.Length) throw new ArgumentException("The suite type information file is not of correct format");

                        do
                        {
                            line = sr.ReadLine();
                            parts = splitCsvLine(line);
                            if (10 != parts.Length) throw new ArgumentException("The suite type information file is not of correct format");

                            _rawData.Add(parts[0], parts);
                        }
                        while (!sr.EndOfStream);
                    }
                }
            }

            private static string[] splitCsvLine(string line)
            {
                int cnt = 1, idx, idx0;
                bool escape = false;

                for (idx = 0; idx < line.Length; idx++)
                    if ('\"' == line[idx]) escape = !escape;
                    else if ((',' == line[idx]) && !escape) cnt++;
                if (escape) throw new ArgumentException("The text is not a valid CSV");

                string[] result = new string[cnt];
                cnt = 0; idx = 0; idx0 = 0;
                do
                {
                    if ('\"' == line[idx])
                    {
                        escape = !escape;
                    }
                    else if ((',' == line[idx]) && !escape)
                    {
                        result[cnt] = line.Substring(idx0, idx - idx0);
                        idx0 = idx + 1;
                        cnt++;
                    }

                    idx++;
                } 
                while (idx < line.Length);

                result[cnt] = line.Substring(idx0, idx - idx0);

                return result;
            }

            private string[] getPropArray(string suiteType)
            {
                if (null == _rawData) return null;
                string[] result;
                if (_rawData.TryGetValue(suiteType, out result)) return result;
                return null;
            }

            private double getProp(string suiteType, int pos, double defValue)
            {
                double result;
                string[] arr = getPropArray(suiteType);
                if ((arr != null) && !string.IsNullOrWhiteSpace(arr[pos]) && double.TryParse(arr[pos], out result)) 
                    return result;
                return defValue;
            }

            private int getProp(string suiteType, int pos, int defValue)
            {
                int result;
                string[] arr = getPropArray(suiteType);
                if ((arr != null) && !string.IsNullOrWhiteSpace(arr[pos]) && int.TryParse(arr[pos], out result)) 
                    return result;
                return defValue;
            }

            private bool getProp(string suiteType, int pos, bool defValue)
            {
                bool result;
                string[] arr = getPropArray(suiteType);
                if ((arr != null) && !string.IsNullOrWhiteSpace(arr[pos]) && tryParse(arr[pos], out result)) 
                    return result;
                return defValue;
            }

            private bool tryParse(string str, out bool result)
            {
                str = str.ToLower();

                if (str.Equals("yes") || str.Equals("true")
                    || str.Equals("y") || str.Equals("1")) { result = true; return true; }
                else if (str.Equals("no") || str.Equals("false")
                    || str.Equals("n") || str.Equals("0")) { result = false; return true; }
                else { result = false; return false; }
            }

            public double GetFloorAreaSqFt(string suiteType)
            {
                return getProp(suiteType, 1, 0.0);
            }

            public double GetCeilingHeightFt(string suiteType)
            {
                return getProp(suiteType, 7, 0.0);
            }

            public int GetBedroomCount(string suiteType)
            {
                return getProp(suiteType, 2, 0);
            }

            public int GetDenCount(string suiteType)
            {
                return getProp(suiteType, 3, 0);
            }

            public int GetBathroomCount(string suiteType)
            {
                return getProp(suiteType, 4, 0);
            }

            public int GetBalconyCount(string suiteType)
            {
                return getProp(suiteType, 5, 0);
            }

            public int GetTerraceCount(string suiteType)
            {
                return getProp(suiteType, 6, 0);
            }

            public double GetDefaultInitialPrice(string suiteType)
            {
                return getProp(suiteType, 8, 0.0);
            }

            public bool GetDefaultShowPanoramicView(string suiteType)
            {
                return getProp(suiteType, 9, true);
            }
        }
    }
}