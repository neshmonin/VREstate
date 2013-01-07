using System.Collections.Generic;
using System.IO;
using Vre.Server.BusinessLogic;
using Vre.Server.RemoteService;
using Vre.Server.Dao;
using System.Threading;
using System;

namespace Vre.Server.ModelCache
{
    internal class ModelCacheManager
    {
        private const string _primaryModelFilename = "wires.kmz";
        private const string _secondaryModelFilename = "model.kmz";

        private string _modelStorePath;
        private FileSystemWatcher _watcher;
        private Dictionary<string, ModelCache> _cache;
        private Dictionary<int, ModelCache> _cacheBySite;
        private Dictionary<int, ModelCache> _cacheByBuilding;

        public ModelCacheManager(string modelStorePath)
        {
            _watcher = null;

            _modelStorePath = modelStorePath;// Path.GetDirectoryName(modelStorePath);
        }

        public void Initialize()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(_modelStorePath);
                _watcher.Filter = string.Empty;
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = false;

                _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
                _watcher.Created += new FileSystemEventHandler(_watcher_Created);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
                _watcher.Error += new ErrorEventHandler(_watcher_Error);

                ServiceInstances.Logger.Info("MC: Started reading model files.");

                _cache = new Dictionary<string, ModelCache>();
                //_cacheByName = new Dictionary<string, ModelCache>();

                _cacheBySite = new Dictionary<int, ModelCache>();
                _cacheByBuilding = new Dictionary<int, ModelCache>();

                // produce a list of all model files reverse-sorted by write time
                List<string> files = new List<string>();
                files.AddRange(Directory.EnumerateFiles(_modelStorePath, string.Empty, SearchOption.AllDirectories));
                files.Sort(delegate(string x, string y) { return File.GetLastWriteTimeUtc(x).CompareTo(File.GetLastWriteTimeUtc(y)); });

                foreach (string path in files) tryAddNewModel(path);

                using (ClientSession cs = ClientSession.MakeSystemSession())
                {
                    cs.Resume();
                    using (EstateDeveloperDao dao = new EstateDeveloperDao(cs.DbSession))
                    {
                        foreach (EstateDeveloper ed in dao.GetAll())
                        {
                            if (ed.Deleted) continue;

                            string path;
                            foreach (Site s in ed.Sites)
                            {
                                if (s.Deleted) continue;
                                if (!string.IsNullOrEmpty(s.WireframeLocation))
                                {
                                    path = ServiceInstances.InternalFileStorageManager.ConvertToFullPath(s.WireframeLocation);
                                    addNewModelInt(path, ModelCache.ModelLevel.Site, s.AutoID, true, _cacheBySite);
                                }
                                foreach (Building b in s.Buildings)
                                {
                                    if (b.Deleted) continue;
                                    if (!string.IsNullOrEmpty(b.WireframeLocation))
                                    {
                                        path = ServiceInstances.InternalFileStorageManager.ConvertToFullPath(b.WireframeLocation);
                                        ModelCache mc = addNewModelInt(path, ModelCache.ModelLevel.Building, b.AutoID, true, _cacheByBuilding);
                                        if (mc != null)
                                        {
                                            // create/update a SuiteType-only site-level model cache
                                            ModelCache smc;
                                            if (_cacheBySite.TryGetValue(s.AutoID, out smc))
                                            {
                                                smc.MergeSuiteTypes(mc);
                                            }
                                            else
                                            {
                                                _cacheBySite.Add(s.AutoID, mc);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ServiceInstances.Logger.Info("MC: Reading model files done.");

// TODO: TEMP TESTING!!!
new Thread(aaTestThread) { IsBackground = true }.Start();

                _watcher.EnableRaisingEvents = true;
            }
        }

        #region Altitude Adjustment test
        private const bool _aaAutoAdjust = true;
        private Random _aaTestRandom = new Random();

        private void aaTestThread()
        {
            HashSet<int> processedBuildings = new HashSet<int>();
            foreach (ModelCache mc in _cache.Values) aaTest(mc, ref processedBuildings);
            foreach (ModelCache mc in _cacheBySite.Values) aaTest(mc, ref processedBuildings);
            foreach (ModelCache mc in _cacheByBuilding.Values) aaTest(mc, ref processedBuildings);
        }

        private void aaTest(ModelCache mc, ref HashSet<int> processedBuildings)
        {
            foreach (KeyValuePair<int, ModelCache.BuildingInfo> kvp in mc._info._buildingInfo)
            {
                if (processedBuildings.Add(kvp.Key))
                {
                    Thread.Sleep(5000 + _aaTestRandom.Next(10000));  // provide delay not to overuse Google API
                    try
                    {
                        aaTestBuilding(kvp.Key, kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        ServiceInstances.Logger.Error("AA Test for buidling {0} ({1}) failed: {2}", kvp.Value._name, kvp.Key, ex);
                    }
                }
            }
        }

        protected void aaTestBuilding(int bid, ModelCache.BuildingInfo mcb)
        {
            double aa =
                Vre.Server.Command.ModelImport.queryForLocationAltitude(mcb._location.Longitude, mcb._location.Latitude)
                - mcb._location.Altitude;

            Building b;
            bool adjusted = false;
            double originalDbAa;
            using (NHibernate.ISession dbSession = NHibernateHelper.GetSession())
                using (INonNestedTransaction tran = NHibernateHelper.OpenNonNestedTransaction(dbSession))
                {
                    using (BuildingDao dao = new BuildingDao(dbSession))
                    {
                        b = dao.GetById(bid);
                        originalDbAa = b.AltitudeAdjustment;

                        if (_aaAutoAdjust &&
                            ((Math.Abs(originalDbAa) < 0.000001)
                            || (Math.Abs(originalDbAa - Math.Round(originalDbAa, 2)) < 0.000001)))
                        {
                            b.AltitudeAdjustment = aa;
                            b.MarkUpdated();
                            dao.SafeUpdate(b);
                            adjusted = true;
                        }
                    }
                    if (adjusted) tran.Commit();
                }

            if (adjusted)
                ServiceInstances.Logger.Debug("AA Test {0} ({1}): GAA={2} DBAA={3} -> ADJUSTED", mcb._name, bid, aa, originalDbAa);
            else
                ServiceInstances.Logger.Debug("AA Test {0} ({1}): GAA={2} DBAA={3}", mcb._name, bid, aa, originalDbAa);
        }
        #endregion

        /// <summary>
        /// Generic auto type-resolving version
        /// </summary>
        public bool FillWithModelInfo(UpdateableBase target, bool withSubObjects)
        {
            bool result = false;

            Site site = target as Site;
            if (site != null)
            {
                result = FillWithModelInfo(site, withSubObjects);
            }
            else
            {
                Building building = target as Building;
                if (building != null)
                {
                    result = FillWithModelInfo(building, withSubObjects);
                }
                else
                {
                    Suite suite = target as Suite;
                    if (suite != null)
                    {
                        result = FillWithModelInfo(suite, withSubObjects);
                    }
                    else
                    {
                        SuiteType suiteType = target as SuiteType;
                        if (suiteType != null)
                        {
                            result = FillWithModelInfo(suiteType, withSubObjects);
                        }
                    }
                }
            }

            return result;
        }

        public bool FillWithModelInfo(Site site, bool withSubObjects)
        {
            bool result = false;
            ModelCache mc = null;

            lock (this)
            {
                if (null != _watcher)  // initialized
                {
                    result = _cacheBySite.TryGetValue(site.AutoID, out mc);
                }
            }

            if (result) mc.UpdateBo(site, withSubObjects);

            return result;
        }

        public bool FillWithModelInfo(SuiteType suiteType, bool withSubObjects)
        {
            bool result = false;
            ModelCache mc = null;

            lock (this)
            {
                if (null != _watcher)  // initialized
                {
                    result = _cacheBySite.TryGetValue(suiteType.ConstructionSite.AutoID, out mc);
                }
            }

            if (result) mc.UpdateBo(suiteType, withSubObjects);

            return result;
        }

        public bool FillWithModelInfo(Building building, bool withSubObjects)
        {
            bool result = false;
            ModelCache mc = null;

            lock (this)
            {
                if (null != _watcher)  // initialized
                {
                    if (_cacheByBuilding.TryGetValue(building.AutoID, out mc))
                    {
                        result = true;
                    }
                    else if (_cacheBySite.TryGetValue(building.ConstructionSite.AutoID, out mc))
                    {
                        result = true;
                    }
                }
            }

            if (result) mc.UpdateBo(building, withSubObjects);

            return result;
        }

        public bool FillWithModelInfo(Suite suite, bool withSubObjects)
        {
            bool result = false;
            ModelCache mc = null;

            lock (this)
            {
                if (null != _watcher)  // initialized
                {
                    if (_cacheByBuilding.TryGetValue(suite.Building.AutoID, out mc))
                    {                        
                        result = true;
                    }
                    else if (_cacheBySite.TryGetValue(suite.Building.ConstructionSite.AutoID, out mc))
                    {
                        result = true;
                    }
                }
            }

            if (result) mc.UpdateBo(suite, withSubObjects);

            return result;
        }

        private void tryAddNewModel(string path)
        {
            ModelCache.ModelLevel level;
            int objectId;
            bool isOverride;

            if (Path.GetFileName(path).Equals(_primaryModelFilename, System.StringComparison.InvariantCultureIgnoreCase))
                isOverride = true;
            else if (Path.GetFileName(path).Equals(_secondaryModelFilename, System.StringComparison.InvariantCultureIgnoreCase))
                isOverride = false;
            else
                return;

            if (objectFromPath(path, out level, out objectId))
            {
                Dictionary<int, ModelCache> cl = null;

                if (ModelCache.ModelLevel.Site == level) cl = _cacheBySite;
                else if (ModelCache.ModelLevel.Building == level) cl = _cacheByBuilding;

                if (cl != null)
                    addNewModelInt(path, level, objectId, isOverride, cl);
            }
        }

        private ModelCache addNewModelInt(string path, ModelCache.ModelLevel level, int objectId, bool isOverride, Dictionary<int, ModelCache> cl)
        {
            bool insert = false;
            ModelCache result = null;

            if (cl.TryGetValue(objectId, out result))  // if cache object exists...
            {
                if (!result.IsOverride && isOverride) insert = true;
                else if (result.IsOverride == isOverride) insert = (result.UpdatedTime < File.GetLastWriteTimeUtc(path));  // ... check if new newer
            }
            else
            {
                insert = true;
            }

            if (insert)
            {
                result = new ModelCache(path, level, isOverride, objectId);
                if (result.IsValid)
                {
                    ServiceInstances.Logger.Info("Added/replaced model: {0}", path);
                    cl[objectId] = result;
                    _cache[path] = result;
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Error("MC: File system watcher failed.  Recreating.\r\n{0}", e.GetException());
                try { _watcher.Dispose(); } catch { }
                _watcher = null;
                Initialize();
            }
        }

        private void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (this)
            {
                ModelCache mc;
                if (_cache.TryGetValue(e.OldFullPath, out mc))
                {
                    _cache.Remove(e.OldFullPath);
                    ServiceInstances.Logger.Info("MC: Detected renaming of model file: \"{0}\" -> \"{1}\"; model removed.", e.OldFullPath, e.FullPath);
                }
            }
        }

        private void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ModelCache mc;
                if (_cache.TryGetValue(e.FullPath, out mc))
                {
                    // TODO: is this proper behavior?
                    //string name = _cache[e.FullPath].SiteName;
                    _cache.Remove(e.FullPath);
                    if (ModelCache.ModelLevel.Site == mc.Level) _cacheBySite.Remove(mc.ObjectId);
                    else if (ModelCache.ModelLevel.Building == mc.Level) _cacheByBuilding.Remove(mc.ObjectId);
                    //_cacheByName.Remove(name);
                    ServiceInstances.Logger.Info("MC: Detected removal of model file: \"{0}\"", e.FullPath);
                }
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                tryAddNewModel(e.FullPath);
            }
        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                ModelCache mc;
                if (_cache.TryGetValue(e.FullPath, out mc))
                {
                    //string name = mc.SiteName;
                    if (mc.TryReRead(e.FullPath))
                    {
                        ServiceInstances.Logger.Info("MC: Re-read model file: \"{0}\"", e.FullPath);

                        //if (!name.Equals(mc.SiteName))
                        //{
                        //    ModelCache emc;
                        //    if (_cacheByName.TryGetValue(mc.SiteName, out emc))
                        //    {
                        //        ServiceInstances.Logger.Warn("MC: Model file re-read (\"{0}\") resulted in site name change: '{1}'->'{2}' which resulted in model \"{3}\" being removed from cache.",
                        //            e.FullPath, name, mc.SiteName, emc.FilePath);
                         
                        //        _cacheByName.Remove(mc.SiteName);
                        //    }
                        //    else
                        //    {
                        //        ServiceInstances.Logger.Warn("MC: Model file re-read (\"{0}\") resulted in site name change: '{1}'->'{2}'", 
                        //            e.FullPath, name, mc.SiteName);
                        //    }

                        //    _cacheByName.Remove(name);
                        //    _cacheByName.Add(mc.SiteName, mc);
                        //}
                    }
                }
            }
        }

        #region path-object conversion
        //private const string SiteDirPrefix = "site";
        //private const string BuildingDirPrefix = "building";

        //private string pathFromObject(Site site)
        //{
        //    return Path.Combine(_modelStorePath, string.Format("{0}{1}", SiteDirPrefix, site.AutoID));
        //}

        //private string pathFromObject(Building building)
        //{
        //    return Path.Combine(_modelStorePath, string.Format("{0}{1}", BuildingDirPrefix, building.AutoID));
        //}

        //private string pathFromObject(Suite suite)
        //{
        //    return Path.Combine(_modelStorePath, string.Format("building{0}", suite.Building.AutoID));
        //}

        private bool objectFromPath(string path, out ModelCache.ModelLevel level, out int id)
        {
            bool result = false;

            level = ModelCache.ModelLevel.Building; id = 0;  // defaults

            if (path.StartsWith(_modelStorePath, System.StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Substring(_modelStorePath.Length);
                
                string[] parts = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                int startIdx = 0;

                // remove trailing path separator element if any
                if (0 == parts[0].Length) startIdx = 1;  // parts array cannot have zero elements (by design)

                // path in form <developer id>\<site id>\model.kmz
                if (3 == (parts.Length - startIdx))  // currently support a single nesting level; last item is file name
                {
                    if (int.TryParse(parts[startIdx + 1], out id))
                    {
                        level = ModelCache.ModelLevel.Site;
                        result = true;
                        //result = Path.GetFileNameWithoutExtension(parts[startIdx + 2])
                        //    .Equals("model", System.StringComparison.InvariantCultureIgnoreCase);
                    }
                }
                // path in form <developer id>\<site id>\<building id>\model.kmz
                else if (4 == (parts.Length - startIdx))
                {
                    if (int.TryParse(parts[startIdx + 2], out id))
                    {
                        ServiceInstances.Logger.Error("Building-level models are not supported yet!");
                        // TODO: To support this ModelCache class must handle level properly and create BuildingInfo directly instead of SiteInfo
                        // See ModelCache.parseFile() for details
                        //level = ModelCache.ModelLevel.Building;
                        //result = true;
                    }
                }
            }

            return result;
        }
        #endregion
    }
}