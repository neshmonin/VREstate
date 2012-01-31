using System.IO;
using System.Collections.Generic;
using Vre.Server.BusinessLogic;
namespace Vre.Server.ModelCache
{
    internal class ModelCacheManager
    {
        private string _modelStorePath;
        private string _modelStoreFilter;
        private FileSystemWatcher _watcher;
        private Dictionary<string, ModelCache> _cache;
        private Dictionary<string, ModelCache> _cacheByName;

        public ModelCacheManager(string modelStorePath)
        {
            _watcher = null;

            _modelStorePath = Path.GetDirectoryName(modelStorePath);
            _modelStoreFilter = Path.GetFileName(modelStorePath);
        }

        public void Initialize()
        {
            lock (this)
            {
                if (_watcher != null) return;

                _watcher = new FileSystemWatcher(_modelStorePath);
                _watcher.Filter = Path.GetFileName(_modelStoreFilter);
                _watcher.EnableRaisingEvents = false;

                _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
                _watcher.Created += new FileSystemEventHandler(_watcher_Created);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
                _watcher.Error += new ErrorEventHandler(_watcher_Error);

                ServiceInstances.Logger.Info("MC: Started reading model files.");

                _cache = new Dictionary<string, ModelCache>();
                _cacheByName = new Dictionary<string, ModelCache>();

                foreach (string path in Directory.EnumerateFiles(_modelStorePath, _modelStoreFilter, SearchOption.TopDirectoryOnly))
                {
                    tryAddNewModel(path);
                }

                ServiceInstances.Logger.Info("MC: Reading model files done.");

                _watcher.EnableRaisingEvents = true;
            }
        }

        public bool FillWithModelInfo(Site site, bool withSubObjects)
        {
            lock (this)
            {
                if (null == _watcher) return false;  // not initialized yet

                ModelCache mc;
                if (_cacheByName.TryGetValue(site.Name, out mc))
                {
                    mc.UpdateBo(site, withSubObjects);
                    return true;
                }
                return false;
            }
        }

        public bool FillWithModelInfo(Building building, bool withSubObjects)
        {
            lock (this)
            {
                if (null == _watcher) return false;  // not initialized yet

                ModelCache mc;
                if (_cacheByName.TryGetValue(building.ConstructionSite.Name, out mc))
                {
                    mc.UpdateBo(building, withSubObjects);
                    return true;
                }
                return false;
            }
        }

        public bool FillWithModelInfo(Suite suite, bool withSubObjects)
        {
            lock (this)
            {
                if (null == _watcher) return false;  // not initialized yet

                ModelCache mc;
                if (_cacheByName.TryGetValue(suite.Building.ConstructionSite.Name, out mc))
                {
                    mc.UpdateBo(suite, withSubObjects);
                    return true;
                }
                return false;
            }
        }

        public SuiteClass[] GetSuiteClassList(Building building)
        {
            lock (this)
            {
                if (null == _watcher) return null;  // not initialized yet

                ModelCache mc;
                if (_cacheByName.TryGetValue(building.ConstructionSite.Name, out mc))
                {
                    return mc.GetSuiteClassList(building);
                }
                return null;
            }
        }

        private bool tryAddNewModel(string path)
        {
            ModelCache mc = new ModelCache(path);
            if (mc.IsValid)
            {
                if (_cacheByName.ContainsKey(mc.SiteName))
                {
                    ModelCache emc = _cacheByName[mc.SiteName];
                    if (emc.UpdatedTime > mc.UpdatedTime)
                    {
                        ServiceInstances.Logger.Warn("MC: Duplicate site name ({2}) found in \"{0}\" and \"{1}\"; former used judging by file update timestamp.",
                            emc.FilePath, path, mc.SiteName);
                    }
                    else
                    {
                        ServiceInstances.Logger.Warn("MC: Duplicate site name ({2}) found in \"{0}\" and \"{1}\"; latter used judging by file update timestamp.",
                            emc.FilePath, path, mc.SiteName);

                        _cache.Remove(emc.FilePath);
                        _cacheByName.Remove(mc.SiteName);

                        _cache.Add(path, mc);
                        _cacheByName.Add(mc.SiteName, mc);
                    }
                }
                else
                {
                    _cache.Add(path, mc);
                    _cacheByName.Add(mc.SiteName, mc);

                    ServiceInstances.Logger.Info("MC: Added new model file: \"{0}\"; site '{1}'", path, mc.SiteName);
                }
            }
            return mc.IsValid;
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
                if (_cache.ContainsKey(e.OldFullPath))
                {
                    ModelCache mc = _cache[e.OldFullPath];
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
                if (_cache.ContainsKey(e.FullPath))
                {
                    // TODO: is this proper behavior?
                    string name = _cache[e.FullPath].SiteName;
                    _cache.Remove(e.FullPath);
                    _cacheByName.Remove(name);
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
                if (_cache.ContainsKey(e.FullPath))
                {
                    ModelCache mc = _cache[e.FullPath];
                    string name = mc.SiteName;
                    if (mc.TryReRead(e.FullPath))
                    {
                        ServiceInstances.Logger.Info("MC: Re-read model file: \"{0}\"", e.FullPath);

                        if (!name.Equals(mc.SiteName))
                        {
                            ModelCache emc;
                            if (_cacheByName.TryGetValue(mc.SiteName, out emc))
                            {
                                ServiceInstances.Logger.Warn("MC: Model file re-read (\"{0}\") resulted in site name change: '{1}'->'{2}' which resulted in model \"{3}\" being removed from cache.",
                                    e.FullPath, name, mc.SiteName, emc.FilePath);
                         
                                _cacheByName.Remove(mc.SiteName);
                            }
                            else
                            {
                                ServiceInstances.Logger.Warn("MC: Model file re-read (\"{0}\") resulted in site name change: '{1}'->'{2}'", 
                                    e.FullPath, name, mc.SiteName);
                            }

                            _cacheByName.Remove(name);
                            _cacheByName.Add(mc.SiteName, mc);
                        }
                    }
                }
            }
        }
    }
}