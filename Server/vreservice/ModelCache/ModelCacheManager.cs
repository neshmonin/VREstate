using System.Collections.Generic;
using System.IO;
using Vre.Server.BusinessLogic;

namespace Vre.Server.ModelCache
{
    internal class ModelCacheManager
    {
        private string _modelStorePath;
        private string _modelStoreFilter;
        private FileSystemWatcher _watcher;
        private Dictionary<string, ModelCache> _cache;
        //private Dictionary<string, ModelCache> _cacheByName;
        private Dictionary<int, ModelCache> _cacheBySite;
        private Dictionary<int, ModelCache> _cacheByBuilding;

        public ModelCacheManager(string modelStorePath)
        {
            _watcher = null;

            _modelStorePath = modelStorePath;// Path.GetDirectoryName(modelStorePath);
            _modelStoreFilter = "model.kmz";// Path.GetFileName(modelStorePath);
        }

        public void Initialize()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(_modelStorePath);
                _watcher.Filter = Path.GetFileName(_modelStoreFilter);
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
                files.AddRange(Directory.EnumerateFiles(_modelStorePath, _modelStoreFilter, SearchOption.AllDirectories));
                files.Sort(delegate(string x, string y) { return File.GetLastWriteTimeUtc(x).CompareTo(File.GetLastWriteTimeUtc(y)); });

                foreach (string path in files) tryAddNewModel(path);

                ServiceInstances.Logger.Info("MC: Reading model files done.");

                _watcher.EnableRaisingEvents = true;
            }
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

            if (objectFromPath(path, out level, out objectId))
            {
                ModelCache mc;
                bool insert = false;
                Dictionary<int, ModelCache> cl = null;

                if (ModelCache.ModelLevel.Site == level) cl = _cacheBySite;
                else if (ModelCache.ModelLevel.Building == level) cl = _cacheByBuilding;

                if (cl != null)
                {
                    if (cl.TryGetValue(objectId, out mc))  // if cache object exists...
                        insert = (mc.UpdatedTime < File.GetLastWriteTimeUtc(path));  // ... check if new newer
                    else
                        insert = true;

                    if (insert)
                    {
                        mc = new ModelCache(path, level, objectId);
                        if (mc.IsValid)
                        {
                            cl[objectId] = mc;
                            _cache[path] = mc;
                        }
                    }
                }
            }
        }

        private void _watcher_Error(object sender, ErrorEventArgs e)
        {
            lock (this)
            {
                ServiceInstances.Logger.Error("MC: File system watcher failed.  Recreating.");
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
                    _cache.Add(e.FullPath, mc);
                    ServiceInstances.Logger.Info("MC: Detected renaming of model file: \"{0}\" -> \"{1}\"", e.OldFullPath, e.FullPath);
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